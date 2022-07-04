using UnityEngine;
using UnityEngine.SceneManagement;

namespace LeMinhHuy
{
	public class LoadSceneOnStart : MonoBehaviour
	{
		[SerializeField] string sceneName = "Main";

		void Start()
		{
			SceneManager.LoadScene(sceneName);
		}
	}
}