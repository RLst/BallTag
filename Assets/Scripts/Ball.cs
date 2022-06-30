
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace LeMinhHuy
{
	public class Ball : MonoBehaviour
	{
		[SerializeField] float startDelay = 2.5f;     //To let the ball bounce a bit

		Collider col;
		Rigidbody rb;
		NavMeshAgent agent;
		//agent required to keep ball on playing field?
		//And to maybe use the system to pass between players
		//Go direct, minimal radius, actively seek out receiver

		void Awake()
		{
			col = GetComponent<Collider>();
			rb = GetComponent<Rigidbody>();
			agent = GetComponent<NavMeshAgent>();
		}

		public void OnEnable()
		{
			//Delay agent every time the ball get's cycled
			StartCoroutine(Delay());
		}

		IEnumerator Delay()
		{
			agent.enabled = false;
			yield return new WaitForSeconds(startDelay);
			agent.enabled = true;
		}

		public void SetActivatePhysics(bool active)
		{
			col.enabled = active;
			rb.isKinematic = !active;
			agent.enabled = active;
		}

		public void Pass(Unit u)
		{

		}
	}
}