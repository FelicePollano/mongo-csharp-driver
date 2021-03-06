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

namespace MongoDB.DriverOnlineTests.Jira.CSharp172
{
    [TestFixture]
    public class CSharp172Tests
    {
        public class C
        {
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id;
            public int N;
        }

        private MongoServer server;
        private MongoDatabase database;
        private MongoCollection<C> collection;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            server = MongoServer.Create("mongodb://localhost/?safe=true");
            database = server["onlinetests"];
            collection = database.GetCollection<C>("csharp172");
        }

        [Test]
        public void TestRoundtrip()
        {
            var obj1 = new C { N = 1 };
            Assert.IsNullOrEmpty(obj1.Id);
            collection.RemoveAll();
            collection.Insert(obj1);
            Assert.IsNotNullOrEmpty(obj1.Id);

            var obj2 = collection.FindOne();
            Assert.AreEqual(obj1.Id, obj2.Id);
            Assert.AreEqual(obj1.N, obj2.N);
        }
    }
}
