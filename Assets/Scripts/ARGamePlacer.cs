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
		[SerializeField] GameObject gameAssembly;

		public UnityEvent onAssemblyPlaced;

		//Members
		bool assemblyPlaced = false;
		bool assemblyCanBePlaced = false;
		Pose placementPose;
		ARRaycastManager raycastManager;
		Camera cam;

		void Awake()
		{
			raycastManager = FindObjectOfType<ARRaycastManager>();
			Debug.Assert(raycastManager is object, "AR Raycast manager not found");
			cam = FindObjectOfType<Camera>();
		}

		void Update()
		{
			if (assemblyPlaced) return;

			//Hover
			var screenCenter = cam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
			if (TryGetPoseAtScreenPos(out Pose pose, screenCenter))
			{
				gameAssembly.SetActive(true);
				gameAssembly.transform.SetPositionAndRotation(pose.position, pose.rotation);

				assemblyCanBePlaced = true;

				//Place assembly
				// if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
				// 	PlaceAssembly();
			}
			else
			{
				//Move it behind the camera
				var behindCamera = cam.transform.forward * -10f; //10 metres behind the camera should do it

				assemblyCanBePlaced = false;
			}
		}

		public void PlaceAssembly()
		{
			if (assemblyCanBePlaced)
				assemblyPlaced = true;

			onAssemblyPlaced.Invoke();
		}

		public void ReleaseAssembly()
		{
			assemblyPlaced = false;
		}

		List<ARRaycastHit> arRayHits = new List<ARRaycastHit>();
		bool TryGetPoseAtScreenPos(out Pose pose, Vector3 screenPos, TrackableType trackableType = TrackableType.Planes)
		{
			pose = Pose.identity;

			arRayHits.Clear();
			raycastManager.Raycast(screenPos, arRayHits, trackableType);
			if (arRayHits.Count > 0)
			{
				pose = arRayHits[0].pose;
				var camForward = cam.transform.forward;
				var camBearing = new Vector3(camForward.x, 0, camForward.z).normalized;
				pose.rotation = Quaternion.LookRotation(camBearing);
				return true;
			}
			return false;
		}
	}
}