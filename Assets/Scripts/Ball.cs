
using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace LeMinhHuy
{
	public class Ball : MonoBehaviour
	{
		[SerializeField] float startDelay = 5f;     //To let the ball bounce a bit
		[SerializeField] float distancePerBounce = 5f;
		[SerializeField] float passHeight = 2.5f;
		Collider col;
		Rigidbody rb;
		Unit receiver;

		void Awake()
		{
			col = GetComponent<Collider>();
			rb = GetComponent<Rigidbody>();
		}
		void Start()
		{
			SetActivatePhysics(true);
		}

		/// <summary>
		/// Deactivating physics will lock the ball in position ie. for carrying
		/// NOTE: Unit's trigger detector can't detect unless this is set to true
		/// </summary>
		public void SetActivatePhysics(bool active)
		{
			//Activating physics
			col.enabled = active;
			rb.isKinematic = !active;
		}

		/// <summary>
		/// Passes this ball to a unit. Unit should be on the same team
		/// </summary>
		/// <param name="receiver">Receiving unit on the same team</param>
		public void Pass(Unit receiver)
		{
			print("Passing ball to " + receiver.name);
			this.receiver = receiver;

			//Calculate time required to move at the desired speed based on distance
			//Time = Distance / Speed
			//Optimization note: This only happens once and not every frame so it's ok
			var distance = Vector3.Distance(this.transform.position, receiver.transform.position);
			float speed = receiver.team.strategy.ballSpeed;
			float time = distance / speed;

			//Calculate bounces based on distance
			int bounces = Mathf.RoundToInt(distance / distancePerBounce);
			print("Bounces: " + bounces);

			//Move towards receiving unit then make unit grab ball
			transform.DOJump(receiver.transform.position, passHeight, bounces, time)
				//The physics have to be activated so that
				.OnStart(() => SetActivatePhysics(true))
				.OnKill(() =>
				{
					SetActivatePhysics(false);
					receiver = null;
				});
		}
	}
}