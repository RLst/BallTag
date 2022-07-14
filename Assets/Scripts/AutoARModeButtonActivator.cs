using UnityEngine;

namespace LeMinhHuy
{
	/// <summary>
	/// Automatically activates the AR switch button based on current platform
	/// </summary>
	public class AutoARModeButtonActivator : MonoBehaviour
	{
		// [SerializeField] bool enableARSwitching = true;
		[SerializeField] GameObject arToggleButton;

		void Awake()
		{
			Debug.Assert(arToggleButton is object, "AR Toggle button not found!");
		}

		void Start()
		{
#if UNITY_IOS
			//Always set
			arToggleButton.gameObject.SetActive(true);
#else
			arToggleButton.gameObject.SetActive(false);
#endif
		}
	}
}