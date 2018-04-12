﻿using System;
using AutoFixture;

namespace AutoFixtureUnitTest
{
    internal class DelegatingCustomization : ICustomization
    {
        public DelegatingCustomization()
        {
            this.OnCustomize = f => { };
        }

        public void Customize(IFixture fixture)
        {
            this.OnCustomize(fixture);
        }

        internal Action<IFixture> OnCustomize { get; set; }
    }
}
