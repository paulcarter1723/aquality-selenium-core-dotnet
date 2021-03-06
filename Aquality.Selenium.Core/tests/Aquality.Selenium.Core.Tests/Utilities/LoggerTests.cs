﻿using Aquality.Selenium.Core.Logging;
using NLog.Targets;
using NUnit.Framework;
using System;
using System.IO;

namespace Aquality.Selenium.Core.Tests.Utilities
{
    [NonParallelizable]
    public class LoggerTests
    {
        private const string AddTargetLogFile = "AddTargetTestLog.log";
        private const string RemoveTargetLogFile = "RemoveTargetTestLog.log";
        private const string TestMessage = "test message";

        [SetUp]
        public void Setup()
        {
            File.Delete(AddTargetLogFile);
            File.Delete(RemoveTargetLogFile);
        }

        [Test]
        public void Should_BePossibleTo_AddTarget()
        {
            Logger.Instance.AddTarget(GetTarget(AddTargetLogFile)).Info(TestMessage);
            Assert.True(File.Exists(AddTargetLogFile),
                $"Target wasn't added. File '{AddTargetLogFile}' doesn't exist.");
            var log = File.ReadAllText(AddTargetLogFile).Trim();
            Assert.True(log.Equals(TestMessage),
                $"Target wasn't added. File doesn't contain message: '{TestMessage}'.");
        }

        [Test]
        public void Should_BePossibleTo_RemoveTarget()
        {
            var target = GetTarget(RemoveTargetLogFile);
            Logger.Instance.AddTarget(target).RemoveTarget(target).Info(TestMessage);
            Assert.False(File.Exists(RemoveTargetLogFile),
                $"Target wasn't removed. File '{RemoveTargetLogFile}' exists.");
        }

        private static Target GetTarget(string filePath)
        {
            return new FileTarget
            {
                Name = Guid.NewGuid().ToString(),
                FileName = filePath,
                Layout = "${message}"
            };
        }
    }
}
