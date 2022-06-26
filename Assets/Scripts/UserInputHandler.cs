using LeMinhHuy.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LeMinhHuy
{
	/// <summary>
	/// Handles input etc from the user
	/// </summary>
	public class UserInputHandler : MonoBehaviour
	{
		//Inspector
		Vector2 startingOrbit;
		Quaternion rotation;
		float zoom;

		//Properties
		// public Vector2 position;

		//Events
		TapEvent onTap;

		//Core
		void Update()
		{
			if (Input.touchCount > 0)
			{
				switch (Input.touchCount)
				{
					//Handle taps
					case 1:
						{
							//Let any UI block taps
							if (EventSystem.current.IsPointerOverGameObject(0))
							{
								Debug.Log("Blocked by the UI!");
								return;
							}

							var touch = Input.GetTouch(0);
							if (touch.phase == TouchPhase.Began)
							{
								onTap.Invoke(touch.position);
							}
						}
						break;

					//Handle view rotate/zoom
					case 2:
						{
							// if (Input.GetTouch(0).phase)
						}
						break;

					default:
						return;
				}
			}
		}
	}
}