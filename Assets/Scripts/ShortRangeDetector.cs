
using UnityEngine;

namespace LeMinhHuy
{
	/// <summary>
	/// Detects short range objects ie. ball and units to tag out
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class ShortRangeDetector : MonoBehaviour
	{
		[Header("Detection method")]
		[SerializeField] bool onTriggerEnter = true;
		[SerializeField] bool onTriggerStay = false;
		[SerializeField] bool onTriggerExit = false;

		Collider col;
		void Awake() => col = GetComponent<Collider>();

		//Seems units need higher priority
		void OnTriggerEnter(Collider other)
		{
			if (!onTriggerEnter) return;

			MonoBehaviour hit = other.GetComponent<MonoBehaviour>();
			switch (hit)
			{
				case Unit u:
					// print("SRD: Unit hit!");
					SendMessageUpwards("OnUnitTouch", u);
					break;

				case Ball b:
					// print("SRD: Ball hit!");
					SendMessageUpwards("OnBallTouch");
					break;
			}
		}

		void OnTriggerStay(Collider other)
		{
			if (!onTriggerStay) return;

			MonoBehaviour hit = other.GetComponent<MonoBehaviour>();
			switch (hit)
			{
				case Unit u:
					// print("SRD: Unit hit!");
					SendMessageUpwards("OnUnitTouch", u);
					break;

				case Ball b:
					// print("SRD: Ball hit!");
					SendMessageUpwards("OnBallTouch");
					break;
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (!onTriggerExit) return;

			MonoBehaviour hit = other.GetComponent<MonoBehaviour>();
			switch (hit)
			{
				case Unit u:
					// print("SRD: Unit hit!");
					SendMessageUpwards("OnUnitTouch", u);
					break;

				case Ball b:
					// print("SRD: Ball hit!");
					SendMessageUpwards("OnBallTouch");
					break;
			}
		}
	}
}