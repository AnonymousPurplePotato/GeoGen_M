﻿using GeoGen.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a <see cref="IContainer{T}"/> of distinct items that are compared based on their string versions.
    /// </summary>
    /// <typeparam name="T">The type of items in the container.</typeparam>
    public class StringBasedContainer<T> : IContainer<T>
    {
        #region Private fields

        /// <summary>
        /// The dictionary mapping the string versions of items to the items itself.
        /// </summary>
        private readonly Dictionary<string, T> _items = new Dictionary<string, T>();

        /// <summary>
        /// The converter of items to a string.
        /// </summary>
        private readonly IToStringConverter<T> _converter;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBasedContainer{T}"/> class.
        /// </summary>
        /// <param name="converter">The converter of items to a string.</param>
        public StringBasedContainer(IToStringConverter<T> converter)
        {
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        #endregion

        #region IContainer implementation

        /// <summary>
        /// Tries to add a given item to the container. If an equal version of the item is present 
        /// in the container, the item won't be added and the <paramref name="equalItem"/> will be set 
        /// to this equal version. Otherwise the item will be added and the <paramref name="equalItem"/> 
        /// will be set to the default value of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        /// <param name="equalItem">Either the equal version of the passed item from the container (if there's any), or the default value of the type <typeparamref name="T"/>.</param>
        public void TryAdd(T item, out T equalItem)
        {
            // Convert the object to a string
            var stringRepresentation = _converter.ConvertToString(item);

            // If we have it cached, we can return it directly 
            if (_items.ContainsKey(stringRepresentation))
            {
                // Set the equal object
                equalItem = _items[stringRepresentation];

                // Terminate
                return;
            }

            // Otherwise add the item to the container 
            _items.Add(stringRepresentation, item);

            // Set that there is no equal item
            equalItem = default;
        }

        /// <summary>
        /// Finds an item in the container equal to a given one.
        /// </summary>
        /// <param name="item">The item which equal version we're seeking.</param>
        /// <returns>The item from the container equal to this one, if it exists; otherwise null.</returns>
        public T FindEqualItem(T item) => _items.GetOrDefault(_converter.ConvertToString(item));

        #endregion

        #region IEnumerable implementation

        /// <summary>
        /// Gets a generic enumerator.
        /// </summary>
        /// <returns>A generic enumerator.</returns>
        public IEnumerator<T> GetEnumerator() => _items.Values.GetEnumerator();

        /// <summary>
        /// Gets a non-generic enumerator.
        /// </summary>
        /// <returns>A non-generic enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}