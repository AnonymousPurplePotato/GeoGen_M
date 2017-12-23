﻿using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities;
using GeoGen.Generator.ConstructingConfigurations;
using GeoGen.Generator.ConstructingConfigurations.ConfigurationToString;
using GeoGen.Generator.ConstructingConfigurations.IdsFixing;
using GeoGen.Generator.ConstructingConfigurations.LeastConfigurationFinding;
using GeoGen.Generator.ConstructingConfigurations.ObjectsContainer;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;
using GeoGen.Generator.ConstructingObjects;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsListToString;
using GeoGen.Generator.ConstructingObjects.Arguments.Container;
using GeoGen.Utilities;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.ConfigurationObjects;
using static GeoGen.Generator.Test.TestHelpers.Configurations;
using static GeoGen.Generator.Test.TestHelpers.ToStringHelper;

namespace GeoGen.Generator.Test.ConstructingConfigurations
{
    [TestFixture]
    public class ConfigurationConstructorTest
    {
        private static ConfigurationObjectsContainer _container;

        private static IArgumentsListContainerFactory _argsContainerFactory;

        private static ConfigurationConstructor Constructor(IReadOnlyList<LooseConfigurationObject> objects)
        {
            var resolver = new DefaultObjectIdResolver();
            var provider = new DefaultObjectToStringProvider(resolver);
            var argsToString = new ArgumentsListToStringProvider(provider);
            var fullProvider = new DefaultFullObjectToStringProvider(argsToString, resolver);
            _container = new ConfigurationObjectsContainer(null, fullProvider);
            //_container.Initialize(AsConfiguration(objects));
            var variations = new VariationsProvider();
            var configurationToString = new ConfigurationToStringProvider();
            var objectToStringFactory = new CustomFullObjectToStringProviderFactory(argsToString);
            var dictionaryObjectIdResolversContainer = new DictionaryObjectIdResolversContainer(null, variations);
            var finder = new LeastConfigurationFinder(configurationToString, objectToStringFactory, dictionaryObjectIdResolversContainer);
            //_argsContainerFactory = new ArgumentsListContainerFactory(argsToString);
            var fixer = new IdsFixerFactory(_container);

            return new ConfigurationConstructor(finder, fixer);
        }

        [Test]
        public void Test_Passed_Constructor_Output_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => Constructor(Objects(1, ConfigurationObjectType.Point)).ConstructWrapper((ConstructorOutput) null)
            );
        }

        [Test]
        public void Test_Passed_Initial_Configuration_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => Constructor(Objects(1, ConfigurationObjectType.Point)).ConstructWrapper((Configuration) null)
            );
        }

        [Test]
        public void Test_Triangle_With_One_Midpoint()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point)
            };

            var handler = Constructor(looseObjects);

            List<ConstructionArgument> Args(ConfigurationObject o1, ConfigurationObject o2)
            {
                var result = new List<ConstructionArgument>
                {
                    new SetConstructionArgument
                    (
                        new HashSet<ConstructionArgument>
                        {
                            new ObjectConstructionArgument(o1),
                            new ObjectConstructionArgument(o2)
                        }
                    )
                };

                return result;
            }

            var looseSet = new HashSet<LooseConfigurationObject>(looseObjects);
            var configuration = new Configuration(looseSet, new List<ConstructedConfigurationObject>());

            var testCases = new List<List<ConstructionArgument>>
                    {
                        Args(looseObjects[0], looseObjects[1]),
                        Args(looseObjects[0], looseObjects[2]),
                        Args(looseObjects[1], looseObjects[2])
                    }
                    .Select
                    (
                        list =>
                        {
                            var obj = ConstructedObject(42, 0, list);
                            obj = _container.Add(obj);
                            var objAsList = new List<ConstructedConfigurationObject> {obj};
                            var objectsMap = new ConfigurationObjectsMap(looseObjects);

                            var wrapper = new ConfigurationWrapper
                            {
                                Configuration = configuration,
                                AllObjectsMap = objectsMap,
                                OriginalObjects = looseObjects.Cast<ConfigurationObject>().ToList()
                            };

                            return new ConstructorOutput
                            {
                                InitialConfiguration = wrapper,
                                ConstructedObjects = objAsList
                            };
                        }
                    );

            foreach (var testCase in testCases)
            {
                var result = handler.ConstructWrapper(testCase);

                Assert.IsFalse(result.Excluded);
                Assert.NotNull(result.Configuration);

                var asString = ConfigurationAsString(result.Configuration);
                Assert.AreEqual("42({1;2})", asString);

                var allObjects = result.AllObjectsMap;
                Assert.AreEqual(1, allObjects.Count);
                Assert.AreEqual(4, allObjects[ConfigurationObjectType.Point].Count);

                var originalObjects = result.OriginalObjects;
                Assert.AreEqual(3, originalObjects.Count);
                Assert.IsTrue(originalObjects.All(o => o is LooseConfigurationObject));

                var constructedObjects = result.LastAddedObjects;
                Assert.AreEqual(1, constructedObjects.Count);
            }
        }

        [Test]
        public void Test_Triangle_With_Two_Midpoints()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point)
            };

            var handler = Constructor(looseObjects);

            List<ConstructionArgument> Args(ConfigurationObject o1, ConfigurationObject o2)
            {
                var result = new List<ConstructionArgument>
                {
                    new SetConstructionArgument
                    (
                        new HashSet<ConstructionArgument>
                        {
                            new ObjectConstructionArgument(o1),
                            new ObjectConstructionArgument(o2)
                        }
                    )
                };

                return result;
            }

            var looseSet = new HashSet<LooseConfigurationObject>(looseObjects);
            var configuration = new Configuration(looseSet, new List<ConstructedConfigurationObject>());

            var objects = new List<List<ConstructionArgument>>
                    {
                        Args(looseObjects[0], looseObjects[1]),
                        Args(looseObjects[0], looseObjects[2]),
                        Args(looseObjects[1], looseObjects[2])
                    }
                    .Select(arg => _container.Add(ConstructedObject(42, 0, arg)))
                    .ToList();


            var testCases = new List<List<ConstructedConfigurationObject>>
                    {
                        new List<ConstructedConfigurationObject> {objects[0], objects[1]},
                        new List<ConstructedConfigurationObject> {objects[0], objects[2]},
                        new List<ConstructedConfigurationObject> {objects[1], objects[2]}
                    }
                    .Select
                    (
                        list =>
                        {
                            var objectsMap = new ConfigurationObjectsMap(looseObjects);

                            var wrapper = new ConfigurationWrapper
                            {
                                Configuration = configuration,
                                AllObjectsMap = objectsMap,
                                OriginalObjects = looseObjects.Cast<ConfigurationObject>().ToList()
                            };

                            return new ConstructorOutput
                            {
                                ConstructedObjects = list,
                                InitialConfiguration = wrapper
                            };
                        }
                    );

            foreach (var testCase in testCases)
            {
                var result = handler.ConstructWrapper(testCase);

                Assert.IsFalse(result.Excluded);
                Assert.NotNull(result.Configuration);

                var asString = ConfigurationAsString(result.Configuration);
                Assert.AreEqual("42({1;2})|42({1;3})", asString);

                var allObjects = result.AllObjectsMap;
                Assert.AreEqual(1, allObjects.Count);
                Assert.AreEqual(5, allObjects[ConfigurationObjectType.Point].Count);

                var originalObjects = result.OriginalObjects;
                Assert.AreEqual(3, originalObjects.Count);
                Assert.IsTrue(originalObjects.All(o => o is LooseConfigurationObject));

                var constructedObjects = result.LastAddedObjects;
                Assert.AreEqual(2, constructedObjects.Count);
            }
        }

        [Test]
        public void Test_Objects_Map_Is_Fixed_Correctly()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Line),
                new LooseConfigurationObject(ConfigurationObjectType.Line),
                new LooseConfigurationObject(ConfigurationObjectType.Circle),
                new LooseConfigurationObject(ConfigurationObjectType.Circle)
            };
            var handler = Constructor(looseObjects);

            var looseSet = new HashSet<LooseConfigurationObject>(looseObjects);
            var configuration = new Configuration(looseSet, new List<ConstructedConfigurationObject>());

            var list = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(looseObjects[2]),
                new ObjectConstructionArgument(looseObjects[1]),
                new ObjectConstructionArgument(looseObjects[0])
            };

            var obj = ConstructedObject(42, 0, list);
            obj = _container.Add(obj);
            var objAsList = new List<ConstructedConfigurationObject> {obj};
            var objectsMap = new ConfigurationObjectsMap(looseObjects);

            var forbidden = _argsContainerFactory.CreateContainer();
            var forbiddenArgs1 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(looseObjects[1]),
                new ObjectConstructionArgument(looseObjects[2]),
                new ObjectConstructionArgument(looseObjects[0])
            };
            var forbiddenArgs2 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(looseObjects[2]),
                new ObjectConstructionArgument(looseObjects[0]),
                new ObjectConstructionArgument(looseObjects[1])
            };
            forbidden.AddArguments(forbiddenArgs1);
            forbidden.AddArguments(forbiddenArgs2);

            var wrapper = new ConfigurationWrapper
            {
                Configuration = configuration,
                AllObjectsMap = objectsMap
            };

            var output = new ConstructorOutput
            {
                ConstructedObjects = objAsList,
                InitialConfiguration = wrapper
            };

            var result = handler.ConstructWrapper(output);
            var asString = ConfigurationAsString(result.Configuration);

            Assert.AreEqual("42(1,2,3)", asString);

            var dictionary2 = result.AllObjectsMap;
            Assert.AreEqual(3, dictionary2.Count);
            Assert.AreEqual(4, dictionary2[ConfigurationObjectType.Point].Count);
            Assert.AreEqual(2, dictionary2[ConfigurationObjectType.Line].Count);
            Assert.AreEqual(2, dictionary2[ConfigurationObjectType.Circle].Count);

            Assert.IsFalse(dictionary2[ConfigurationObjectType.Point].Contains(obj));
        }

        [Test]
        public void Test_Complex_Triangle_Configuration()
        {
            var looseObjects = new List<LooseConfigurationObject>
            {
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point),
                new LooseConfigurationObject(ConfigurationObjectType.Point)
            };
            var handler = Constructor(looseObjects);

            var looseSet = new HashSet<LooseConfigurationObject>(looseObjects);
            var configuration = new Configuration(looseSet, new List<ConstructedConfigurationObject>());

            ConstructedConfigurationObject Obj(List<ConstructionArgument> args)
            {
                return ConstructedObject(42, 0, args);
            }

            var list1 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(looseObjects[1]),
                new ObjectConstructionArgument(looseObjects[0]),
                new ObjectConstructionArgument(looseObjects[2])
            };

            var obj1 = Obj(list1);
            obj1 = _container.Add(obj1);

            var list2 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(looseObjects[1]),
                new ObjectConstructionArgument(looseObjects[2]),
                new ObjectConstructionArgument(looseObjects[0])
            };

            var obj2 = Obj(list2);
            obj2 = _container.Add(obj2);

            var list3 = new List<ConstructionArgument>
            {
                new SetConstructionArgument
                (
                    new HashSet<ConstructionArgument>
                    {
                        new ObjectConstructionArgument(looseObjects[2]),
                        new ObjectConstructionArgument(obj1)
                    }
                )
            };

            var obj3 = Obj(list3);
            obj3 = _container.Add(obj3);

            var list4 = new List<ConstructionArgument>
            {
                new SetConstructionArgument
                (
                    new HashSet<ConstructionArgument>
                    {
                        new ObjectConstructionArgument(looseObjects[0]),
                        new ObjectConstructionArgument(obj2)
                    }
                )
            };

            var obj4 = Obj(list4);
            obj4 = _container.Add(obj4);

            var objAsList = new List<ConstructedConfigurationObject> {obj1, obj2, obj3, obj4};
            var objectsMap = new ConfigurationObjectsMap(looseObjects);

            var wrapper = new ConfigurationWrapper
            {
                Configuration = configuration,
                AllObjectsMap = objectsMap
            };

            var output = new ConstructorOutput
            {
                InitialConfiguration = wrapper,
                ConstructedObjects = objAsList
            };

            var result = handler.ConstructWrapper(output);
            var asString1 = ConfigurationAsString(result.Configuration);

            looseObjects[0].Id = 3;
            looseObjects[2].Id = 1;

            var asString2 = ConfigurationAsString(result.Configuration);

            Assert.AreEqual(asString1, asString2);
            Console.WriteLine(asString1);
        }

        [Test]
        public void Test_Constructor_With_Initial_Configuraton()
        {
            var objects = Objects(3, ConfigurationObjectType.Point);
            var constructor = Constructor(objects);

            var args1 = new List<ConstructionArgument>
            {
                new ObjectConstructionArgument(objects[0]),
                new ObjectConstructionArgument(objects[1])
            };
            var constructed1 = ConstructedObject(42, 1, args1);
            constructed1 = _container.Add(constructed1);

            var args2 = new List<ConstructionArgument>
            {
                new SetConstructionArgument
                (
                    new HashSet<ConstructionArgument>
                    {
                        new ObjectConstructionArgument(constructed1),
                        new ObjectConstructionArgument(objects[1])
                    }
                )
            };
            var constructed2 = ConstructedObject(43, 0, args2);
            constructed2 = _container.Add(constructed2);

            var constructed = new List<ConstructedConfigurationObject> {constructed1, constructed2};
            var configuration = new Configuration(objects.ToSet(), constructed);
            var wrapper = constructor.ConstructWrapper(configuration);

            Assert.AreSame(wrapper.Configuration, configuration);

            var map = wrapper.AllObjectsMap;
            Assert.AreEqual(1, map.Count);
            var allObjects = objects.Cast<ConfigurationObject>().Concat(constructed);

            Assert.IsTrue(allObjects.All(o => map[ConfigurationObjectType.Point].Contains(o)));
            Assert.AreEqual(5, map[ConfigurationObjectType.Point].Count);

            Assert.IsFalse(wrapper.Excluded);

            var originalObjects = wrapper.OriginalObjects;

            Assert.AreEqual(5, originalObjects.Count);
            Assert.IsEmpty(wrapper.LastAddedObjects);
        }
    }
}