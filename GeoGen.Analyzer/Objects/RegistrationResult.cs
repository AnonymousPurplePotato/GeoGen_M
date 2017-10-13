﻿using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Objects
{
    internal class RegistrationResult
    {
        public bool CanBeConstructed { get; set; }

        public Dictionary<ConfigurationObject, ConfigurationObject> DuplicateObjects { get; set; }
    }
}
