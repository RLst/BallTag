/*  */using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace LeMinhHuy
{
	/// <summary>
	/// Allows the user to place the main game assembly into the scene
	/// </summary>
	public class ARGamePlacer : MonoBehaviour
	{
		[Tooltip("The main game assembly for this AR scene")]
		[SerializeField] GameObject gameAssembly;

		//Members
		bool assemblyPlaced = false;
		bool assemblyCanBePlaced = false;
		ARRaycastManager raycastManager;
		Camera cam;

		void Awake()
		{
			raycastManager = FindObjectOfType<ARRaycastManager>();
			Debug.Assert(raycastManager is object, "AR Raycast manager not found");
			cam = FindObjectOfType<Camera>();
		}

		void Start()
		{
			assemblyPlaced = false;

			//TEMP
			gameAssembly.transform.localScale = Vector3.one * 0.1f;
		}

		void Update()
		{
			//Ignore if the assembly has already been placed down in session space
			if (assemblyPlaced) return;

			//Hover assembly over if a surface is found
			var screenCenter = cam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
			if (TryGetPoseAtScreenPos(out Pose pose, screenCenter))
			{
				assemblyCanBePlaced = true;

				gameAssembly.transform.SetPositionAndRotation(pose.position, pose.rotation);

				//Place assembly
				if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
					PlaceAssembly();
			}
			else
			{
				assemblyCanBePlaced = false;

				//Move it far away out of veiw
				var outOfView = Vector3.up * 1000f;
				gameAssembly.transform.position = outOfView;
			}

			ControlScale();
		}

		//Touch zoom
		Touch[] touchStart = new Touch[2];
		float touchStartDistance;
		float touchZoomScaleMult = 0.5f;
		void ControlScale()
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
					gameAssembly.transform.localScale *= difference * touchZoomScaleMult;
				}
			}
		}

		//Hook this up to some button or tap sensor
		public void PlaceAssembly()
		{
			if (!assemblyCanBePlaced)
				return;

			assemblyPlaced = true;

			MainMenuController.current.SetActiveTapToStart(true);
		}
		public void ReleaseAssembly() => assemblyPlaced = false;


		List<ARRaycastHit> arRayHits = new List<ARRaycastHit>();
		bool TryGetPoseAtScreenPos(out Pose pose, Vector3 screenPos, TrackableType trackableType = TrackableType.PlaneWithinPolygon)
		{
			pose = Pose.identity;

			arRayHits.Clear();
			raycastManager.Raycast(screenPos, arRayHits, trackableType);
			if (arRayHits.Count > 0)
			{
				pose = arRayHits[0].pose;

				//Make the pose face the same bearing as the camera
				var camForward = cam.transform.forward;
				var camBearing = new Vector3(camForward.x, 0, camForward.z).normalized;
				pose.rotation = Quaternion.LookRotation(camBearing);

				return true;
			}
			return false;
		}

		bool TryGetTouchPosition(out Vector2 touchPosition)
		{
			touchPosition = default;
			if (Input.touchCount == 1)
			{
				touchPosition = Input.GetTouch(0).position;
				return true;
			}
			return false;
		}
	}
}