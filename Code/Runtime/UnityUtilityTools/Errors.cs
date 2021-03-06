using System;
using UnityUtility.Async;

namespace UnityUtilityTools
{
    internal static class Errors
    {
        public static NotSupportedException ReadOnlyBitList()
        {
            return new NotSupportedException("BitList is read only.");
        }

        public static InvalidOperationException NoElements()
        {
            return new InvalidOperationException("Collection is empty.");
        }

        public static InvalidOperationException CollectionChanged()
        {
            return new InvalidOperationException("Collection has been changed.");
        }

        public static ArgumentOutOfRangeException NegativeParameter(string paramName)
        {
            return new ArgumentOutOfRangeException(paramName, "The value cannot be negative.");
        }

        public static ArgumentOutOfRangeException ZeroParameter(string paramName)
        {
            return new ArgumentOutOfRangeException(paramName, "Argument must be greater than zero.");
        }

        public static ArgumentNullException NullParameter(string paramName)
        {
            return new ArgumentNullException(paramName, "Argument cannot be null.");
        }

        public static ArgumentException InvalidArrayArgument(string paramName)
        {
            return new ArgumentException(paramName, "Array argument cannot be null or empty.");
        }

        public static ArgumentOutOfRangeException OutOfRange(string paramName, string minName, string maxName)
        {
            return new ArgumentOutOfRangeException(paramName, $"The value cannot be out of range between {minName} and {maxName}.");
        }

        public static ArgumentException DifferentArrayLengths()
        {
            return new ArgumentException("Array lengths are not equal.");
        }

        public static IndexOutOfRangeException IndexOutOfRange()
        {
            return new IndexOutOfRangeException("The index is out of range.");
        }

        public static InvalidOperationException CannotStopTask()
        {
            return new InvalidOperationException($"Task cannot be stopped. Check {TaskSystem.SYSTEM_NAME} settings.");
        }

        public static ArgumentOutOfRangeException NegativeTime(string paramName)
        {
            return new ArgumentOutOfRangeException(paramName, "Time cannot be negative.");
        }

        public static InvalidOperationException DisposingNonEditable()
        {
            return new InvalidOperationException("Non-editable objects cannot be disposed. Probably it is a prefab reference.");
        }

        public static ArgumentOutOfRangeException MinMax(string minName, string maxName)
        {
            return new ArgumentOutOfRangeException(minName, $"{minName} cannot be greater than {maxName}.");
        }

        public static InvalidOperationException RangeDoesNotContain(string valuesName)
        {
            return new InvalidOperationException($"The range does not contain {valuesName} values.");
        }

        public static ArgumentOutOfRangeException RadixOutOfRange(string paramName, int symbolsLength)
        {
            return new ArgumentOutOfRangeException(paramName, $"The radix must be >= 2 and <= {symbolsLength}");
        }

        public static InvalidOperationException EmptyTraker()
        {
            return new InvalidOperationException("Tracker does not yet contain nodes.");
        }
    }
}
