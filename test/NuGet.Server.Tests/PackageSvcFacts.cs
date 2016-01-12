﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Linq;
using System.ServiceModel.Web;
using Moq;
using NuGet.Server.DataServices;
using Xunit;

namespace NuGet.Server.Tests
{
    public class PackageSvcFacts
    {
        [Fact]
        public void EnsureAllDeclaredServicesAreRegistered()
        {
            // Arrange
            var registeredServices = new List<string>();
            var config = new Mock<IDataServiceConfiguration>(MockBehavior.Strict);
            config.Setup(s => s.SetServiceOperationAccessRule(It.IsAny<string>(), ServiceOperationRights.AllRead))
                  .Callback<string, ServiceOperationRights>((svc, _) => registeredServices.Add(svc));
            var expectedServices = typeof(Packages).GetMethods()
                                                     .Where(m => m.GetCustomAttributes(inherit: false).OfType<WebGetAttribute>().Any())
                                                     .Select(m => m.Name);

            // Act
            Packages.RegisterServices(config.Object);

            // Assert
            Assert.Equal(expectedServices.OrderBy(s => s, StringComparer.Ordinal),
                         registeredServices.OrderBy(s => s, StringComparer.Ordinal));
        }
    }
}
