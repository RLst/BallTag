
using UnityEngine;

namespace LeMinhHuy
{
	public class BallDetector : MonoBehaviour
	{
		[SerializeField] bool onTriggerEnter = true;
		[SerializeField] bool onTriggerStay = false;

		void OnTriggerEnter(Collider other)
		{
			var hit = other.GetComponent<Ball>();
			if (hit is Ball)
			{
				SendMessageUpwards("OnBallTouch", hit as Ball);
			}
		}

		void OnTriggerStay(Collider other)
		{
			var hit = other.GetComponent<Ball>();
			if (hit is Ball)
			{
				SendMessageUpwards("OnBallTouch", hit as Ball);
			}
		}
	}
}