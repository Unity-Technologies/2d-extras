using System;
using System.Collections.Generic;

namespace UnityEngine.Tilemaps
{
    public interface IRuleOverrideTile<T, K>
    {

        /// <summary>
        /// Gets the overriding K of a given T. 
        /// </summary>
        /// <param name="original">The original T that is overridden</param>
        K this[T original] { get; set; }

        /// <summary>
        /// Applies overrides to this
        /// </summary>
        /// <param name="overrides">A list of overrides to apply</param>
        /// <exception cref="ArgumentNullException">The input overrides list is not valid</exception>
        void ApplyOverrides(IList<KeyValuePair<T, K>> overrides);

        /// <summary>
        /// Gets overrides for this
        /// </summary>
        /// <param name="overrides">A list of overrides to fill</param>
        /// <exception cref="ArgumentNullException">The input overrides list is not valid</exception>
        void GetOverrides(List<KeyValuePair<T, K>> overrides);
    }
}
