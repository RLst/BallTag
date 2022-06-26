
using UnityEngine;
using LeMinhHuy.Utils;

namespace LeMinhHuy
{
	[RequireComponent(typeof(BoxCollider))]
	public class Field : MonoBehaviour
	{
		BoxCollider col;

		public GameObject test;

		void Awake() => col = GetComponent<BoxCollider>();

		void Start()
		{
			InvokeRepeating("AddRandomObject", 0f, 2f);
		}
		void AddRandomObject()
		{
			Instantiate(test, GetRandomLocationOnField(0.5f), Quaternion.identity);
		}

		public Vector3 GetRandomLocationOnField(float height)
		{
			var point = Extensions.GetRandomPointInsideCollider(col);
			point.y = height;
			return point;
		}
	}
}