﻿using System;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using IMu;
using CollectionsOnline.Core.Extensions;

namespace CollectionsOnline.Import.Factories
{
    public class MuseumLocationFactory : IMuseumLocationFactory
    {
        public MuseumLocation Make(Map map)
        {
            if (map == null || map.GetString("LocLocationType") == null)
                return null;

            if(map.GetString("LocLocationType").Contains("holder", StringComparison.OrdinalIgnoreCase))
                return Make(map.GetMap("location"));
            
            var locationKey = new Tuple<string, string, string, string>(
                map.GetString("LocLevel1"), 
                map.GetString("LocLevel2"),
                map.GetString("LocLevel3"),
                map.GetString("LocLevel4"));

            return Constants.MuseumLocations.ContainsKey(locationKey) ? Constants.MuseumLocations[locationKey] : null;
        }
    }
}