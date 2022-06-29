
using UnityEngine;

namespace LeMinhHuy
{
	public class BallCatcher : MonoBehaviour
	{
		void OnTriggerEnter(Collider other)
		{
			var hit = other.GetComponent<Ball>();
			if (hit is Ball)
			{
				SendMessageUpwards("OnBallCaught", hit as Ball);
			}
		}
	}
}