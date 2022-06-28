using System;
using System.Collections.Generic;
using UnityEngine;

namespace LeMinhHuy
{
	/// <summary>
	/// Basic object pooling with collection checks
	/// to prevent double recyling and improved callbacks and counters
	/// </summary>
	public class Pool<T> : IDisposable
	{
		readonly Stack<T> stack;
		readonly Func<T> createFunction;
		readonly Action<T> actionOnGet;
		readonly Action<T> actionOnRelease;
		readonly Action<T> actionOnDestroy;
		readonly int maxSize;
		public bool collectionCheck;

		//Counters
		public int countAll { get; private set; }   //All units
		public int countInactive => this.stack.Count;   //Units that have being recyled
		public int countActive => this.countAll - this.countInactive;   //Units that are currently in use

		//Constructors
		public Pool(
			Func<T> createFunction,
			Action<T> actionOnGet = null,
			Action<T> actionOnRecycle = null,
			Action<T> actionOnDestroy = null,
			bool collectionCheck = true,
			int defaultSize = 10,
			int maxSize = 10000)
		{
			if (createFunction is null)
				throw new ArgumentException("No create function passed in", nameof(createFunction));

			if (maxSize <= 0)
				throw new System.ArgumentException("Max size must be greater than 0", nameof(maxSize));

			this.stack = new Stack<T>(defaultSize);

			this.createFunction = createFunction;
			this.actionOnGet = actionOnGet;
			this.actionOnRelease = actionOnRecycle;
			this.actionOnDestroy = actionOnDestroy;

			this.collectionCheck = collectionCheck;
			this.maxSize = maxSize;
		}

		//Get an object from the pool. Make one with supplied create function if there's none
		public T Get()
		{
			T element;
			if (stack.Count == 0)
			{
				//Create
				element = createFunction();
				++countAll;
			}
			else
			{
				//Pull
				element = stack.Pop();
			}

			//Perform get action on object
			var actionOnGet = this.actionOnGet;
			if (actionOnGet is object)
				actionOnGet(element);

			return element;
		}

		//Push object back into pool
		public void Recycle(T element)
		{
			//Make sure the object hasn't already been released
			if (collectionCheck && stack.Count > 0 && stack.Contains(element))
			{
				Debug.LogWarning("Object has already been recycled ");
				return;
			}

			//Perform release operations
			Action<T> actionOnRelease = this.actionOnRelease;
			if (actionOnRelease is object)
				actionOnRelease(element);

			//Destroy if there's way too much
			if (countActive < maxSize)
			{
				stack.Push(element);
			}
			else
			{
				Action<T> actionOnDetroy = this.actionOnDestroy;
				if (actionOnDestroy is object)
					actionOnDestroy(element);
			}
		}

		public void Clear()
		{
			if (actionOnDestroy is object)
			{
				foreach (T element in stack)
					this.actionOnDestroy(element);
			}
			stack.Clear();
			countAll = 0;
		}

		public void Dispose() => this.Clear();
	}
}