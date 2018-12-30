﻿using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a <see cref="GeometricalObject"/> that can be defined by <see cref="PointObject"/>s.
    /// </summary>
    public abstract class DefinableByPoints : GeometricalObject
    {
        #region Public properties

        /// <summary>
        /// Gets the points that lie on this object.
        /// </summary>
        public HashSet<PointObject> Points { get; }

        #endregion

        #region Public abstract properties

        /// <summary>
        /// Gets the minimal number of distinct points that are needed to define this type of object.
        /// </summary>
        public abstract int NumberOfNeededPoints { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of the <see cref="DefinableByPoints"/> class wrapping a given circle <see cref="ConfigurationObject"/>.
        /// </summary>
        /// <param name="configurationObject">The configuration object represented by this geometrical object.</param>
        protected DefinableByPoints(ConfigurationObject configurationObject)
                : base(configurationObject)
        {
            Points = new HashSet<PointObject>();
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="DefinableByPoints"/> class by <see cref="PointObject"/>s.
        /// </summary>
        /// <param name="points">The points that define this object.</param>
        protected DefinableByPoints(params PointObject[] points)
                : base(configurationObject: null)
        {
            Points = new HashSet<PointObject>(points);
        }

        #endregion
    }
}