using System;
using UnityEngine;

namespace UnityUtility.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DrawTypenameAttribute : PropertyAttribute { }
}
