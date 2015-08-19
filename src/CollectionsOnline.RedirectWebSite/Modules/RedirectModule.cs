﻿using System.Configuration;
using Nancy;
using Nancy.Responses;

namespace CollectionsOnline.RedirectWebSite.Modules
{
    public class RedirectModule : NancyModule
    {
        public RedirectModule()
        {
            var canonicalSiteBase = ConfigurationManager.AppSettings["CanonicalSiteBase"];

            Get["/"] = parameters => new RedirectResponse(canonicalSiteBase, RedirectResponse.RedirectType.Permanent);

            Get["/search"] = parameters => new RedirectResponse(string.Format("{0}search", canonicalSiteBase), RedirectResponse.RedirectType.Permanent);

            // Phar lap
            Get["/items/1499868/{name?}"] = parameters => new RedirectResponse(string.Format("{0}specimens/139139", canonicalSiteBase), RedirectResponse.RedirectType.Permanent);
            // Sam the koala
            Get["/items/1550331/{name?}"] = parameters => new RedirectResponse(string.Format("{0}specimens/1487596", canonicalSiteBase), RedirectResponse.RedirectType.Permanent);

            Get["/items/{id:int}/{name*}"] = parameters => new RedirectResponse(string.Format("{0}items/{1}", canonicalSiteBase, parameters.id), RedirectResponse.RedirectType.Permanent);

            Get["/themes/{id:int}/{name*}"] = parameters => new RedirectResponse(string.Format("{0}articles/{1}", canonicalSiteBase, parameters.id), RedirectResponse.RedirectType.Permanent);

            Get["/{everythingelse*}"] = parameters => new RedirectResponse(canonicalSiteBase, RedirectResponse.RedirectType.Permanent);
        }
    }
}