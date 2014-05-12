using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Tests.Editor.Core.Unit.Attributes;
using Assets.Tests.Editor.Core.Unit.Executors;
using UnityEditor;
using UnityEngine;

namespace Assets.Tests.Editor.Core.Unit
{
    [InitializeOnLoad]
    public static class TestRunnerService
    {
        public static readonly Dictionary<TestRunner, bool> Tests = new Dictionary<TestRunner, bool>();
        private static bool _autoRun;
        private static bool _dirty;

        static TestRunnerService()
        {
            _autoRun = PlayerPrefs.GetInt("Assets.Tests.Editor.Core.Unit.TestRunnerService._autoRun", 0) != 0;
            _dirty = PlayerPrefs.GetInt("Assets.Tests.Editor.Core.Unit.TestRunnerService._dirty", 0) != 0;
            EditorApplication.update += Update;
        }

        public static int TotalNumberOfTests { get; private set; }

        public static int NumberOfPassTests { get; private set; }

        public static bool AutoRun
        {
            get { return _autoRun; }
            set
            {
                if (value == _autoRun) return;
                PlayerPrefs.SetInt("Assets.Tests.Editor.Core.Unit.TestRunnerService._autoRun", value ? 1 : 0);
                _autoRun = !_autoRun;
            }
        }

        public static bool Dirty
        {
            get { return _dirty; }
            set
            {
                if (value == _dirty) return;
                PlayerPrefs.SetInt("Assets.Tests.Editor.Core.Unit.TestRunnerService._dirty", value ? 1 : 0);
                _dirty = !_dirty;
            }
        }

        public static void RefreshTests()
        {
            var tests = Assembly.GetAssembly(typeof (TestRunner)).GetTypes()
                .Where(type => type.GetCustomAttributes(true).Count(a => a is TestFixtureAttribute) != 0).ToList();
            RemoveNotExistingTests(tests);
            AddNewTests(tests);
            Tests.Keys.ToList().ForEach(test => test.Refresh());
        }

        public static void ExecuteTests()
        {
            NumberOfPassTests = 0;
            TotalNumberOfTests = 0;
            Tests.Where(test => test.Value).ToList().ForEach(test =>
            {
                test.Key.Execute();
                NumberOfPassTests += test.Key.NumberOfPassTests;
                TotalNumberOfTests += test.Key.NumberOfTests;
            });
        }

        private static void RemoveNotExistingTests(IEnumerable<Type> types)
        {
            if (types == null) throw new ArgumentNullException("types");
            Tests.Keys.ToList().ForEach(test =>
            {
                if (!types.Contains(test.Type))
                {
                    Tests.Remove(test);
                }
            });
        }

        private static void AddNewTests(IEnumerable<Type> types)
        {
            if (types == null) throw new ArgumentNullException("types");
            types.ToList().ForEach(type =>
            {
                var exist = false;
                Tests.Keys.ToList().ForEach(test => exist = exist || type == test.Type);
                if (!exist) Tests.Add(TestRunner.NewInstance(type), true);
            });
        }

        private static void Update()
        {
            if (Dirty && !EditorApplication.isCompiling)
            {
                RefreshTests();
                if (AutoRun)
                {
                    ExecuteTests();
                }
                Dirty = false;
            }
            if (Dirty || !EditorApplication.isCompiling) return;
            Dirty = true;
        }
    }
}