﻿using System.Collections.Generic;

namespace GeoGen.Core.Utilities.Variations
{
    /// <summary>
    /// Represents a non-repeating variations generator from a given list of items with 
    /// a given number of elements. It's supposed to work in a lazy way (providing IEnumerables of results). 
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    public interface IVariationsProvider<T>
    {
        /// <summary>
        /// Generates all possible variations of a given list. For example: For the list {1, 2, 3} all 
        /// the variations with 2 elements are: {1, 2}, {1, 3}, {2, 1}, {2, 3}, {3, 1}, {3, 2}.
        /// The generation is lazy. The count of generated enumerables will be numberOfElements.
        /// </summary>
        /// <param name="list">The list whose elements are used in variations.</param>
        /// <param name="numberOfElement">The number of elements in each variation.</param>
        /// <returns>Lazy enumerable of all possible variations.</returns>
        IEnumerable<IEnumerable<T>> GetVariations(List<T> list, int numberOfElement);
    }
}