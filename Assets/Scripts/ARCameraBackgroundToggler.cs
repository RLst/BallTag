using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace LeMinhHuy
{
	[RequireComponent(typeof(ARCameraBackground))]
	public class ARCameraBackgroundToggler : MonoBehaviour
	{
		ARCameraBackground arCamBackground;

		void Awake()
		{
			arCamBackground = GetComponent<ARCameraBackground>();
		}

		public void Toggle()
		{
			arCamBackground.useCustomMaterial = !arCamBackground.useCustomMaterial;
		}
	}
}
