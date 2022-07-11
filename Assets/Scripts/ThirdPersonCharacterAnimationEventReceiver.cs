using UnityEngine;
using UnityEngine.Events;

namespace LeMinhHuy
{
	public class ThirdPersonCharacterAnimationEventReceiver : MonoBehaviour
	{
		public UnityEvent onStep;
		public void OnStep()
		{
			onStep.Invoke();
		}
	}
}