using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Tests.Editor.Core.Unit.Attributes;
using UnityEngine;
using Object = System.Object;

namespace Assets.Tests.Editor.Core.Unit.Executors
{
    public class TestRunner
    {
        private readonly List<string> _result = new List<string>();
        private readonly Object _testInstance;
        private readonly List<MethodInfo> _testMethods = new List<MethodInfo>();
        private readonly Type _testType;
        private MethodInfo _setUpMethod;
        private MethodInfo _tearDownMethod;
        private MethodInfo _testFixtureSetUpMethod;
        private MethodInfo _testFixtureTearDownMethod;

        private TestRunner(Type test)
        {
            _testType = test;
            _testInstance = Assembly.GetAssembly(typeof (TestRunner)).CreateInstance(_testType.FullName);
            Refresh();
        }

        public Type Type
        {
            get { return _testType; }
        }

        public int NumberOfTests
        {
            get { return _testMethods.Count(); }
        }

        public int NumberOfPassTests
        {
            get { return _result.Count(result => result.StartsWith("[PASS]")); }
        }

        public List<string> Result
        {
            get { return _result; }
        }

        public static TestRunner NewInstance(Type test)
        {
            return IsValidType(test) ? new TestRunner(test) : null;
        }

        public List<string> Execute()
        {
            _result.Clear();
            InvokeSetupMethod(_testFixtureSetUpMethod);
            _testMethods.ForEach(testMethod =>
            {
                InvokeSetupMethod(_setUpMethod);
                InvokeTestMethod(testMethod);
                InvokeSetupMethod(_tearDownMethod);
            });
            InvokeSetupMethod(_testFixtureTearDownMethod);
            return _result;
        }

        public void Refresh()
        {
            if (!IsValidType(_testType)) return;
            _result.Clear();
            _testMethods.Clear();
            _testType.GetMethods().ToList().ForEach(method =>
            {
                var methodAttributes = method.GetCustomAttributes(true);
                if (methodAttributes.Count(methodAttribute => methodAttribute is TestFixtureSetUpAttribute) != 0)
                {
                    _testFixtureSetUpMethod = method;
                }
                else if (methodAttributes.Count(methodAttribute => methodAttribute is SetUpAttribute) != 0)
                {
                    _setUpMethod = method;
                }
                else if (methodAttributes.Count(methodAttribute => methodAttribute is TestAttribute) != 0)
                {
                    _testMethods.Add(method);
                }
                else if (methodAttributes.Count(methodAttribute => methodAttribute is TearDownAttribute) != 0)
                {
                    _tearDownMethod = method;
                }
                else if (methodAttributes.Count(methodAttribute => methodAttribute is TestFixtureTearDownAttribute) != 0)
                {
                    _testFixtureTearDownMethod = method;
                }
            });
        }

        private static bool IsValidType(Type type)
        {
            return type.GetCustomAttributes(true).Count(typeAttribute => typeAttribute is TestFixtureAttribute) != 0;
        }

        private void InvokeTestMethod(MethodBase method)
        {
            if (method == null) return;
            string result;
            try
            {
                method.Invoke(_testInstance, new Object[0]);
                result = string.Format("[{2}] {0}.{1}", _testType, method.Name, "PASS");
                Debug.Log(result);
            }
            catch (Exception exception)
            {
                result = string.Format("[{2}] {0}.{1}. {3}", _testType, method.Name, "FAIL", exception);
                Debug.LogError(result);
            }

            _result.Add(result);
        }

        private void InvokeSetupMethod(MethodBase method)
        {
            if (method == null) return;
            try
            {
                method.Invoke(_testInstance, new Object[0]);
            }
            catch
            {
                Debug.LogError(string.Format("[{2}] {0}.{1}", _testType, method.Name, "FAIL"));
            }
        }
    }
}