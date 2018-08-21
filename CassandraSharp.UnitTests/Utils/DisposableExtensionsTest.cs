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
using CassandraSharp.Utils;
using Moq;
using NUnit.Framework;

namespace CassandraSharp.UnitTests.Utils
{
    [TestFixture]
    public class DisposableExtensionsTest
    {
        [Test]
        public void TestException()
        {
            var mock = new Mock<IDisposable>();
            mock.Setup(x => x.Dispose()).Throws<ArithmeticException>();

            var disposable = mock.Object;
            disposable.SafeDispose();

            mock.Verify(x => x.Dispose(), Times.Once());
        }

        [Test]
        public void TestNormal()
        {
            var mock = new Mock<IDisposable>();
            mock.Setup(x => x.Dispose());

            var disposable = mock.Object;
            disposable.SafeDispose();

            mock.Verify(x => x.Dispose(), Times.Once());
        }

        [Test]
        public void TestNull()
        {
            IDisposable disposable = null;
            disposable.SafeDispose();
        }
    }
}