using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace LeMinhHuy
{
	/// <summary>
	/// Controls whether to zoom
	/// </summary>
	public class ARZoomController : MonoBehaviour
	{
		//Members
		ARSessionOrigin arOrigin;

		void Start()
		{
			arOrigin = FindObjectOfType<ARSessionOrigin>();
		}

		//Touch zoom
		[SerializeField] GameObject subject;
		Touch[] touchStart = new Touch[2];
		Vector2 touchStartCenter;
		float touchStartDistance;
		float touchZoomScaleMult = 0.25f;

		void ControlScaleAndRotation()
		{
			if (Input.touchCount >= 2)
			{
				//Start
				if (Input.GetTouch(0).phase == TouchPhase.Began)
				{
					touchStart[0] = Input.GetTouch(0);
					touchStart[1] = Input.GetTouch(1);
				}

				//Initial dist
				if (Input.GetTouch(0).phase == TouchPhase.Stationary)
				{
					touchStartDistance = Vector2.Distance(touchStart[0].position, touchStart[1].position);
				}

				//Zoom
				if (Input.GetTouch(0).phase == TouchPhase.Moved)
				{
					var newTouchDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
					var difference = touchStartDistance - newTouchDistance;
					subject.transform.localScale *= difference * touchZoomScaleMult;
				}
			}
		}
	}
}