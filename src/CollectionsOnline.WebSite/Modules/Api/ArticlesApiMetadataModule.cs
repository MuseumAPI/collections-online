﻿using System.Collections.Generic;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models.Api;
using Nancy;
using Nancy.Metadata.Modules;
using Newtonsoft.Json;
using Raven.Client;

namespace CollectionsOnline.WebSite.Modules.Api
{
    public class ArticlesApiMetadataModule : MetadataModule<ApiMetadata>
    {
        public ArticlesApiMetadataModule(IDocumentStore documentStore)
        {
            using (var documentSession = documentStore.OpenSession())
            {
                var sampleArticle = documentSession.Advanced.DocumentQuery<Article, CombinedIndex>()
                    .WhereEquals("RecordType", "Article")
                    .FirstOrDefault();

                Describe["articles-api-index"] = description =>
                {
                    return new ApiMetadata
                    {
                        Name = description.Name,
                        Method = description.Method,
                        Path = description.Path.Replace(Constants.CurrentApiVersionPathSegment, string.Empty),
                        Description = "Returns a bunch of articles.",
                        StatusCodes = new Dictionary<HttpStatusCode, string>
                        {
                            {HttpStatusCode.OK, "A bunch of articles were able to be retrieved ok."}
                        },
                        SampleResponse = JsonConvert.SerializeObject(new[] { Mapper.Map<Article, ArticleApiViewModel>(sampleArticle) }, Formatting.Indented),
                        ExampleUrl = description.Path.Replace(Constants.CurrentApiVersionPathSegment, string.Empty)
                    };
                };

                Describe["articles-api-by-id"] = description =>
                {
                    return new ApiMetadata
                    {
                        Name = description.Name,
                        Method = description.Method,
                        Path = description.Path.Replace(Constants.CurrentApiVersionPathSegment, string.Empty),
                        Description = "Returns a single article by Id.",
                        Parameters = new []
                        {
                            new ApiParameter
                            {
                                Parameter = "Id",
                                Description = "Id of article to be retrieved",
                                Type = "Integer"
                            }
                        },
                        StatusCodes = new Dictionary<HttpStatusCode, string>
                        {
                            {HttpStatusCode.OK, "The article was found and retrieved ok."},
                            {HttpStatusCode.NotFound, "The article could not be found and probably does not exist."}
                        },
                        SampleResponse = JsonConvert.SerializeObject(Mapper.Map<Article, ArticleApiViewModel>(sampleArticle), Formatting.Indented),
                        ExampleUrl = (sampleArticle != null) ? string.Format("/{0}/{1}", Constants.ApiBasePath, sampleArticle.Id) : null,
                    };
                };
            }
        }
    }
}