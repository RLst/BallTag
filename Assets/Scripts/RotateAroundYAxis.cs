using UnityEngine;

namespace LeMinhHuy
{
	//This just spins this at a set speed when it's active
	public class RotateAroundYAxis : MonoBehaviour
	{
		[SerializeField] float rotationSpeed = 120f;

		void Update()
		{
			this.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
		}
	}
}