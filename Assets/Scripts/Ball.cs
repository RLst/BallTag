
using UnityEngine;
using UnityEngine.AI;

namespace LeMinhHuy
{
	[RequireComponent(typeof(NavMeshAgent))]
	public class Ball : MonoBehaviour
	{
		private NavMeshAgent agent;

		void Awake()
		{
			agent = GetComponent<NavMeshAgent>();
		}
	}
}