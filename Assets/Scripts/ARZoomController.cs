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
	}
}