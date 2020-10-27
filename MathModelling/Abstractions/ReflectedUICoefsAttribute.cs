using System;

namespace MM.Abstractions
{
    internal class ReflectedTargetAttribute : Attribute
    {

    }
    internal class DefaultModAttribute : Attribute
    {

    }
    internal class ReflectedUICoefsAttribute : Attribute
    {
        /// <summary>
        /// Precision
        /// </summary>
        public int P { get; set; } = 2;
    }
}