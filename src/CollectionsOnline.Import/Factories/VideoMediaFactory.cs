﻿using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using CollectionsOnline.Core.Models;
using Google.Apis.YouTube.v3;
using ImageProcessor;
using ImageProcessor.Imaging;
using Serilog;

namespace CollectionsOnline.Import.Factories
{
    public class VideoMediaFactory : IVideoMediaFactory
    {
        private readonly YouTubeService _youTubeService;

        public VideoMediaFactory(
            YouTubeService youTubeService)
        {
            _youTubeService = youTubeService;            
        }

        public bool Make(ref VideoMedia videoMedia)
        {
            var stopwatch = Stopwatch.StartNew();

            var uri = new Uri(videoMedia.Uri);
            videoMedia.VideoId = !string.IsNullOrWhiteSpace(uri.Segments.LastOrDefault()) ? uri.Segments.Last() : null;

            if (!string.IsNullOrWhiteSpace(videoMedia.VideoId))
            {
                if (ThereAreExistingMedia(ref videoMedia))
                {
                    stopwatch.Stop();
                    Log.Logger.Debug("Found existing video preview {Irn} in {ElapsedMilliseconds} ms", videoMedia.Irn, stopwatch.ElapsedMilliseconds);

                    return true;
                }

                try
                {
                    var request = _youTubeService.Videos.List("snippet");
                    request.Id = videoMedia.VideoId;

                    var listResponse = request.Execute();

                    var youtubeVideo = listResponse.Items.FirstOrDefault();

                    if (youtubeVideo != null)
                    {
                        // try and find highest resolution
                        var thumbnail = youtubeVideo.Snippet.Thumbnails.Maxres ?? youtubeVideo.Snippet.Thumbnails.High;

                        using (var imageFactory = new ImageFactory())
                        using (var webClient = new WebClient())
                        using (var memoryStream = new MemoryStream(webClient.DownloadData(thumbnail.Url)))
                        {
                            // Create thumbnail
                            var destPath = PathFactory.MakeDestPath(videoMedia.Irn, ".jpg", FileDerivativeType.Thumbnail);

                            imageFactory
                                .Load(memoryStream)
                                .Resize(new ResizeLayer(new Size(250, 250), ResizeMode.Crop))
                                .Quality(80)
                                .Save(destPath);

                            stopwatch.Stop();
                            Log.Logger.Debug("Loaded video preview {Irn} in {ElapsedMilliseconds} ms", videoMedia.Irn, stopwatch.ElapsedMilliseconds);
                            stopwatch.Reset();
                            stopwatch.Start();

                            videoMedia.Thumbnail = new ImageMediaFile
                            {
                                Uri = PathFactory.MakeUriPath(videoMedia.Irn, ".jpg", FileDerivativeType.Thumbnail),
                                Size = new FileInfo(destPath).Length,
                                Width = imageFactory.Image.Width,
                                Height = imageFactory.Image.Height
                            };

                            // Create medium preview placeholder
                            imageFactory.Reset();

                            destPath = PathFactory.MakeDestPath(videoMedia.Irn, ".jpg", FileDerivativeType.Medium);

                            imageFactory
                                .Resize(new ResizeLayer(new Size(0, 500), ResizeMode.Max))
                                .Quality(80)
                                .Save(destPath);

                            videoMedia.Medium = new ImageMediaFile
                            {
                                Uri = PathFactory.MakeUriPath(videoMedia.Irn, ".jpg", FileDerivativeType.Medium),
                                Size = new FileInfo(destPath).Length,
                                Width = imageFactory.Image.Width,
                                Height = imageFactory.Image.Height
                            };
                        }

                        stopwatch.Stop();
                        Log.Logger.Debug("Completed video preview {Irn} creation in {ElapsedMilliseconds} ms", videoMedia.Irn, stopwatch.ElapsedMilliseconds);

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Log.Logger.Fatal(ex, "Unexpected error occured creating video preview {Irn}", videoMedia.Irn);
                    throw;
                }
            }

            return false;
        }

        private bool ThereAreExistingMedia(ref VideoMedia videoMedia)
        {
            // First check to see if we are not overwriting existing data,
            // then if we find existing files matching all of our image media, use the files on disk instead
            if (!bool.Parse(ConfigurationManager.AppSettings["OverwriteExistingMedia"]))
            {
                var destPathThumbnail = PathFactory.MakeDestPath(videoMedia.Irn, ".jpg", FileDerivativeType.Thumbnail);
                var destPathMedium = PathFactory.MakeDestPath(videoMedia.Irn, ".jpg", FileDerivativeType.Medium);

                if (File.Exists(destPathThumbnail) &&
                    File.Exists(destPathMedium))
                {
                    using (var imageFactory = new ImageFactory())
                    {
                        // Thumbnail
                        imageFactory.Load(destPathThumbnail);

                        videoMedia.Thumbnail = new ImageMediaFile
                        {
                            Uri = PathFactory.MakeUriPath(videoMedia.Irn, ".jpg", FileDerivativeType.Thumbnail),
                            Size = new FileInfo(destPathThumbnail).Length,
                            Width = imageFactory.Image.Width,
                            Height = imageFactory.Image.Height
                        };

                        // Medium preview placeholder
                        imageFactory.Load(destPathMedium);

                        videoMedia.Medium = new ImageMediaFile
                        {
                            Uri = PathFactory.MakeUriPath(videoMedia.Irn, ".jpg", FileDerivativeType.Medium),
                            Size = new FileInfo(destPathMedium).Length,
                            Width = imageFactory.Image.Width,
                            Height = imageFactory.Image.Height
                        };
                    }

                    return true;
                }
            }

            return false;
        }
    }
}