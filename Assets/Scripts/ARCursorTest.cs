using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace LeMinhHuy
{

	public class ARCursorTest : MonoBehaviour
	{
		public GameObject cursorChildObject;
		public GameObject placeObject;
		public ARRaycastManager raycastManager;

		[SerializeField] bool useCursor = true;
		List<ARRaycastHit> arRayHits = new List<ARRaycastHit>();

		void Start()
		{
			cursorChildObject.SetActive(useCursor);
			// planeManager.planesChanged += test;
		}
		// void test(ARPlanesChangedEventArgs args) { }

		void Update()
		{
			//Position the cursor
			if (useCursor)
			{
				PositionCursorAtCenterRay();
			}

			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
			{
				//If screen touched then place object at cursor
				if (useCursor)
				{
					GameObject.Instantiate(placeObject, transform.position, transform.rotation);
				}
				//Place object at point of touch
				else
				{
					var touchPoint = GetPoseAtScreenPos(Input.GetTouch(0).position);
					if (touchPoint.HasValue)
					{
						GameObject.Instantiate(placeObject, touchPoint.Value.position, touchPoint.Value.rotation);
					}
				}
			}
		}

		void PositionCursorAtCenterRay()
		{
			var arRaycastHitPose = GetPoseAtScreenPos(Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f)));
			if (arRaycastHitPose.HasValue)
			{
				transform.position = arRaycastHitPose.Value.position;
				transform.rotation = arRaycastHitPose.Value.rotation;
			}
		}

		Pose? GetPoseAtScreenPos(Vector3 screenPos)
		{
			arRayHits.Clear();
			raycastManager.Raycast(screenPos, arRayHits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);
			if (arRayHits.Count > 0)
			{
				return arRayHits[0].pose;
			}
			return null;
		}

		public void ToggleCursor()
		{
			useCursor = !useCursor;
		}
	}
}