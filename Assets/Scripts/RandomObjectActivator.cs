using UnityEngine;

namespace LeMinhHuy
{
	public class RandomObjectActivator : MonoBehaviour
	{
		[SerializeField] bool randomizeOnStart = true;
		[SerializeField] GameObject[] objects;

		void Start()
		{
			if (randomizeOnStart) ActivateRandom();
		}

		public void ActivateRandom()
		{
			//Deactivate all
			foreach (var o in objects)
			{
				o.SetActive(false);
			}
			//Activate a random one
			objects[UnityEngine.Random.Range(0, objects.Length)].SetActive(true);
		}
	}
}