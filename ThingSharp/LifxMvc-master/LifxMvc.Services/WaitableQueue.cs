using LifxNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxMvc.Services
{
	public class PacketEventArgs : EventArgs
	{
		public LifxPacketBase Packet { get; private set; }
		public PacketEventArgs(LifxPacketBase packet)
		{
			this.Packet = packet;
		}
	}


	public class WaitableQueue<T>

	{
		public event EventHandler<PacketEventArgs> ItemEnqueued;
		public event EventHandler<PacketEventArgs> ItemDequeued;

		Queue<T> Q { get; set; }

		#region Wrapper methods

		//
		// Summary:
		//     Initializes a new instance of the System.Collections.Generic.Queue`1 class that
		//     is empty and has the default initial capacity.
		public WaitableQueue()
		{
			Q = new Queue<T>();
		}
		//
		// Summary:
		//     Initializes a new instance of the System.Collections.Generic.Queue`1 class that
		//     is empty and has the specified initial capacity.
		//
		// Parameters:
		//   capacity:
		//     The initial number of elements that the System.Collections.Generic.Queue`1 can
		//     contain.
		//
		// Exceptions:
		//   T:System.ArgumentOutOfRangeException:
		//     capacity is less than zero.
		public WaitableQueue(int capacity)
		{
			Q = new Queue<T>(capacity);
		}
		//
		// Summary:
		//     Initializes a new instance of the System.Collections.Generic.Queue`1 class that
		//     contains elements copied from the specified collection and has sufficient capacity
		//     to accommodate the number of elements copied.
		//
		// Parameters:
		//   collection:
		//     The collection whose elements are copied to the new System.Collections.Generic.Queue`1.
		//
		// Exceptions:
		//   T:System.ArgumentNullException:
		//     collection is null.
		public WaitableQueue(IEnumerable<T> collection)
		{
			Q = new Queue<T>(collection);
		}

		//
		// Summary:
		//     Gets the number of elements contained in the System.Collections.Generic.Queue`1.
		//
		// Returns:
		//     The number of elements contained in the System.Collections.Generic.Queue`1.
		public int Count { get { return Q.Count; } }

		//
		// Summary:
		//     Removes all objects from the System.Collections.Generic.Queue`1.
		public void Clear()
		{
			Q.Clear();
		}
		//
		// Summary:
		//     Determines whether an element is in the System.Collections.Generic.Queue`1.
		//
		// Parameters:
		//   item:
		//     The object to locate in the System.Collections.Generic.Queue`1. The value can
		//     be null for reference types.
		//
		// Returns:
		//     true if item is found in the System.Collections.Generic.Queue`1; otherwise, false.
		public bool Contains(T item)
		{
			return Q.Contains(item);
		}
		//
		// Summary:
		//     Copies the System.Collections.Generic.Queue`1 elements to an existing one-dimensional
		//     System.Array, starting at the specified array index.
		//
		// Parameters:
		//   array:
		//     The one-dimensional System.Array that is the destination of the elements copied
		//     from System.Collections.Generic.Queue`1. The System.Array must have zero-based
		//     indexing.
		//
		//   arrayIndex:
		//     The zero-based index in array at which copying begins.
		//
		// Exceptions:
		//   T:System.ArgumentNullException:
		//     array is null.
		//
		//   T:System.ArgumentOutOfRangeException:
		//     arrayIndex is less than zero.
		//
		//   T:System.ArgumentException:
		//     The number of elements in the source System.Collections.Generic.Queue`1 is greater
		//     than the available space from arrayIndex to the end of the destination array.
		public void CopyTo(T[] array, int arrayIndex)
		{
			Q.CopyTo(array, arrayIndex);
		}
		//
		// Summary:
		//     Removes and returns the object at the beginning of the System.Collections.Generic.Queue`1.
		//
		// Returns:
		//     The object that is removed from the beginning of the System.Collections.Generic.Queue`1.
		//
		// Exceptions:
		//   T:System.InvalidOperationException:
		//     The System.Collections.Generic.Queue`1 is empty.
		public T Dequeue()
		{
			var item = Q.Dequeue();
			if (null != ItemDequeued)
				ItemDequeued(this, new PacketEventArgs(item as LifxPacketBase));
			return item;
		}
		//
		// Summary:
		//     Adds an object to the end of the System.Collections.Generic.Queue`1.
		//
		// Parameters:
		//   item:
		//     The object to add to the System.Collections.Generic.Queue`1. The value can be
		//     null for reference types.
		public void Enqueue(T item)
		{
			Q.Enqueue(item);
			if (null != ItemEnqueued)
				ItemEnqueued(this, new PacketEventArgs(item as LifxPacketBase));

		}
		//
		// Summary:
		//     Returns an enumerator that iterates through the System.Collections.Generic.Queue`1.
		//
		// Returns:
		//     An System.Collections.Generic.Queue`1.Enumerator for the System.Collections.Generic.Queue`1.
		public Queue<T>.Enumerator GetEnumerator()
		{
			return Q.GetEnumerator();
		}
		//
		// Summary:
		//     Returns the object at the beginning of the System.Collections.Generic.Queue`1
		//     without removing it.
		//
		// Returns:
		//     The object at the beginning of the System.Collections.Generic.Queue`1.
		//
		// Exceptions:
		//   T:System.InvalidOperationException:
		//     The System.Collections.Generic.Queue`1 is empty.
		public T Peek()
		{
			return Q.Peek();
		}
		//
		// Summary:
		//     Copies the System.Collections.Generic.Queue`1 elements to a new array.
		//
		// Returns:
		//     A new array containing elements copied from the System.Collections.Generic.Queue`1.
		public T[] ToArray()
		{
			return Q.ToArray();
		}
		//
		// Summary:
		//     Sets the capacity to the actual number of elements in the System.Collections.Generic.Queue`1,
		//     if that number is less than 90 percent of current capacity.
		public void TrimExcess()
		{
			Q.TrimExcess();
		}

		//
		// Summary:
		//     Enumerates the elements of a System.Collections.Generic.Queue`1.
#if false
		public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			//
			// Summary:
			//     Gets the element at the current position of the enumerator.
			//
			// Returns:
			//     The element in the System.Collections.Generic.Queue`1 at the current position
			//     of the enumerator.
			//
			// Exceptions:
			//   T:System.InvalidOperationException:
			//     The enumerator is positioned before the first element of the collection or after
			//     the last element.
			public T Current { get; }

			//
			// Summary:
			//     Releases all resources used by the System.Collections.Generic.Queue`1.Enumerator.
			public void Dispose();
			//
			// Summary:
			//     Advances the enumerator to the next element of the System.Collections.Generic.Queue`1.
			//
			// Returns:
			//     true if the enumerator was successfully advanced to the next element; false if
			//     the enumerator has passed the end of the collection.
			//
			// Exceptions:
			//   T:System.InvalidOperationException:
			//     The collection was modified after the enumerator was created.
			public bool MoveNext();
		}

#endif
		#endregion


	}
}
