using System;

namespace Assets.Tests.Editor.Core.Unit.Executors
{
    public static class Assert
    {
        public static void AreEqual<T>(T expected, T actual, string message = "")
        {
            if (expected == null && actual == null) return;
            if(expected == null && actual != null) throw new Exception(message);
            if (!expected.Equals(actual)) throw new Exception(message);
        }

        public static void AreNotEqual<T>(T notExpected, T actual, string message = "")
        {
            if (notExpected == null && actual == null) throw new Exception(message);
            if (notExpected == null && actual != null) return;
            if (notExpected.Equals(actual)) throw new Exception(message);
        }

        public static void AreSame(Object expected, Object actual, string message = "")
        {
            if (!ReferenceEquals(expected, actual)) throw new Exception(message);
        }

        public static void AreNotSame(Object notExpected, Object actual, string message = "")
        {
            if (ReferenceEquals(notExpected, actual)) throw new Exception(message);
        }

        public static void IsFalse(bool condition, string message = "")
        {
            if (condition) throw new Exception(message);
        }

        public static void IsTrue(bool condition, string message = "")
        {
            if (!condition) throw new Exception(message);
        }

        public static void IsNull(Object value, string message = "")
        {
            if (value != null) throw new Exception(message);
        }

        public static void IsNotNull(Object value, string message = "")
        {
            if (value == null) throw new Exception(message);
        }

        public static void IsInstanceOfType(Object value, Type expectedType, string message = "")
        {
            if (value == null) throw new Exception(message);
            if (value.GetType() != expectedType) throw new Exception(message);
        }

        public static void IsNotInstanceOfType(Object value, Type wrongType, string message = "")
        {
            if (value == null) throw new Exception(message);
            if (value.GetType() == wrongType) throw new Exception(message);
        }

        public static void ShouldThrowException<T>(Action action, string message = "") where T : Exception
        {
            try
            {
                action.Invoke();
            }
            catch (T)
            {
            }
            catch
            {
                throw new Exception(message);
            }
        }
    }
}