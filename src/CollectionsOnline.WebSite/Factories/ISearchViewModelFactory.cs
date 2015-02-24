﻿using System.Collections.Generic;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.WebSite.Models;
using Nancy;
using Raven.Abstractions.Data;

namespace CollectionsOnline.WebSite.Factories
{
    public interface ISearchViewModelFactory
    {
        SearchViewModel MakeViewModel(IList<CombinedResult> results, FacetResults facets, List<string> suggestions, Request request, int totalResults, SearchInputModel searchInputModel, long queryTimeElapsed, long facetTimeElapsed);
    }
}