
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
		void Awake()
		{
			col = GetComponent<Collider>();
		}

		void OnTriggerEnter(Collider other)
		{
			if (!onTriggerEnter) return;

			MonoBehaviour hit = other.GetComponent<Ball>();
			if (hit is Ball)
			{
				SendMessageUpwards("OnBallTouch");
				return;
			}
			hit = other.GetComponent<Unit>();
			if (hit is Unit)
			{
				SendMessageUpwards("OnUnitTouch", hit);
				return;
			}
		}

		void OnTriggerStay(Collider other)
		{
			if (!onTriggerStay) return;

			MonoBehaviour hit = other.GetComponent<Ball>();
			if (hit is Ball)
			{
				SendMessageUpwards("OnBallTouch");
				return;
			}
			hit = other.GetComponent<Unit>();
			if (hit is Unit)
			{
				SendMessageUpwards("OnUnitTouch", hit);
				return;
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (!onTriggerExit) return;

			MonoBehaviour hit = other.GetComponent<Ball>();
			if (hit is Ball)
			{
				SendMessageUpwards("OnBallTouch");
				return;
			}
			hit = other.GetComponent<Unit>();
			if (hit is Unit)
			{
				SendMessageUpwards("OnUnitTouch", hit);
				return;
			}
		}
	}
}