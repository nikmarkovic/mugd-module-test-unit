using System;

namespace Assets.Tests.Editor.Core.Unit.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestAttribute : Attribute
    {
    }
}