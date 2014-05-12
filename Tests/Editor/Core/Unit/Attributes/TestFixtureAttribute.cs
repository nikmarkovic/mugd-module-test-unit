using System;

namespace Assets.Tests.Editor.Core.Unit.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TestFixtureAttribute : Attribute
    {
    }
}