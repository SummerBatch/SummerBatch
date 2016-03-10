using Microsoft.Practices.ObjectBuilder2;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Core.Unity.Injection
{
    /// <summary>
    /// Implementation of <see cref="IDependencyResolverPolicy"/> that converts a string to the
    /// specified type using <see cref="StringConverter"/>.
    /// </summary>
    /// <typeparam name="T">&nbsp;the type to convert the string to.</typeparam>
    public class LiteralValueDependencyResolverPolicy<T> : IDependencyResolverPolicy
    {
        private readonly object _value;

        /// <summary>
        /// Constructs a new <see cref="IDependencyResolverPolicy"/>.
        /// </summary>
        /// <param name="value">the string value to convert</param>
        public LiteralValueDependencyResolverPolicy(string value)
        {
            _value = StringConverter.Convert<T>(value);
        }

        /// <summary>
        /// Returns the converted value.
        /// </summary>
        /// <param name="context">the builder context</param>
        /// <returns>the converted value</returns>
        public object Resolve(IBuilderContext context)
        {
            return _value;
        }
    }
}