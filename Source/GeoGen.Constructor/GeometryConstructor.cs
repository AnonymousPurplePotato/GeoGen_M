﻿using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The default implementation of <see cref="IGeometryConstructor"/>. 
    /// </summary>
    public class GeometryConstructor : IGeometryConstructor
    {
        #region Dependencies

        /// <summary>
        /// The factory for creating pictures.
        /// </summary>
        private readonly IPicturesOfConfigurationFactory _factory;

        /// <summary>
        /// The resolver of object constructors for particular constructions.
        /// </summary>
        private readonly IConstructorsResolver _resolver;

        /// <summary>
        /// The tracer for objects that couldn't be constructed because of inconsistencies between pictures.
        /// </summary>
        private readonly IGeometryConstructionFailureTracer _tracer;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryConstructor"/> class.
        /// </summary>
        /// <param name="factory">The factory for creating pictures.</param>
        /// <param name="resolver">The resolver of object constructors for particular constructions.</param>
        /// <param name="tracer">The tracer for objects that couldn't be constructed because of inconsistencies between pictures.</param>
        public GeometryConstructor(IPicturesOfConfigurationFactory factory, IConstructorsResolver resolver, IGeometryConstructionFailureTracer tracer = null)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _tracer = tracer;
        }

        #endregion

        #region IGeometryConstructor implementation

        /// <summary>
        /// Constructs a given <see cref="Configuration"/>. Throws a <see cref="GeometryConstructionException"/>
        /// if the construction couldn't be carried out.
        /// </summary>
        /// <param name="configuration">The configuration to be constructed.</param>
        /// <returns>The tuple consisting of the pictures and the construction data.</returns>
        public (PicturesOfConfiguration pictures, ConstructionData data) Construct(Configuration configuration)
        {
            // Create pictures for the configuration
            var pictures = _factory.CreatePictures(configuration);

            // First we add loose objects to all pictures
            foreach (var picture in pictures)
            {
                // Objects are constructed using the private helper method
                picture.AddObjects(configuration.LooseObjects, () => Construct(configuration.LooseObjectsHolder.Layout));
            }

            // Then we add all the constructed object
            foreach (var constructedObject in configuration.ConstructedObjects)
            {
                // Prepare the variable holding the final result
                ConstructionData data = null;

                try
                {
                    // Execute the construction using our helper function
                    pictures.ExecuteAndReconstructAtIncosistencies(
                        // Call the internal construction function
                        () => data = ConstructObject(constructedObject, pictures, addToPictures: true),
                        // Trace any inconsistency exception
                        e => _tracer?.TraceInconsistencyWhileDrawingConfiguration(configuration, constructedObject, e.Message));
                }
                // If there are unresolvable inconsistencies...
                catch (UnresolvedInconsistencyException e)
                {
                    // We trace it
                    _tracer?.TraceUnresolvedInconsistencyWhileDrawingConfiguration(configuration, e.Message);

                    // And re-throw the exception
                    throw new GeometryConstructionException("The configuration couldn't be constructed.", e);
                }

                // At this point the construction of the object is completed
                // Find out if the result is correct
                var correctResult = data.InconstructibleObject == null && data.Duplicates == default;

                // If it's not, we directly return the current data without dealing with the remaining objects
                if (!correctResult)
                    return (null, data);
            }

            // If we got here, then there are no inconstructible objects and no duplicates
            return (pictures, new ConstructionData(default, default));
        }

        /// <summary>
        /// Constructs a given <see cref="Configuration"/> using an already constructed old one.
        /// It is assumed that the new configuration differs only by the last object from the already 
        /// constructed one. Thus only the last object is constructed. Throws a 
        /// <see cref="GeometryConstructionException"/> if the construction couldn't be carried out.
        /// </summary>
        /// <param name="oldConfigurationPictures">The pictures where the old configuration is drawn.</param>
        /// <param name="newConfiguration">The new configuration that should be drawn.</param>
        /// <returns>The tuple consisting of the pictures and the construction data.</returns>
        public (PicturesOfConfiguration pictures, ConstructionData data) ConstructByCloning(PicturesOfConfiguration oldConfigurationPictures, Configuration newConfiguration)
        {
            // Clone the pictures
            var pictures = oldConfigurationPictures.Clone(newConfiguration);

            try
            {
                // Prepare the result
                var data = default(ConstructionData);

                // Execute the construction without adding the object to the picture
                pictures.ExecuteAndReconstructAtIncosistencies(
                    // Call the internal construction function
                    () => data = ConstructObject(newConfiguration.LastConstructedObject, pictures, addToPictures: true),
                    // Trace any inconsistency exception
                    e => _tracer?.TraceInconsistencyWhileDrawingConfiguration(newConfiguration, newConfiguration.LastConstructedObject, e.Message));

                // Return the data
                return (pictures, data);
            }
            // If there are unresolvable inconsistencies...
            catch (UnresolvedInconsistencyException e)
            {
                // We trace it
                _tracer?.TraceInconsistencyWhileDrawingConfiguration(newConfiguration, newConfiguration.LastConstructedObject, e.Message);

                // And re-throw the exception
                throw new GeometryConstructionException("The configuration couldn't be constructed.", e);
            }
        }

        /// <summary>
        /// Constructs a given <see cref="ConstructedConfigurationObject"/>. It is assumed that the constructed 
        /// object can be construed in each of the passed pictures using its objects or its remembered duplicates.
        /// Throws a <see cref="GeometryConstructionException"/> if the construction couldn't be carried out.
        /// </summary>
        /// <param name="pictures">The pictures that should contain the input for the construction.</param>
        /// <param name="constructedObject">The object that is about to be constructed.</param>
        /// <param name="addToPictures">Indicates if we should add the object to the pictures.</param>
        /// <returns>The construction data.</returns>
        public ConstructionData Construct(Pictures pictures, ConstructedConfigurationObject constructedObject, bool addToPictures)
        {
            try
            {
                // Prepare the result
                var data = default(ConstructionData);

                // Execute the construction
                pictures.ExecuteAndReconstructAtIncosistencies(
                    // Call the internal construction function
                    () => data = ConstructObject(constructedObject, pictures, addToPictures),
                    // Trace any inconsistency exception
                    // TODO: Change API
                    e => _tracer?.TraceInconsistencyWhileExaminingObject(null, constructedObject, e.Message));

                // Return the data
                return data;
            }
            // If there are unresolvable inconsistencies...
            catch (UnresolvedInconsistencyException e)
            {
                // We trace it
                // TODO: Change API
                _tracer?.TraceUnresolvedInconsistencyWhileExaminingObject(null, constructedObject, e.Message);

                // And re-throw the exception
                throw new GeometryConstructionException("The object couldn't be examined.", e);
            }
        }

        /// <summary>
        /// Constructs a given <see cref="ConstructedConfigurationObject"/> without adding it to the pictures.
        /// It is assumed that the constructed object can be construed in the passed pictures. The fact whether
        /// the object is or is not already present in individual pictures is ignored. If the object is 
        /// inconstructible, null is returned. Throws a <see cref="GeometryConstructionException"/> if the 
        /// construction couldn't be carried out.
        /// </summary>
        /// <param name="pictures">The pictures that should contain the input for the construction.</param>
        /// <param name="constructedObject">The object that is about to be constructed.</param>
        /// <returns>The dictionary mapping pictures to constructed objects, or null; if the object is inconstructible.</returns>
        public IReadOnlyDictionary<Picture, IAnalyticObject> Construct(Pictures pictures, ConstructedConfigurationObject constructedObject)
        {
            try
            {
                // Prepare the result
                var result = default(IReadOnlyDictionary<Picture, IAnalyticObject>);

                // Execute the construction without adding the object to the picture
                pictures.ExecuteAndReconstructAtIncosistencies(
                    // Call the internal construction function
                    () => result = ConstructObject(pictures, constructedObject),
                    // Trace any inconsistency exception
                    // TODO: Change API
                    e => _tracer?.TraceInconsistencyWhileExaminingObject(null, constructedObject, e.Message));

                // Return the data
                return result;
            }
            // If there are unresolvable inconsistencies...
            catch (UnresolvedInconsistencyException e)
            {
                // We trace it
                // TODO: Change API
                _tracer?.TraceUnresolvedInconsistencyWhileExaminingObject(null, constructedObject, e.Message);

                // And re-throw the exception
                throw new GeometryConstructionException("The object couldn't be examined.", e);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Performs the construction of a given constructed object with respect to all the pictures of given pictures.
        /// </summary>
        /// <param name="constructedObject">The object to be constructed.</param>
        /// <param name="pictures">The pictures where the input for the construction is drawn.</param>
        /// <returns>The map of pictures and construction results, if it can be performed; or null, if not.</returns>
        private IReadOnlyDictionary<Picture, IAnalyticObject> ConstructObject(Pictures pictures, ConstructedConfigurationObject constructedObject)
        {
            // Prepare the result
            var result = new Dictionary<Picture, IAnalyticObject>();

            // Initialize a variable indicating if the construction is possible
            bool canBeConstructed = default;

            // Let the resolver find the constructor and let it create the constructor function
            var constructorFunction = _resolver.Resolve(constructedObject.Construction).Construct(constructedObject);

            // Construct it in every picture
            foreach (var picture in pictures)
            {
                // Perform the construction
                var analyticObject = constructorFunction(picture);

                // Find out if it's been constructed
                var objectConstructed = analyticObject != null;

                // We need to first check if some other picture didn't mark constructibility in the opposite way
                // If yes, we have an inconsistency
                if (picture != pictures.First() && canBeConstructed != objectConstructed)
                    throw new InconsistentPicturesException("The fact whether the object can be constructed was not determined consistently.");

                // Mark the construction result
                canBeConstructed = objectConstructed;

                // If the object can be constructed, add it to the result
                result.Add(picture, analyticObject);
            }

            // If the object can be constructed, return the result, otherwise null
            return canBeConstructed ? result : null;
        }

        /// <summary>
        /// Performs the construction of a given constructed object with respect to all the pictures of given pictures.
        /// </summary>
        /// <param name="constructedObject">The object to be constructed.</param>
        /// <param name="pictures">The pictures where the input for the construction is drawn.</param>
        /// <param name="addToPictures">Indicates if the object should be added to the pictures.</param>
        /// <returns>The result of the construction.</returns>
        private ConstructionData ConstructObject(ConstructedConfigurationObject constructedObject, Pictures pictures, bool addToPictures)
        {
            // Initialize a variable indicating if the construction is possible
            bool canBeConstructed = default;

            // Initialize a variable holding a potential duplicate version of the object
            ConfigurationObject duplicate = default;

            // Let the resolver find the constructor and let it create the constructor function
            var constructorFunction = _resolver.Resolve(constructedObject.Construction).Construct(constructedObject);

            // Construct it in every picture
            foreach (var picture in pictures)
            {
                // Prepare value indicating whether the object was constructed in the picture
                var objectConstructed = default(bool);

                // Prepare value holding a potential equal object in the picture to this object
                var equalObject = default(ConfigurationObject);

                // If we are supposed to add the object...
                if (addToPictures)
                {
                    // Then ask the picture to do it (it will perform the construction)
                    picture.TryAdd(constructedObject, () => constructorFunction(picture), out objectConstructed, out equalObject);
                }
                // Otherwise we need to perform the construction here
                else
                {
                    // Construct it
                    var analyticObject = constructorFunction(picture);

                    // Set if it the construction went fine
                    objectConstructed = analyticObject != null;

                    // Set if there is an equal object
                    equalObject = analyticObject != null && picture.Contains(analyticObject) ? picture.Get(analyticObject) : null;
                }

                // We need to first check if some other picture didn't mark constructibility in the opposite way
                // If yes, we have an inconsistency
                if (picture != pictures.First() && canBeConstructed != objectConstructed)
                    throw new InconsistentPicturesException("The fact whether the object can be constructed was not determined consistently.");

                // Now we need to check if some other picture didn't find a different duplicate 
                // If yes, we have an inconsistency
                if (picture != pictures.First() && duplicate != equalObject)
                    throw new InconsistentPicturesException("The fact whether the object has an equal version was not determined consistently.");

                // If there is an equal object and we could manipulate the picture, mark the equality
                if (equalObject != null && addToPictures)
                    picture.MarkDuplicate(equalObject, constructedObject);

                // Set the found values
                canBeConstructed = objectConstructed;
                duplicate = equalObject;
            }

            //  Now the object is handled with respect to all the pictures
            return new ConstructionData
            (
                // Set the inconstructible object to the given one, if it can't be constructed
                inconstructibleObject: !canBeConstructed ? constructedObject : default,

                // Set the duplicates to the pair of this object and the found duplicate, if there's any
                duplicates: duplicate != null ? (olderObject: duplicate, newerObject: constructedObject) : default
            );
        }

        /// <summary>
        /// Constructs analytic objects having a given layout.
        /// </summary>
        /// <param name="layout">The layout of loose objects.</param>
        /// <returns>The constructed analytic objects.</returns>
        private IAnalyticObject[] Construct(LooseObjectsLayout layout)
        {
            switch (layout)
            {
                // With two points all options are equivalent
                case LooseObjectsLayout.LineSegment:
                    return new IAnalyticObject[] { new Point(0, 0), new Point(1, 0) };

                // With three points we'll create an acute scalene triangle
                case LooseObjectsLayout.Triangle:
                {
                    // Create the points
                    var (point1, point2, point3) = AnalyticHelpers.ConstructRandomScaleneAcuteTriangle();

                    // Return them in an array 
                    return new IAnalyticObject[] { point1, point2, point3 };
                }

                // With four points we'll create a convex quadrilateral
                case LooseObjectsLayout.Quadrilateral:
                {
                    // Create the points
                    var (point1, point2, point3, point4) = AnalyticHelpers.ConstructRandomConvexQuadrilateral();

                    // Return them in an array 
                    return new IAnalyticObject[] { point1, point2, point3, point4 };
                }

                // In this case the line is fixed and the point is arbitrary
                case LooseObjectsLayout.ExplicitLineAndPoint:
                {
                    // Create the points
                    var (point, line) = AnalyticHelpers.ConstructLineAndRandomPointNotLyingOnIt();

                    // Return them in an array 
                    return new IAnalyticObject[] { point, line };
                }

                // In this case the line is fixed and points are arbitrary
                case LooseObjectsLayout.ExplicitLineAndTwoPoints:
                {
                    // Create the points
                    var (line, point1, point2) = AnalyticHelpers.ConstructLineAndTwoRandomPointsNotLyingOnIt();

                    // Return them in an array 
                    return new IAnalyticObject[] { line, point1, point2 };
                }

                // In this case we have three points
                case LooseObjectsLayout.RightTriangle:
                {
                    // Create the points
                    var (point1, point2, point3) = AnalyticHelpers.ConstructRandomRightTriangle();

                    // Return them in an array 
                    return new IAnalyticObject[] { point1, point2, point3 };
                }

                // If we got here, we have an unsupported layout :/
                default:
                    throw new ConstructorException($"Construction of loose objects layout '{layout}' is not supported.");
            }
        }

        #endregion
    }
}