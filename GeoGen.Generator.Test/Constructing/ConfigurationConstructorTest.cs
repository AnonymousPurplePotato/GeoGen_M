﻿using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationHandling.ConfigurationsContainer;
using GeoGen.Generator.Constructing;
using GeoGen.Generator.Constructing.Arguments;
using GeoGen.Generator.Constructing.Arguments.Container;
using GeoGen.Generator.Constructing.Container;
using Moq;
using NUnit.Framework;

namespace GeoGen.Generator.Test.Constructing
{
    public class ConfigurationConstructorTest
    {
        private static T SimpleMock<T>() where T : class
        {
            return new Mock<T>().Object;
        }

        private static List<ConstructionWrapper> ConstructionWrappers(int numberOfConstruction, int outputCount)
        {
            return Enumerable.Range(1, numberOfConstruction)
                    .Select(
                        i =>
                        {
                            var mock = new Mock<Construction>();
                            var outputTypes = Enumerable.Repeat(ConfigurationObjectType.Point, outputCount).ToList();
                            mock.Setup(c => c.OutputTypes).Returns(outputTypes);
                            mock.Setup(c => c.Id).Returns(i);

                            return mock.Object;
                        })
                    .Select(c => new ConstructionWrapper {Construction = c})
                    .ToList();
        }

        private static IEnumerable<ConfigurationObject> Objects(int count)
        {
            return Enumerable.Range(1, count)
                    .Select(i => new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = i})
                    .ToList();
        }

        private static IArgumentsContainer Arguments(IEnumerable<ConfigurationObject> objects)
        {
            var argsList = objects
                    .Select(obj => new List<ConstructionArgument> {new ObjectConstructionArgument(obj)})
                    .Cast<IReadOnlyList<ConstructionArgument>>()
                    .ToList();

            var mock = new Mock<IArgumentsContainer>();
            mock.Setup(s => s.GetEnumerator()).Returns(() => argsList.GetEnumerator());

            mock.Setup(s => s.RemoveElementsFrom(It.IsAny<IArgumentsContainer>()))
                    .Callback<IArgumentsContainer>(
                        c =>
                        {
                            int Id(IReadOnlyList<ConstructionArgument> arguments)
                            {
                                return ((ObjectConstructionArgument) arguments[0]).PassedObject.Id.Value;
                            }

                            foreach (var argsToRemove in c)
                            {
                                argsList.RemoveAll(args => Id(args) == Id(argsToRemove));
                            }
                        });

            return mock.Object;
        }

        private static IConstructionsContainer ConstructionsContainer(int count, int outputCount)
        {
            var constructions = ConstructionWrappers(count, outputCount);
            var mock = new Mock<IConstructionsContainer>();
            mock.Setup(c => c.GetEnumerator()).Returns(constructions.GetEnumerator());

            return mock.Object;
        }

        private static IArgumentsGenerator ArgumentsGenerator(List<ConfigurationObject> objects)
        {
            var generatorMock = new Mock<IArgumentsGenerator>();
            generatorMock.Setup(g => g.GenerateArguments(It.IsAny<ConfigurationWrapper>(), It.IsAny<ConstructionWrapper>()))
                    .Returns(() => Arguments(objects));

            return generatorMock.Object;
        }

        private static IArgumentsContainer Container(IEnumerable<ConfigurationObject> objects, int forbidenIdStart, int forbidenIdEnd)
        {
            var mock = new Mock<IArgumentsContainer>();

            mock.Setup(c => c.GetEnumerator()).Returns(
                () => objects.Where
                        (
                            o =>
                            {
                                var id = o.Id;

                                return id >= forbidenIdStart && id <= forbidenIdEnd;
                            }
                        )
                        .Select(o => new List<ConstructionArgument> {new ObjectConstructionArgument(o)})
                        .GetEnumerator());

            return mock.Object;
        }

        private static ConfigurationWrapper ConfigurationWrapper(List<ConfigurationObject> objects,
            int forbiddenConstructionsStartId, int forbiddenConstructionEndId, int forbiddenArgumentsStartId, int forbiddenArgumentsEndId)
        {
            var forbiddenArgs = new Dictionary<int, IArgumentsContainer>();

            for (var constructionId = forbiddenConstructionsStartId; constructionId <= forbiddenConstructionEndId; constructionId++)
            {
                forbiddenArgs.Add(constructionId, Container(objects, forbiddenArgumentsStartId, forbiddenArgumentsEndId));
            }

            return new ConfigurationWrapper {ConstructionIdToForbiddenArguments = forbiddenArgs};
        }

        private static int ExecuteConstructionProcces(int numberOfConstructions, int argumentsPerConstruction,
            int forbiddenConstructionsStartId, int forbiddenConstructionEndId, int constructionOutputCount,
            int forbiddenArgumentsStartId, int forbiddenArgumentsEndId)
        {
            var objects = Objects(argumentsPerConstruction).ToList();
            var container = ConstructionsContainer(numberOfConstructions, constructionOutputCount);
            var argumentsGenerator = ArgumentsGenerator(objects);
            var wrapper = ConfigurationWrapper(objects, forbiddenConstructionsStartId, forbiddenConstructionEndId, forbiddenArgumentsStartId, forbiddenArgumentsEndId);
            var testConstructor = new ConfigurationConstructor(container, argumentsGenerator);

            return testConstructor.GenerateNewConfigurationObjects(wrapper).Sum(output => output.ConstructedObjects.Count);
        }

        [Test]
        public void Test_Constructor_Constructions_Container_Null()
        {
            Assert.Throws<ArgumentNullException>(() => { new ConfigurationConstructor(null, SimpleMock<IArgumentsGenerator>()); });
        }

        [Test]
        public void Test_Constructor_Arguments_Generator_Null()
        {
            Assert.Throws<ArgumentNullException>(() => { new ConfigurationConstructor(SimpleMock<IConstructionsContainer>(), null); });
        }

        [Test]
        public void Test_Configuration_Wrapper_Not_Null()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    new ConfigurationConstructor(SimpleMock<IConstructionsContainer>(), SimpleMock<IArgumentsGenerator>())
                            .GenerateNewConfigurationObjects(null);
                });
        }

        [TestCase(1, 2, 3, 6)]
        [TestCase(2, 7, 4, 56)]
        [TestCase(8, 1, 3, 24)]
        [TestCase(42, 4, 666, 111888)]
        [TestCase(42, 4, 1, 168)]
        [TestCase(0, 0, 666, 0)]
        [TestCase(0, 1, 666, 0)]
        [TestCase(1, 0, 1, 0)]
        public void Test_Without_Anything_Forbidden(int constructions, int arguments, int perConstruction, int expected)
        {
            var count = ExecuteConstructionProcces(constructions, arguments, 0, 0, perConstruction, 0, 0);

            Assert.AreEqual(expected, count);
        }

        [TestCase(10, 20, 1, 1, 190)]
        [TestCase(10, 20, 1, 3, 170)]
        [TestCase(10, 20, 2, 20, 10)]
        public void Test_With_Arguments_Forbidden_For_All_Constructions_And_One_Output_Per_Construction(
            int constructions, int arguments, int forbiddenStart, int forbiddenEnd, int expected)
        {
            var count = ExecuteConstructionProcces(constructions, arguments, 1, constructions, 1, forbiddenStart, forbiddenEnd);

            Assert.AreEqual(expected, count);
        }

        [TestCase(10, 20, 1, 1, 380)]
        [TestCase(10, 20, 1, 3, 340)]
        [TestCase(10, 20, 2, 20, 20)]
        public void Test_With_Arguments_Forbidden_For_All_Constructions_And_Two_Outputs_Per_Construction(
            int constructions, int arguments, int forbiddenStart, int forbiddenEnd, int expected)
        {
            var count = ExecuteConstructionProcces(constructions, arguments, 1, constructions, 2, forbiddenStart, forbiddenEnd);

            Assert.AreEqual(expected, count);
        }

        [TestCase(42, 42, 1, 1676)]
        [TestCase(42, 42, 3, 5028)]
        [TestCase(45, 41, 1, 1757)]
        [TestCase(42, 666, 1, 27884)]
        [TestCase(666, 42, 2, 55768)]
        [TestCase(666, 666, 1, 443468)]
        public void Test_With_Arguments_Forbidden_For_Few_Constructions(int constructions, int arguments,
            int perConstruction, int expected)
        {
            var count = ExecuteConstructionProcces(constructions, arguments, 10, 20, perConstruction, 10, 17);

            Assert.AreEqual(expected, count);
        }
    }
}