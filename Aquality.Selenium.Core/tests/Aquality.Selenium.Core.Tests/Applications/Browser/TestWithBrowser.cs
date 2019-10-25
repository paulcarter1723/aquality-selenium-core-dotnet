﻿using Aquality.Selenium.Core.Applications;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Aquality.Selenium.Core.Tests.Applications.Browser
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public abstract class TestWithBrowser
    {
        protected ServiceProvider ServiceProvider { get; private set; }

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();
            Startup.ConfigureServices(services, serviceCollection => ApplicationManager.Application);
            ServiceProvider = services.BuildServiceProvider();
        }

        [TearDown]
        public void CleanUp()
        {
            if (ApplicationManager.IsApplicationStarted())
            {
                ApplicationManager.Application.Quit();
            }
        }
    }
}
