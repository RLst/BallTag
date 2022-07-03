
using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace LeMinhHuy
{
	// [RequireComponent(typeof(NavMeshAgent))]
	public class Ball : Singleton<Ball>
	{
		[SerializeField] float startDelay = 5f;     //To let the ball bounce a bit
		[SerializeField] float distancePerBounce = 5f;
		[SerializeField] float passHeight = 2.5f;
		Collider col;
		Rigidbody rb;
		Renderer r;
		//Probably don't need the agent. Just use a funnel over the stadium so the ball doesn't go too far out
		// NavMeshAgent agent;     //This is only so the ball stays within the playing boundaries
		Stadium stadium;

		protected override void Init()
		{
			r = GetComponent<Renderer>();
			col = GetComponent<Collider>();
			rb = GetComponent<Rigidbody>();
			// agent = GetComponent<NavMeshAgent>();
			stadium = Stadium.current;
		}
		void Start()
		{
			ResetParentToStadium();
			SetActivatePhysics(true);
			// agent.enabled = false;
		}

		public void Hide() => r.enabled = false;
		public void Show() => r.enabled = true;
		// public void GroundBall()
		// {
		// 	//Just turns the agent on for a second to ground it on the nav mesh
		// 	StartCoroutine(groundBall());
		// 	IEnumerator groundBall()
		// 	{
		// 		agent.enabled = true;
		// 		yield return new WaitForSeconds(1f);
		// 		agent.enabled = false;
		// 	}
		// }

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
		/// Passes this ball to a unit with simulated bounce effects
		/// </summary>
		/// <param name="receiver">Receiving unit on the same team</param>
		public void Pass(Unit receiver)
		{
			// print("Passing ball to " + receiver.name);

			//Calculate time required to move at the desired speed based on distance
			//Time = Distance / Speed
			//Optimization note: v3.dist() not ideal but this only happens once and not repeatedly so it's ok
			var distance = Vector3.Distance(this.transform.position, receiver.transform.position);
			float speed = receiver.team.strategy.ballSpeed;
			float time = distance / speed;

			//Calculate bounces based on distance
			int bounces = Mathf.RoundToInt(distance / distancePerBounce);  // print("Bounces: " + bounces);

			//Move towards receiving unit then make unit grab ball
			transform.DOJump(receiver.transform.position, passHeight, bounces, time)
				//The physics have to be activated so that the other unit can detect it so that they can kill the tween early
				.OnStart(() => SetActivatePhysics(true))
				//Deactivate physics so the ball doesn't move around randomly
				.OnKill(() => SetActivatePhysics(false));
		}

		/// <summary>
		/// Release ball from units and root it under the stadium
		/// Move the ball to the same level as the playing surface
		/// </summary>
		public void ResetParentToStadium()
		{
			this.transform.SetParent(stadium.transform);
		}

		public void CenterBallToStadium()
		{
			const float smallAmountAbovePlayingSurface = 0.5f;
			transform.position = stadium.transform.position + Vector3.up * smallAmountAbovePlayingSurface;
		}
	}
}