using System;
using UnityEngine;

namespace LeMinhHuy
{
	[RequireComponent(typeof(CapsuleCollider))]
	public class DetectionZone : MonoBehaviour
	{
		private MeshRenderer mr;
		private CapsuleCollider col;
		void Awake()
		{
			mr = GetComponent<MeshRenderer>();
			col = GetComponent<CapsuleCollider>();
		}

		void OnTriggerEnter(Collider other)
		{
			var hit = other.GetComponent<Unit>();
			if (hit is object)
				SendMessageUpwards("OnDetectionZoneEnter", hit);
		}

		public void Hide()
		{
			mr.enabled = false;
		}
		public void Show()
		{
			mr.enabled = true;
		}
	}
}