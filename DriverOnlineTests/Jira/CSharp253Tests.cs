﻿/* Copyright 2010-2011 10gen Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using NUnit.Framework;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace MongoDB.DriverOnlineTests.Jira.CSharp253
{
    [TestFixture]
    public class CSharp253Tests
    {
        public class C
        {
            public ObjectId Id { get; set; }
            public MongoDBRef DBRef { get; set; }
            public BsonNull BsonNull { get; set; }
        }

        private MongoServer server;
        private MongoDatabase database;
        private MongoCollection<BsonDocument> collection;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            server = MongoServer.Create("mongodb://localhost/?safe=true");
            database = server["onlinetests"];
            collection = database.GetCollection("testcollection");
        }

        [Test]
        public void TestInsertClass()
        {
            var c = new C
            {
                DBRef = new MongoDBRef("database", "collection", ObjectId.GenerateNewId()),
                BsonNull = null
            };
            collection.Insert(c);
        }

        [Test]
        public void TestInsertDollar()
        {
            Assert.Throws<BsonSerializationException>(() => { collection.Insert(new BsonDocument("$x", 1)); });
            Assert.Throws<BsonSerializationException>(() => { collection.Insert(new BsonDocument("x", new BsonDocument("$x", 1))); });
        }

        [Test]
        public void TestInsertPeriod()
        {
            Assert.Throws<BsonSerializationException>(() => { collection.Insert(new BsonDocument("a.b", 1)); });
            Assert.Throws<BsonSerializationException>(() => { collection.Insert(new BsonDocument("a", new BsonDocument("b.c", 1))); });
        }

        [Test]
        public void TestLegacyDollar()
        {
            var document = new BsonDocument
            {
                { "_id", ObjectId.GenerateNewId() },
                { "BsonNull", new BsonDocument("$csharpnull", true) },
                { "Code", new BsonDocument
                    {
                        { "$code", "code" },
                        { "$scope", "scope" }
                    }
                },
                { "DBRef", new BsonDocument
                    {
                        { "$db", "db" },
                        { "$id", "id" },
                        { "$ref", "ref" }
                    }
                }
            };
            collection.Insert(document);
        }

        [Test]
        public void TestCreateIndexOnNestedElement()
        {
            collection.CreateIndex("a.b");
        }
    }
}
