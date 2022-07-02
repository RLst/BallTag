using UnityEngine;

namespace LeMinhHuy
{
	public class MazeBuilder : MonoBehaviour
	{
		[SerializeField][Range(1, 50)] int width = 10;
		[SerializeField][Range(1, 50)] int height = 10;
		[SerializeField] float size = 1f;
		[SerializeField] float scale = 2f;
		[SerializeField] Transform wallPrefab = null;

		public void Build(Transform origin = null)
		{
			if (origin is null)
				origin = this.transform;

			var maze = MazeGenerator.Generate(width, height);
			Build(maze, origin);
		}
		public void Build(WallState[,] maze, Transform origin)
		{
			for (int i = 0; i < width; ++i)
			{
				for (int j = 0; j < height; ++j)
				{
					var cell = maze[i, j];
					var position = new Vector3(-width * 0.5f * scale + i, 0, -height * 0.5f * scale + j);

					if (cell.HasFlag(WallState.UP))
					{
						var topWall = Instantiate(wallPrefab, origin) as Transform;
						topWall.position = position + new Vector3(0, 0, size * 0.5f);
						// topWall.localScale = new Vector3(size, topWall.localScale.y, topWall.localScale.z);
					}

					if (cell.HasFlag(WallState.LEFT))
					{
						var leftWall = Instantiate(wallPrefab, origin) as Transform;
						leftWall.position = position + new Vector3(-size * 0.5f, 0, 0);
						// leftWall.localScale = new Vector3(size, leftWall.localScale.y, leftWall.localScale.z);
						leftWall.eulerAngles = new Vector3(0, 90, 0);
					}

					if (i == width - 1)
					{
						if (cell.HasFlag(WallState.RIGHT))
						{
							var rightWall = Instantiate(wallPrefab, origin) as Transform;
							rightWall.position = position + new Vector3(+size * 0.5f, 0, 0);
							// rightWall.localScale = new Vector3(size, rightWall.localScale.y, rightWall.localScale.z);
							rightWall.eulerAngles = new Vector3(0, 90, 0);
						}
					}

					if (j == 0)
					{
						if (cell.HasFlag(WallState.DOWN))
						{
							var bottomWall = Instantiate(wallPrefab, origin) as Transform;
							bottomWall.position = position + new Vector3(0, 0, -size * 0.5f);
							// bottomWall.localScale = new Vector3(size, bottomWall.localScale.y, bottomWall.localScale.z);
						}
					}
				}

			}

		}
	}
}