﻿using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Generator;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;
using NUnit.Framework;
using static GeoGen.Generator.Test.TestHelpers.ConfigurationObjects;

namespace GeoGen.Generator.Test.ConstructingConfigurations.ObjectToString.ObjectIdResolving
{
    [TestFixture]
    public class DictionaryObjectIdResolverTest
    {
        private static DictionaryObjectIdResolver Resolver()
        {
            var dictionary = Enumerable.Range(0, 42).ToDictionary(i => i, i => i * i);

            return new DictionaryObjectIdResolver(1, dictionary);
        }

        [Test]
        public void Test_Passed_Dictionary_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new DictionaryObjectIdResolver(1, null));
        }

        [Test]
        public void Test_Passed_Object_Cant_Be_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Resolver().ResolveId(null));
        }

        [Test]
        public void Test_Passed_Object_Must_Have_Id()
        {
            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point);

            Assert.Throws<GeneratorException>(() => Resolver().ResolveId(obj));
        }

        [Test]
        public void Test_Objects_Id_Isnt_Present_In_Dictionary()
        {
            var obj = new LooseConfigurationObject(ConfigurationObjectType.Point) {Id = 42};

            Assert.Throws<KeyNotFoundException>(() => Resolver().ResolveId(obj));
        }

        [Test]
        public void Test_Objects_Id_Is_Fine()
        {
            var objs = Objects(42, ConfigurationObjectType.Point, 0);

            var resolver = Resolver();

            foreach (var looseConfigurationObject in objs)
            {
                var id = resolver.ResolveId(looseConfigurationObject);
                var realId = looseConfigurationObject.Id ?? throw new Exception();
                Assert.AreEqual(realId * realId, id);
            }
        }

        [Test]
        public void Test_Id_Of_Resolver_Is_Set()
        {
            var id = Resolver().Id;

            Assert.AreEqual(1, id);
        }

        [Test]
        public void Test_Id_Of_Resolver_Cant_Be_Default()
        {
            var id = DefaultObjectIdResolver.DefaultId;
            var dictionary = new Dictionary<int, int>();

            Assert.Throws<ArgumentException>(() => new DictionaryObjectIdResolver(id, dictionary));
        }
    }
}