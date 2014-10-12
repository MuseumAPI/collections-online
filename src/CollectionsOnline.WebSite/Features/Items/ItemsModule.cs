﻿using CollectionsOnline.Core.Models;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebSite.Features.Items
{
    public class ItemsModule : NancyModule
    {
        public ItemsModule(            
            IItemViewModelQuery itemViewModelQuery,
            IDocumentSession documentSession)            
        {
            Get["/items/{id}"] = parameters =>
            {
                var item = documentSession.Load<Item>("items/" + parameters.id as string);

                return (item == null || item.IsHidden) ? HttpStatusCode.NotFound : View["items", itemViewModelQuery.BuildItem("items/" + parameters.id)];
            };
        }
    }
}