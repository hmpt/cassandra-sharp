﻿// cassandra-sharp - high performance .NET driver for Apache Cassandra
// Copyright (c) 2011-2018 Pierre Chalamet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Linq;
using CassandraSharp.Config;
using CassandraSharp.CQLPropertyBag;
using NUnit.Framework;

namespace CassandraSharp.UnitTests.Functional
{
    [TestFixture]
    public class NullTest
    {
        [TearDown]
        public void TearDown()
        {
            clusterManager.Dispose();
        }

        private IClusterManager clusterManager;

        [Test]
        public void TestNull()
        {
            var cassandraSharpConfig = new CassandraSharpConfig();
            clusterManager = new ClusterManager(cassandraSharpConfig);

            var clusterConfig = new ClusterConfig
                                {
                                    Endpoints = new EndpointsConfig
                                                {
                                                    Servers = new[] {"cassandra1"}
                                                }
                                };

            using (var cluster = clusterManager.GetCluster(clusterConfig))
            {
                var cmd = cluster.CreatePropertyBagCommand();

                const string dropFoo = "drop keyspace Tests";

                try
                {
                    cmd.Execute(dropFoo).AsFuture().Wait();
                }
                catch
                {
                }

                const string createFoo = "CREATE KEYSPACE Tests WITH replication = {'class': 'SimpleStrategy', 'replication_factor' : 1}";
                Console.WriteLine("============================================================");
                Console.WriteLine(createFoo);
                Console.WriteLine("============================================================");

                cmd.Execute(createFoo).AsFuture().Wait();
                Console.WriteLine();
                Console.WriteLine();

                const string createBar = @"CREATE TABLE Tests.AllTypes (a int, b int, primary key (a))";
                Console.WriteLine("============================================================");
                Console.WriteLine(createBar);
                Console.WriteLine("============================================================");
                cmd.Execute(createBar).AsFuture().Wait();
                Console.WriteLine();
                Console.WriteLine();

                //const string useBar = @"use Tests";
                //Console.WriteLine("============================================================");
                //Console.WriteLine(useBar);
                //Console.WriteLine("============================================================");
                //cmd.Execute(useBar).AsFuture().Wait();
                //Console.WriteLine();
                //Console.WriteLine();

                const string insertBatch = @"insert into Tests.AllTypes (a, b) values (?, ?)";
                var prepared = cmd.Prepare(insertBatch);

                var insertBag = new PropertyBag();
                insertBag["a"] = 1;
                insertBag["b"] = null;

                prepared.Execute(insertBag).AsFuture().Wait();

                const string selectAll = "select * from Tests.AllTypes";
                var res = cmd.Execute(selectAll).AsFuture();
                Assert.IsTrue(1 == res.Result.Count);
                var selectBag = res.Result.Single();
                Assert.IsTrue(selectBag.Keys.Contains("a"));
                Assert.IsTrue(1 == (int)selectBag["a"]);
                Assert.IsTrue(null == selectBag["b"]);
            }
        }
    }
}