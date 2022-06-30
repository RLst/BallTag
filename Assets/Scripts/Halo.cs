using UnityEngine;

namespace LeMinhHuy
{
	//This just spins this at a set speed when it's active
	public class Halo : MonoBehaviour
	{
		[SerializeField] float speed = 0.3f;
		float currentAngle = 0;

		void Update()
		{
			this.transform.Rotate(Vector3.up, currentAngle += speed);
		}
	}
}