﻿using System;
using System.Linq;
using System.Reflection;
using AutoFixture.Kernel;
using TestTypeFoundation;
using Xunit;

namespace AutoFixtureUnitTest.Kernel
{
    public class OmitArrayParameterRequestRelayTests
    {
        [Fact]
        public void SutIsSpecimenBuilder()
        {
            var sut = new OmitArrayParameterRequestRelay();
            Assert.IsAssignableFrom<ISpecimenBuilder>(sut);
        }

        [Theory]
        [InlineData(typeof(object[]))]
        [InlineData(typeof(string[]))]
        [InlineData(typeof(int[]))]
        [InlineData(typeof(Version[]))]
        [InlineData(typeof(SingleParameterType<string>[]))]
        public void CreateWithEnumerableParameterReturnsCorrectResult(
            Type argumentType)
        {
            var parameterInfo =
                typeof(SingleParameterType<>)
                    .MakeGenericType(new[] { argumentType })
                    .GetConstructors()
                    .First()
                    .GetParameters()
                    .First();
            var expected = new object();
            var context = new DelegatingSpecimenContext
            {
                OnResolve = r =>
                {
                    Assert.Equal(
                        new SeededRequest(
                            parameterInfo.ParameterType,
                            parameterInfo.Name),
                        r);
                    return expected;
                }
            };
            var sut = new OmitArrayParameterRequestRelay();

            var actual = sut.Create(parameterInfo, context);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(false)]
        [InlineData(true)]
        [InlineData(0)]
        [InlineData(42)]
        [InlineData("")]
        [InlineData("Foo")]
        [InlineData(typeof(object))]
        [InlineData(typeof(string))]
        public void CreateReturnsCorrectResultForNonParameterRequest(
            object request)
        {
            var sut = new OmitArrayParameterRequestRelay();
            var actual = sut.Create(request, new DelegatingSpecimenContext());
            Assert.Equal(new NoSpecimen(), actual);
        }

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(string))]
        [InlineData(typeof(int))]
        [InlineData(typeof(Version))]
        [InlineData(typeof(SingleParameterType<string>))]
        public void CreateWithNonEnumerableParameterRequestReturnsCorrectResult(
            Type argumentType)
        {
            var parameterInfo =
                typeof(SingleParameterType<>)
                    .MakeGenericType(new[] { argumentType })
                    .GetConstructors()
                    .First()
                    .GetParameters()
                    .First();
            var sut = new OmitArrayParameterRequestRelay();

            var dummyContext = new DelegatingSpecimenContext();
            var actual = sut.Create(parameterInfo, dummyContext);

            var expected = new NoSpecimen();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(typeof(object[]))]
        [InlineData(typeof(string[]))]
        [InlineData(typeof(int[]))]
        [InlineData(typeof(Version[]))]
        [InlineData(typeof(SingleParameterType<string>[]))]
        public void CreateReturnsCorrectResultWhenContextReturnsOmitSpecimen(
            Type argumentType)
        {
            var parameterInfo =
                typeof(SingleParameterType<>)
                    .MakeGenericType(new[] { argumentType })
                    .GetConstructors()
                    .First()
                    .GetParameters()
                    .First();
            var context = new DelegatingSpecimenContext
            {
                OnResolve = r => new OmitSpecimen()
            };
            var sut = new OmitArrayParameterRequestRelay();

            var actual = sut.Create(parameterInfo, context);

            var expected = Array.CreateInstance(
                argumentType.GetElementType(),
                0);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateWithNullContextThrows()
        {
            var sut = new OmitArrayParameterRequestRelay();
            Assert.Throws<ArgumentNullException>(
                () => sut.Create(new object(), null));
        }
    }
}
