
using UnityEngine;
using UnityEngine.AI;

namespace LeMinhHuy
{
	public class Ball : MonoBehaviour
	{
		Collider col;
		Rigidbody rb;

		void Awake()
		{
			col = GetComponent<Collider>();
			rb = GetComponent<Rigidbody>();
		}

		// void OnTriggerEnter(Collider other)
		// {
		// 	var hit = other.GetComponent<Unit>();
		// 	if (hit is object)
		// 	{
		// 		SendMessageUpwards("OnBallEnter", this);
		// 	}
		// }
	}
}