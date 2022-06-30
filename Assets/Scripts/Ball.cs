
using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace LeMinhHuy
{
	public class Ball : MonoBehaviour
	{
		[SerializeField] float startDelay = 2.5f;     //To let the ball bounce a bit

		Collider col;
		Rigidbody rb;
		// NavMeshAgent agent;
		//agent required to keep ball on playing field?
		//And to maybe use the system to pass between players
		//Go direct, minimal radius, actively seek out receiver

		void Awake()
		{
			col = GetComponent<Collider>();
			rb = GetComponent<Rigidbody>();
			// agent = GetComponent<NavMeshAgent>();
		}
		void Start()
		{
			SetActivatePhysics(true);
		}

		// public void OnEnable()
		// {
		// 	//Delay agent every time the ball get's cycled
		// 	StartCoroutine(Delay());
		// }

		// IEnumerator Delay()
		// {
		// 	agent.enabled = false;
		// 	yield return new WaitForSeconds(startDelay);
		// 	agent.enabled = true;
		// }

		public void SetActivatePhysics(bool active)
		{
			col.enabled = active;
			rb.isKinematic = !active;
			// agent.enabled = active;
		}

		/// <summary>
		/// Passes this ball to a unit. Unit should be on the same team
		/// </summary>
		/// <param name="receiver">Receiving unit on the same team</param>
		public void Pass(Unit receiver)
		{
			//Calculate time required to move at the desired speed based on distance
			//Time = Distance / Speed
			//Optimization note: This only happens once and not every frame so it's ok
			var distance = Vector3.Distance(this.transform.position, receiver.hands.transform.position);
			float speed = receiver.team.strategy.ballSpeed;
			float time = distance / speed;

			//Move towards receiving unit then make unit grab ball
			transform.DOMove(receiver.transform.position, time).OnComplete(receiver.SeizeBall);
		}
	}
}