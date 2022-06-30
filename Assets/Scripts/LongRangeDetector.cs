using System;
using DG.Tweening;
using UnityEngine;

namespace LeMinhHuy
{
	/// <summary>
	/// Detects long range ie. Attacking units
	/// </summary>
	[RequireComponent(typeof(CapsuleCollider))]
	public class LongRangeDetector : MonoBehaviour
	{
		[SerializeField] Unit owner;

		[Header("Detection method")]
		[SerializeField] bool onTriggerEnter = true;
		[SerializeField] bool onTriggerStay = false;
		[SerializeField] bool onTriggerExit = false;

		private MeshRenderer mr;
		private CapsuleCollider col;
		GameSettings settings;

		void Awake()
		{
			mr = GetComponent<MeshRenderer>();
			col = GetComponent<CapsuleCollider>();
			settings = GameController.current.settings;
		}
		void Start()
		{
			Debug.Assert(owner is object, "Detector has no owner");

			col.isTrigger = true;

			//Set detector to the correct size
			var scanWidth = settings.fieldWidth * owner.team.strategy.detectionRange;
			this.transform.DOScaleX(scanWidth, 1f);
			this.transform.DOScaleZ(scanWidth, 1f);
		}

		void OnTriggerEnter(Collider other)
		{
			if (!onTriggerEnter) return;
			var unitFound = other.GetComponent<Unit>();
			if (unitFound is object)
				SendMessageUpwards("OnInsideDetectionZone", unitFound);
		}
		void OnTriggerStay(Collider other)
		{
			if (!onTriggerStay) return;
			var unitFound = other.GetComponent<Unit>();
			if (unitFound is object)
				SendMessageUpwards("OnInsideDetectionZone", unitFound);
		}
		void OnTriggerExit(Collider other)
		{
			if (!onTriggerExit) return;
			var unitFound = other.GetComponent<Unit>();
			if (unitFound is object)
				SendMessageUpwards("OnInsideDetectionZone", unitFound);
		}
	}
}