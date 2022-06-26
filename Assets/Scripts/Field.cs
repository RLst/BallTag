
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
		BoxCollider col;
		public GameObject parent; //This is root the players will be in instantiated onto

		void Awake()
		{
			parent = transform.parent.gameObject;
			col = GetComponent<BoxCollider>();
		}

		public bool isPosWithinField(Vector3 position, float checkSphereRadius = 0.2f)
			=> Physics.CheckSphere(position, checkSphereRadius);

		//Get a random location on this field at specified height
		public Vector3 GetRandomLocationOnField(float height)
		{
			var point = Extensions.GetRandomPointInsideCollider(col);
			point.y = height;
			return point;
		}
	}
}