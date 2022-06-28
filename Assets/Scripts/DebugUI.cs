using UnityEngine;

namespace LeMinhHuy
{
	public class DebugUI : MonoBehaviour
	{
		void Awake()
		{
			DontDestroyOnLoad(this);
		}
	}
}