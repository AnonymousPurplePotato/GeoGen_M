﻿using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// A static class for creating <see cref="PredefinedConstruction"/>s.
    /// </summary>
    public static class PredefinedConstructions
    {
        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.CenterOfCircle"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction CenterOfCircle
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(ConfigurationObjectType.Circle)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.CenterOfCircle, parameters, ConfigurationObjectType.Point);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.Circumcircle"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction Circumcircle
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 3)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.Circumcircle, parameters, ConfigurationObjectType.Circle);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.CircleWithCenterThroughPoint"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction CircleWithCenterThroughPoint
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(ConfigurationObjectType.Point),
                    new ObjectConstructionParameter(ConfigurationObjectType.Point),
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.CircleWithCenterThroughPoint, parameters, ConfigurationObjectType.Circle);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.InternalAngleBisector"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction InternalAngleBisector
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(ConfigurationObjectType.Point),
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.InternalAngleBisector, parameters, ConfigurationObjectType.Line);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.IntersectionOfLines"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction IntersectionOfLines
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Line), 2)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.IntersectionOfLines, parameters, ConfigurationObjectType.Point);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.LineFromPoints"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction LineFromPoints
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.LineFromPoints, parameters, ConfigurationObjectType.Line);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.Midpoint"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction Midpoint
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.Midpoint, parameters, ConfigurationObjectType.Point);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.PerpendicularProjection"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction PerpendicularProjection
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(ConfigurationObjectType.Point),
                    new ObjectConstructionParameter(ConfigurationObjectType.Line)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.PerpendicularProjection, parameters, ConfigurationObjectType.Point);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.PerpendicularLine"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction PerpendicularLine
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(ConfigurationObjectType.Point),
                    new ObjectConstructionParameter(ConfigurationObjectType.Line)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.PerpendicularLine, parameters, ConfigurationObjectType.Line);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.ParallelLine"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction ParallelLine
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(ConfigurationObjectType.Point),
                    new ObjectConstructionParameter(ConfigurationObjectType.Line)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.ParallelLine, parameters, ConfigurationObjectType.Line);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.PointReflection"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction PointReflection
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(ConfigurationObjectType.Point),
                    new ObjectConstructionParameter(ConfigurationObjectType.Point)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.PointReflection, parameters, ConfigurationObjectType.Point);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.SecondIntersectionOfCircleAndLineFromPoints"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction SecondIntersectionOfCircleAndLineFromPoints
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(ConfigurationObjectType.Point),
                    new ObjectConstructionParameter(ConfigurationObjectType.Point),
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.SecondIntersectionOfCircleAndLineFromPoints, parameters, ConfigurationObjectType.Point);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.SecondIntersectionOfCircleWithCenterAndLineFromPoints"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction SecondIntersectionOfCircleWithCenterAndLineFromPoints
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(ConfigurationObjectType.Point),
                    new ObjectConstructionParameter(ConfigurationObjectType.Point),
                    new ObjectConstructionParameter(ConfigurationObjectType.Point),
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.SecondIntersectionOfCircleWithCenterAndLineFromPoints, parameters, ConfigurationObjectType.Point);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.SecondIntersectionOfTwoCircumcircles"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction SecondIntersectionOfTwoCircumcircles
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(ConfigurationObjectType.Point),
                    new SetConstructionParameter(new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2), 2)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.SecondIntersectionOfTwoCircumcircles, parameters, ConfigurationObjectType.Point);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.RandomPointOnLine"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction RandomPointOnLine
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(ConfigurationObjectType.Line)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.RandomPointOnLine, parameters, ConfigurationObjectType.Point);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.RandomPointOnLineFromPoints"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction RandomPointOnLineFromPoints
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new SetConstructionParameter(new ObjectConstructionParameter(ConfigurationObjectType.Point), 2)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.RandomPointOnLineFromPoints, parameters, ConfigurationObjectType.Point);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.RandomPointOnCircle"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction RandomPointOnCircle
        {
            get
            {
                // Create the parameters
                var parameters = new List<ConstructionParameter>
                {
                    new ObjectConstructionParameter(ConfigurationObjectType.Circle)
                };

                // Create the actual construction
                return new PredefinedConstruction(PredefinedConstructionType.RandomPointOnCircle, parameters, ConfigurationObjectType.Point);
            }
        }

        /// <summary>
        /// Creates a <see cref="PredefinedConstructionType.RandomPoint"/> construction.
        /// </summary>
        /// <returns>The construction.</returns>
        public static PredefinedConstruction RandomPoint
        {
            // Create the actual construction, which has no parameters
            get => new PredefinedConstruction(PredefinedConstructionType.RandomPoint, new ConstructionParameter[0], ConfigurationObjectType.Point);
        }
    }
}