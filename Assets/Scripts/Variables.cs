using System;
using UnityEngine;

namespace LeMinhHuy.Variables
{
	[CreateAssetMenu]
	public class FloatVar : VarBase<float>
	{
	}

	[CreateAssetMenu]
	public class IntVar : VarBase<int>
	{
	}
	public abstract class VarBase<T> : ScriptableObject
	{
		T _value;
		public virtual T value
		{
			get => _value;
			set
			{
				if (!this.value.Equals(value))
				{
					onChange.Invoke();
					this.value = value;
				}
			}
		}
		public Action onChange;
	}
}
