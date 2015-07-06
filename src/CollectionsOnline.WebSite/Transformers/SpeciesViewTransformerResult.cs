﻿using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;

namespace CollectionsOnline.WebSite.Transformers
{
    public class SpeciesViewTransformerResult
    {
        public Species Species { get; set; }

        public IList<Media> SpeciesMedia { get; set; }

        public IList<EmuAggregateRootViewModel> RelatedItems { get; set; }

        public IList<EmuAggregateRootViewModel> RelatedSpecimens { get; set; }

        public int RelatedSpecimenCount { get; set; }        

        public string JsonSpeciesMedia { get; set; }

        public SpeciesViewTransformerResult()
        {
            SpeciesMedia = new List<Media>();
            RelatedItems = new List<EmuAggregateRootViewModel>();
            RelatedSpecimens = new List<EmuAggregateRootViewModel>();
        }
    }
}