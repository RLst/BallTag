using System.Collections.Generic;
using TMPro;
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

		public TextMeshProUGUI infoText;  //TEMP

		public UnityEvent onAssemblyPlaced;

		//Members
		bool assemblyPlaced = false;
		bool assemblyCanBePlaced = false;
		ARRaycastManager raycastManager;
		Camera cam;
		List<ARRaycastHit> arRayHits = new List<ARRaycastHit>();

		void Awake()
		{
			raycastManager = FindObjectOfType<ARRaycastManager>();
			cam = FindObjectOfType<Camera>();

			Debug.Assert(gameAssembly is object, "Game Assembly Object required");
			Debug.Assert(raycastManager is object, "AR Raycast manager not found");
		}

		void Start()
		{
			assemblyPlaced = false;

			//Prevent game from starting etc
			MainMenuController.current.SetActiveTapToStart(false);

			onAssemblyPlaced.AddListener(OnAssemblyPlaced);
		}

		void Update()
		{
			//Ignore if the assembly has already been placed down in session space
			if (assemblyPlaced) return;

			//Hover assembly over if a surface is found
			HoverAssembly();
		}

		private void HoverAssembly()
		{
			var screenCenter = cam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
			if (TryGetPoseAtScreenPos(out Pose pose, screenCenter))
			{
				assemblyCanBePlaced = true;

				infoText.text = "Tap to place the game down";

				//Hover
				gameAssembly.transform.SetPositionAndRotation(pose.position, pose.rotation);

				//Place assembly
				if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
					PlaceAssembly();
			}
			else
			{
				assemblyCanBePlaced = false;

				infoText.text = "Move camera to a better surface";

				//Move game far away out of view
				var outOfView = Vector3.up * 10f;
				gameAssembly.transform.position = outOfView;
			}
		}

		//Hook this up to some button or tap sensor
		public void PlaceAssembly()
		{
			if (!assemblyCanBePlaced)
				return;

			assemblyPlaced = true;  //Stop

			MainMenuController.current.SetActiveTapToStart(true);

			onAssemblyPlaced.Invoke();
		}
		// public void ReleaseAssembly() => assemblyPlaced = false;

		void OnAssemblyPlaced()
		{
			//Disable plane manager

			//Play particle effects?

		}

		/// <summary>
		///	Get a pose at the specified screen point
		/// </summary>
		/// <param name="pose"></param>
		/// <param name="screenPos">Screen point that will be raycasted out to</param>
		/// <param name="trackableType"></param>
		/// <returns>Returns true if a surface in session space was detected</returns>
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