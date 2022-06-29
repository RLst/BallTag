
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
	}
}