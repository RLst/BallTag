using System;
using UnityEngine;

namespace LeMinhHuy
{
	[RequireComponent(typeof(MazeBuilder))]
	public class Stadium : Singleton<Stadium>
	{
		[SerializeField] Transform mazeOrigin;
		public Transform frontOfGoal;   //TEMP

		MazeBuilder mb;

		void Awake()
		{
			mb = GetComponent<MazeBuilder>();
		}

		internal void GenerateMaze()
		{
			mazeOrigin.localPosition = Vector3.zero;
			mb.Build(mazeOrigin);
			mazeOrigin.localPosition = new Vector3(-1.62f, 0, -1.75f);
			mazeOrigin.localScale = new Vector3(2.5f, 1, 2.6f);
		}
		internal void DeleteMaze()
		{
			for (int i = 0; i < mazeOrigin.childCount; i++)
				Destroy(mazeOrigin.GetChild(i).gameObject);
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				GenerateMaze();
			}
			if (Input.GetKeyDown(KeyCode.D))
			{
				DeleteMaze();
			}
		}
	}
}