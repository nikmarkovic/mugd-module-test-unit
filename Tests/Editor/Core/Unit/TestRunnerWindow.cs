using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Tests.Editor.Core.Unit
{
    public class TestRunnerWindow : EditorWindow
    {
        private Vector2 _scrollPosition;

        [MenuItem("Tests/Run Unit Tests...")]
        public static void Run()
        {
            GetWindow<TestRunnerWindow>("Unit Tests", true,
                typeof (EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow"));
        }

        private void OnGUI()
        {
            EditorGUILayout.Separator();
            TestRunnerService.AutoRun = EditorGUILayout.Toggle("Automatically run tests", TestRunnerService.AutoRun);
            EditorGUILayout.Separator();

            if (!TestRunnerService.AutoRun && GUILayout.Button("Run Tests"))
            {
                TestRunnerService.RefreshTests();
                TestRunnerService.ExecuteTests();
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(string.Format("Results: {0}/{1}", TestRunnerService.NumberOfPassTests,
                TestRunnerService.TotalNumberOfTests));

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            TestRunnerService.Tests.Keys.ToList().ForEach(test =>
            {
                TestRunnerService.Tests[test] = GUILayout.Toggle(
                    TestRunnerService.Tests[test],
                    string.Format("{0} ({1}/{2})", test.Type.FullName, test.NumberOfPassTests, test.NumberOfTests),
                    test.NumberOfTests - test.NumberOfPassTests == 0 ? Green() : Red());
            });
            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("All", GUILayout.ExpandWidth(true)))
            {
                TestRunnerService.Tests.Keys.ToList().ForEach(test => TestRunnerService.Tests[test] = true);
            }
            if (GUILayout.Button("None", GUILayout.ExpandWidth(true)))
            {
                TestRunnerService.Tests.Keys.ToList().ForEach(test => TestRunnerService.Tests[test] = false);
            }
            EditorGUILayout.EndHorizontal();
        }

        private static GUIStyle Red()
        {
            return new GUIStyle(EditorStyles.toggle)
            {
                onNormal = {textColor = Color.red},
                fontStyle = FontStyle.Bold
            };
        }

        private static GUIStyle Green()
        {
            return new GUIStyle(EditorStyles.toggle)
            {
                onNormal = {textColor = Color.green},
                fontStyle = FontStyle.Bold
            };
        }
    }
}