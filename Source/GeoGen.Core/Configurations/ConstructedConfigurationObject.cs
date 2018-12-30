﻿using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represent a <see cref="ConfigurationObject"/> that is composed of a <see cref="Core.Construction"/>, and 
    /// <see cref="Arguments"/> that hold actual configuration objects from which this object should be constructed.
    /// </summary>
    public class ConstructedConfigurationObject : ConfigurationObject
    {
        #region Public properties

        /// <summary>
        /// Gets the construction that should be used to draw this object.
        /// </summary>
        public Construction Construction { get; }

        /// <summary>
        /// Gets the arguments that should be passed to the construction function.
        /// </summary>
        public Arguments PassedArguments { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructedConfigurationObject"/> class.
        /// </summary>
        /// <param name="construction">The construction that should be used to draw this object.</param>
        /// <param name="arguments">The arguments that should be passed to the construction function.</param>
        public ConstructedConfigurationObject(Construction construction, Arguments arguments)
            : base(construction.OutputType)
        {
            Construction = construction ?? throw new ArgumentNullException(nameof(construction));
            PassedArguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructedConfigurationObject"/> class.
        /// </summary>
        /// <param name="construction">The construction that should be used to draw this object.</param>
        /// <param name="argumentsList">The list of arguments that should be passed to the construction function.</param>
        /// <param name="id">The id of the object.</param>
        public ConstructedConfigurationObject(Construction construction, List<ConstructionArgument> argumentsList)
            : this(construction, new Arguments(argumentsList))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructedConfigurationObject"/> class.
        /// </summary>
        /// <param name="construction">The construction that should be used to draw this object.</param>
        /// <param name="arguments">The arguments that should be passed to the construction function.</param>
        /// <param name="id">The id of the object.</param>
        public ConstructedConfigurationObject(Construction construction, Arguments arguments, int id) 
            : this(construction, arguments)
        {
            Id = id;
        }

        #endregion

        #region ConfigurationObject overridden methods

        /// <summary>
        /// Enumerates the objects that are internally used to define this configuration object.
        /// </summary>
        /// <returns>The enumeration of the internal objects.</returns>
        public override IEnumerable<ConfigurationObject> GetInternalObjects()
        {
            // Take the passed objects from the arguments....
            return PassedArguments.FlattenedList
                // Add the internal objects to each of them
                .Concat(PassedArguments.FlattenedList.Select(obj => obj.GetInternalObjects()).Flatten())
                // Take only distinct ones
                .Distinct();
        }

        #endregion
    }
}