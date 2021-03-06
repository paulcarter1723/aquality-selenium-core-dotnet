﻿using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Waitings;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace Aquality.Selenium.Core.Elements
{
    public class ElementStateProvider : IElementStateProvider
    {
        private readonly By elementLocator;

        public ElementStateProvider(By elementLocator, IConditionalWait conditionalWait, IElementFinder elementFinder)
        {
            this.elementLocator = elementLocator;
            ConditionalWait = conditionalWait;
            ElementFinder = elementFinder;
        }

        private IConditionalWait ConditionalWait { get; }

        private IElementFinder ElementFinder { get; }

        public bool IsDisplayed => WaitForDisplayed(TimeSpan.Zero);

        public bool IsExist => WaitForExist(TimeSpan.Zero);

        public bool IsEnabled => WaitForEnabled(TimeSpan.Zero);

        public bool IsClickable => IsElementClickable(TimeSpan.Zero, true);

        public bool WaitForDisplayed(TimeSpan? timeout = null)
        {
            return IsAnyElementFound(timeout, ElementState.Displayed);
        }

        public bool WaitForNotDisplayed(TimeSpan? timeout = null)
        {
            return ConditionalWait.WaitFor(() => !IsDisplayed, timeout);
        }

        public bool WaitForExist(TimeSpan? timeout = null)
        {
            return IsAnyElementFound(timeout, ElementState.ExistsInAnyState);
        }

        public bool WaitForNotExist(TimeSpan? timeout = null)
        {
            return ConditionalWait.WaitFor(() => !IsExist, timeout);
        }

        private bool IsAnyElementFound(TimeSpan? timeout, ElementState state)
        {
            return ElementFinder.FindElements(elementLocator, state, timeout).Any();
        }

        public bool WaitForEnabled(TimeSpan? timeout = null)
        {
            return IsElementInDesiredState(element => IsElementEnabled(element), "ENABLED", timeout);
        }

        public bool WaitForNotEnabled(TimeSpan? timeout = null)
        {
            return IsElementInDesiredState(element => !IsElementEnabled(element), "NOT ENABLED", timeout);
        }

        protected virtual bool IsElementEnabled(IWebElement element)
        {
            return element.Enabled;
        }

        private bool IsElementInDesiredState(Func<IWebElement, bool> elementStateCondition, string state, TimeSpan? timeout)
        {
            var desiredState = new DesiredState(elementStateCondition, state)
            {
                IsCatchingTimeoutException = true,
                IsThrowingNoSuchElementException = true
            };
            return IsElementInDesiredCondition(timeout, desiredState);
        }

        public void WaitForClickable(TimeSpan? timeout = null)
        {
            IsElementClickable(timeout, false);
        }

        private bool IsElementClickable(TimeSpan? timeout, bool catchTimeoutException)
        {
            var desiredState = new DesiredState(element => element.Displayed && element.Enabled, "CLICKABLE")
            {
                IsCatchingTimeoutException = catchTimeoutException
            };
            return IsElementInDesiredCondition(timeout, desiredState);
        }

        private bool IsElementInDesiredCondition(TimeSpan? timeout, DesiredState elementStateCondition)
        {
            return ElementFinder.FindElements(elementLocator, elementStateCondition, timeout).Any();
        }
    }
}
