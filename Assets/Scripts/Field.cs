
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
		const float CHECK_CAST_LENGTH = 10f;
		//Members
		[HideInInspector] new public BoxCollider collider;
		RaycastHit[] hitResults;

		void Awake()
		{
			collider = GetComponent<BoxCollider>();
			hitResults = new RaycastHit[10];
		}

		public bool isPointWithinField(Vector3 position, float sphereCastRadius = 0.1f)
		{
			var posHigh = position + Vector3.up * 5f;
			Debug.DrawRay(posHigh, Vector3.down * CHECK_CAST_LENGTH, Color.green, 5f);
			if (Physics.SphereCastNonAlloc(posHigh, sphereCastRadius, Vector3.down, hitResults, CHECK_CAST_LENGTH) > 0)
			{
				// Debug.Log("Hit");
				foreach (var hr in hitResults)
					if (hr.collider == this.collider)
						return true;
			}
			return false;
		}

		//Get a random location on this field at specified height
		public Vector3 GetRandomLocationOnField(float height = 0.05f)
		{
			var point = Extensions.GetRandomPointInsideCollider(collider);
			point.y = this.transform.position.y + height;
			return point;
		}
	}
}