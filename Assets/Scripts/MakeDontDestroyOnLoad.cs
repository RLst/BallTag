using UnityEngine;

namespace LeMinhHuy
{
	public class MakeDontDestroyOnLoad : MonoBehaviour
	{
		void Awake()
		{
			DontDestroyOnLoad(this);
		}
	}
}