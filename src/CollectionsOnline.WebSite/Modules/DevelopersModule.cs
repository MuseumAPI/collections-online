﻿using CollectionsOnline.WebSite.Factories;
using Nancy;
using Nancy.Routing;

namespace CollectionsOnline.WebSite.Modules
{
    public class DevelopersModule : NancyModule
    {
        public DevelopersModule(
            IRouteCacheProvider routeCacheProvider,
            IDevelopersViewModelFactory developersViewModelFactory,
            IMetadataViewModelFactory metadataViewModelFactory)            
        {
            Get["developers-index", "/developers"] = parameters =>
            {
                ViewBag.metadata = metadataViewModelFactory.MakeDevelopersIndex();

                return View["DevelopersIndex", developersViewModelFactory.MakeDevelopersIndex(routeCacheProvider.GetCache(), Request)];
            };
        }
    }
}