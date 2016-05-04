﻿using CollectionsOnline.Core.Models;
using IMu;

namespace CollectionsOnline.Import.Factories
{
    public interface ILicenceFactory
    {
        Licence MakeMediaLicence(string licence);

        Licence MakeItemSpecimenLicence(Map map);
    }
}