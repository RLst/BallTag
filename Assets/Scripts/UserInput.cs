using LeMinhHuy.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LeMinhHuy
{
	/// <summary>
	/// Handles input etc from the user
	/// </summary>
	public class UserInput : Singleton<UserInput>
	{
		//Inspector
		Vector2 startingOrbit;
		Quaternion rotation;
		float zoom;

		//Properties
		// public Vector2 position;

		//Events
		public PointInputEvent onScreenPosInput;

		//Core
		void Update()
		{
			if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
			{
				//Touch
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
								onScreenPosInput.Invoke(touch.position);
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
						//Probably a mouse click then
						break;
				}

				//Mouse
				onScreenPosInput.Invoke(Input.mousePosition);
			}
		}
	}
}