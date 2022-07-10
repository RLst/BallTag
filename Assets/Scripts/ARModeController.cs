using UnityEngine;

namespace LeMinhHuy
{
	/// <summary>
	/// Activates or deactivates the AR toggle button based on user choice
	/// </summary>
	public class ARModeController : MonoBehaviour
	{
		[SerializeField] bool enableARSwitching = true;
		[SerializeField] GameObject arToggleButton;

		void Start()
		{
			arToggleButton.gameObject.SetActive(enableARSwitching);
		}
	}
}