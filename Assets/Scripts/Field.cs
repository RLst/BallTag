
using UnityEngine;
using LeMinhHuy.Utils;

namespace LeMinhHuy
{
	/// <summary>
	/// A playing field
	/// </summary>
	[RequireComponent(typeof(BoxCollider))]
	public class Field : MonoBehaviour
	{
		//Members
		new public BoxCollider collider;

		void Awake()
		{
			collider = GetComponent<BoxCollider>();
		}

		public bool isPosWithinField(Vector3 position, float checkSphereRadius = 0.2f)
			=> Physics.CheckSphere(position, checkSphereRadius);

		//Get a random location on this field at specified height
		public Vector3 GetRandomLocationOnField(float height)
		{
			var point = Extensions.GetRandomPointInsideCollider(collider);
			point.y = height;
			return point;
		}
	}
}