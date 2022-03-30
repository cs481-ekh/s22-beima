using BEIMA.Backend.MongoService;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace BEIMA.Backend.Test.MongoService
{
    [TestFixture]
    public class MongoFilterGeneratorTest
    {
        [TestCase("abc", "ABCdef")]
        [TestCase("def", 123)]
        [TestCase("jjjjjjjjjjjjjjjj", true)]
        public void FilterNotGenerated_GenerateEqualsFilter_FilterGenerated(string key, dynamic value)
        {
            //Create the filter
            FilterDefinition<BsonDocument> filter = MongoFilterGenerator.GetEqualsFilter(key, value);

            //Render the filter as a BsonDocument so we can run tests to make sure the filter is correct
            var serializerRegistry = BsonSerializer.SerializerRegistry;
            var documentSerializer = serializerRegistry.GetSerializer<BsonDocument>();
            var doc = filter.Render(documentSerializer, serializerRegistry);

            //Ensure filter is correct
            Assert.That(filter, Is.Not.Null);
            Assert.That(filter, Is.InstanceOf(typeof(FilterDefinition<BsonDocument>)));
            Assert.That(doc.GetElement(key).Value?.ToString()?.ToLower(), Is.EqualTo(value.ToString().ToLower()));
        }

        [TestCase("nullValue", null)]
        [TestCase("anotherNull12345", null)]
        public void FilterNotGenerated_GenerateEqualsFilterUsingNullValue_FilterGenerated(string key, dynamic value)
        {
            //Create the filter
            FilterDefinition<BsonDocument> filter = MongoFilterGenerator.GetEqualsFilter(key, value);

            //Render the filter as a BsonDocument so we can run tests to make sure the filter is correct
            var serializerRegistry = BsonSerializer.SerializerRegistry;
            var documentSerializer = serializerRegistry.GetSerializer<BsonDocument>();
            var doc = filter.Render(documentSerializer, serializerRegistry);

            //Ensure filter is correct
            Assert.That(filter, Is.Not.Null);
            Assert.That(filter, Is.InstanceOf(typeof(FilterDefinition<BsonDocument>)));
            Assert.That(doc.GetElement(key).Value, Is.EqualTo(BsonNull.Value));
        }

        [TestCase("key1", "value1", "key2", "value2")]
        public void TwoFiltersGenerated_CombineFilters_FiltersCombined(string key1, dynamic value1, string key2, dynamic value2)
        {
            //Create filters
            var f1 = MongoFilterGenerator.GetEqualsFilter(key1, value1);
            var f2 = MongoFilterGenerator.GetEqualsFilter(key2, value2);
            var combinedFilter = MongoFilterGenerator.CombineFilters(f1, f2);

            //Render the filter as a BsonDocument so we can run tests to make sure the filter is correct
            var serializerRegistry = BsonSerializer.SerializerRegistry;
            var documentSerializer = serializerRegistry.GetSerializer<BsonDocument>();
            var doc = combinedFilter.Render(documentSerializer, serializerRegistry);

            //Ensure filter is correct
            Assert.That(combinedFilter, Is.Not.Null);
            Assert.That(combinedFilter, Is.InstanceOf(typeof(FilterDefinition<BsonDocument>)));
            Assert.That(doc.GetElement(key1).Value?.ToString()?.ToLower(), Is.EqualTo(value1.ToString().ToLower()));
            Assert.That(doc.GetElement(key2).Value?.ToString()?.ToLower(), Is.EqualTo(value2.ToString().ToLower()));
        }
    }
}
