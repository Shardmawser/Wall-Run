using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

	private void Awake()
	{
		Instance = this;
	}

	[Header("Checkpoints")]
    [SerializeField] Checkpoint[] checkpoints;
	public Checkpoint currentCheckpoint;

	[Header("References")]
	[SerializeField] PlayerController playerController;

	bool removeCurrentCheckpoint;
	public bool canSetCheckpoint;


	private void Update()
	{
		if (removeCurrentCheckpoint)
		{
			currentCheckpoint = null;
		}

		foreach(Checkpoint checkpoint in checkpoints)
		{
			if (checkpoint.gameObject.activeSelf)
			{
				playerController.canSetCheckpoint = true;
				break;
			} else
			{
				playerController.canSetCheckpoint = false;
			}
		}
	}

	public void SetPlayerCheckPoint(string name)
	{
		removeCurrentCheckpoint = false;
		foreach(Checkpoint checkpoint in checkpoints)
		{
			if (checkpoint.checkpointName ==  name)
			{
				currentCheckpoint = checkpoint;
			}
		}

	}

	public void RemovePlayerCheckpoint()
	{
		removeCurrentCheckpoint = true;
	}

	public void SetNewCheckpoint()
	{
		playerController.currentCheckpoint = currentCheckpoint;
	}


}
