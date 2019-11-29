using System;
using System.Collections.Generic;

namespace UnityEngine.Tilemaps
{
    public interface IRuleOverrideTile<T>
    {

        /// <summary>
        /// Gets the overriding T of a given T. 
        /// </summary>
        /// <param name="original">The original T that is overridden</param>
        T this[T original] { get; set; }

        /// <summary>
        /// Applies overrides to this
        /// </summary>
        /// <param name="overrides">A list of overrides to apply</param>
        /// <exception cref="ArgumentNullException">The input overrides list is not valid</exception>
        void ApplyOverrides(IList<KeyValuePair<T, T>> overrides);

        /// <summary>
        /// Gets overrides for this
        /// </summary>
        /// <param name="overrides">A list of overrides to fill</param>
        /// <exception cref="ArgumentNullException">The input overrides list is not valid</exception>
        void GetOverrides(List<KeyValuePair<T, T>> overrides);
    }
}
