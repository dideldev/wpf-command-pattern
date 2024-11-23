using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dideldev.Wpf.CommandPattern.Structures
{
    /// <summary>
    /// Defines a stack with a limited size. 
    /// </summary>
    /// <remarks>
    /// It is implemented through lists.
    /// </remarks>
    /// <typeparam name="T">Type of the content.</typeparam>
    /// <remarks>
    /// Initializes a new instance of <see cref="CappedStack{T}"/>.
    /// </remarks>
    public class CappedStack<T> : IEnumerable<T>
    {
        /// <summary>
        /// Item container.
        /// </summary>
        private List<T> items = [];

        /// <summary>
        /// Maximum number of items in the stack. 
        /// </summary>
        public int MaxCount { get; private set; } = 1000;

        /// <summary>
        /// Initializes a new instance of <see cref="CappedStack{T}"/>.
        /// </summary>
        /// <param name="maxCount"></param>
        public CappedStack(int maxCount = 1000)
        {
            MaxCount = maxCount;
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxCount);
        }

        /// <summary>
        /// Get or sets the items in the stack.
        /// </summary>
        public List<T> Items
        {
            get => items;
            internal set => items = value;
        }

        /// <summary>
        /// Get or sets the numeber of items in the stack. 
        /// </summary>
        public int Count => items.Count;

        /// <summary>
        /// Gets wether the stack contains any item or not.
        /// </summary>
        public bool HasItems => items.Count > 0;


        /// <summary>
        /// Remove all elements in the stack. 
        /// </summary>
        public void Clear()
        {
            items.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)items).GetEnumerator();
        }

        /// <summary>
        /// Gets the last added element on the stack and removeit. 
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            if (items.Count == 0)
                throw new InvalidOperationException("Pop when no items left on the stack.");

            T item = items[^1];
            items.RemoveAt(items.Count - 1);
            return item;
        }

        /// <summary>
        /// Adds a new item on the top of the stack.
        /// </summary>
        /// <param name="item"></param>
        public void Push(T item)
        {
            items.Add(item);
            while (items.Count > MaxCount)
            {
                items.RemoveAt(0);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)items).GetEnumerator();
        }
    }
}
