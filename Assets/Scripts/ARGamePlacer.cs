using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
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
		[SerializeField] ARPlaneManager arPlane;
		[SerializeField] Vector3 outOfViewPosition = new Vector3(0, 1000f, 0);

		[Header("UI")]
		[SerializeField] GameObject arUIAssembly;   //The canvas that is only specific to the AR scene
		[SerializeField] TextMeshProUGUI infoText;
		[SerializeField] Image scanningTint;
		[SerializeField] Color okColor = Color.green;
		[SerializeField] Color notOkColor = Color.red;

		[Space]
		public UnityEvent onAssemblyPlaced;

		//Members
		bool assemblyPlaced = false;
		bool assemblyCanBePlaced = false;
		ARRaycastManager arRaycaster;
		Camera cam;
		List<ARRaycastHit> arRayHits = new List<ARRaycastHit>();

		void Awake()
		{
			arRaycaster = FindObjectOfType<ARRaycastManager>();
			cam = FindObjectOfType<Camera>();

			Debug.Assert(gameAssembly is object, "Game Assembly Object required");
			Debug.Assert(arRaycaster is object, "AR Raycast manager not found");
			Debug.Assert(arPlane is object, "AR Plane manager not found");
			Debug.Assert(arUIAssembly is object, "AR Canvas not found");
		}

		void Start()
		{
			assemblyPlaced = false;

			//Prevent game from starting etc
			MainMenuController.current.SetActiveTapToStart(false);
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

				infoText.text = "Tap to place the game";
				scanningTint.color = okColor;

				//Hover
				gameAssembly.transform.SetPositionAndRotation(pose.position, pose.rotation);

				//Place assembly
				// if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
				// 	PlaceAssembly();
			}
			else
			{
				assemblyCanBePlaced = false;

				infoText.text = "Point camera towards a non-reflective, textured and level surface";
				scanningTint.color = notOkColor;

				//Move game far away out of view (can't deactivate it because it screws it up)
				gameAssembly.transform.position = outOfViewPosition;
			}
		}

		//Hook this up to some button or tap sensor
		public void PlaceAssembly()
		{
			if (!assemblyCanBePlaced)
				return;

			assemblyPlaced = true;  //Stop trying to find surfaces

			//Hide any AR specific UIs
			arUIAssembly.SetActive(false);

			//Re-enable and resume normal main menu functions
			MainMenuController.current.SetActiveTapToStart(true);

			//Disable plane manager so the dots don't keep showing up
			arPlane.enabled = false;

			onAssemblyPlaced.Invoke();
		}
		public void ReleaseAssembly() => assemblyPlaced = false;


		//UTILS
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
			arRaycaster.Raycast(screenPos, arRayHits, trackableType);
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