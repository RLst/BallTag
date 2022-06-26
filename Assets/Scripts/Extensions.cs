using UnityEngine;

namespace LeMinhHuy.Utils
{
	public static class Extensions
	{
		public static Vector3 GetRandomPointInsideCollider(this BoxCollider boxCollider)
		{
			Vector3 extents = boxCollider.size / 2f;
			Vector3 point = new Vector3(
				Random.Range(-extents.x, extents.x),
				Random.Range(-extents.y, extents.y),
				Random.Range(-extents.z, extents.z)
			);

			return boxCollider.transform.TransformPoint(point);
		}
	}
}