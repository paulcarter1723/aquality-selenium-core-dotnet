﻿using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Tests.Applications.WindowsApp.Elements;
using Aquality.Selenium.Core.Tests.Applications.WindowsApp.Locators;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenQA.Selenium;
using System;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp
{
    public class ElementFactoryTests : TestWithApplication
    {
        private IElementFactory Factory => AqualityServices.ServiceProvider.GetRequiredService<IElementFactory>();

        private IElement NumberPad => Factory.GetButton(CalculatorWindow.WindowLocator, "Number pad");

        [Test]
        public void Should_WorkWithCalculator_ViaElementFactory()
        {
            Factory.GetButton(CalculatorWindow.OneButton, "1").Click();
            Factory.GetButton(CalculatorWindow.PlusButton, "+").Click();
            Factory.GetButton(CalculatorWindow.TwoButton, "2").Click();
            Factory.GetButton(CalculatorWindow.EqualsButton, "=").Click();
            var result = Factory.GetButton(CalculatorWindow.ResultsLabel, "Results bar").Text;
            StringAssert.Contains("3", result);
        }

        [Test]
        public void Should_FindChildElements_ViaElementFactory()
        {
            Assert.IsNotNull(Factory.FindChildButton(NumberPad, CalculatorWindow.OneButton).GetElement(TimeSpan.Zero));
        }

        [Test]
        public void Should_FindElements_ViaElementFactory()
        {
            Assert.IsTrue(Factory.FindButtons(By.XPath("//*")).Count > 1);
        }

        [Test]
        public void Should_FindElements_WithCustomName_ViaElementFactory()
        {
            const string name = "Custom Name";
            var buttons = Factory.FindButtons(By.XPath("//*"), name);
            Assert.Multiple(() =>
            {
                Assert.IsTrue(buttons.Count > 1);
                for (var i = 0; i < buttons.Count; i++)
                {
                    var button = buttons[i];
                    StringAssert.StartsWith(name, button.Name);
                    StringAssert.EndsWith((i + 1).ToString(), button.Name);
                }
            });
        }

        [Test]
        public void Should_FindElements_WithDefaultName_ViaElementFactory()
        {
            var buttons = Factory.FindButtons(By.XPath("//*"));
            Assert.Multiple(() =>
            {
                Assert.IsTrue(buttons.Count > 1);
                for (var i = 0; i < buttons.Count; i++)
                {
                    var button = buttons[i];
                    var endOfName = (i + 1).ToString();
                    StringAssert.AreNotEqualIgnoringCase(endOfName, button.Name);
                    StringAssert.EndsWith(endOfName, button.Name);
                }
            });
        }

        [Test]
        public void Should_ThrowInvalidOperationException_WhenConstructorIsNotDefined_ForFindChildElement()
        {
            Assert.Throws<InvalidOperationException>(() => Factory.FindChildElement<Button>(NumberPad, CalculatorWindow.OneButton).GetElement());
        }

        [Test]
        public void Should_ThrowInvalidOperationException_WhenConstructorIsNotDefined_ForFindElements()
        {
            Assert.Throws<InvalidOperationException>(() => Factory.FindElements<Button>(CalculatorWindow.OneButton));
        }
    }
}
