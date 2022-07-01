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
		[SerializeField] MeshRenderer mr;
		[SerializeField] float scaleUpTime = 0.6f;
		[SerializeField] float rotationSpeed = 40f;

		[Header("Detection method")]
		[SerializeField] bool onTriggerEnter = true;
		[SerializeField] bool onTriggerStay = false;
		[SerializeField] bool onTriggerExit = false;

		private CapsuleCollider col;
		GameSettings settings;
		Vector3 origScale;

		void Awake()
		{
			col = GetComponent<CapsuleCollider>();
			settings = GameController.current.settings;
			origScale = transform.localScale;
		}
		void Start()
		{
			Debug.Assert(owner is object, "Detector has no owner");

			col.isTrigger = true;
		}

		void OnEnable()
		{
			if (owner.team is null) return;

			this.transform.localScale = origScale;

			//Set detector to the correct size
			var scanWidth = settings.fieldWidth * owner.team.strategy.detectionRange;
			this.transform.DOScaleX(scanWidth, scaleUpTime);
			this.transform.DOScaleZ(scanWidth, scaleUpTime);

			//Try to set the team color of the radar
			SetColor();
		}

		void Update()
		{
			this.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
		}

		void SetColor()
		{
			if (owner is null) return;
			if (owner.team is null) return;
			mr.material.color = owner.team.color;
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