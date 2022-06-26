using System;
using System.Collections.Generic;
using UnityEngine;

namespace LeMinhHuy
{
	public class Pool<T> : IDisposable
	{
		readonly Stack<T> stack;
		readonly int maxSize;

		public int count => this.stack.Count;
		// public int CountAll { get; private set; }
		// public int CountInactive => this.stack.Count;
		// public int CountActive => this.CountAll - this.CountInactive;

		public Pool(int defaultSize = 10, int maxSize = 10000)
		{
			if (maxSize <= 0) throw new System.ArgumentException("Max size must be greater than 0", nameof(maxSize));

			this.stack = new Stack<T>(defaultSize);
			this.maxSize = maxSize;
		}

		public T Get()
		{
			T obj;
			if (stack.Count == 0)
			{
				Debug.LogWarning("Nothing in the pool to get!");
				//Add a run create function
				return default;
			}
			else
			{
				obj = stack.Pop();
			}

			return obj;
		}

		public void Release(T element)
		{
			//Make sure the object hasn't already been released
			if (stack.Count > 0 && stack.Contains(element))
				throw new InvalidOperationException("Object that has already been released to the pool");

			//Only take in if there's enough room
			if (stack.Count < maxSize)
			{
				stack.Push(element);
			}
		}

		public void Clear()
		{
			stack.Clear();
		}

		public void Dispose() => this.Clear();
	}
}