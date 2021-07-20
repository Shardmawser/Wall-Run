using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("General")]
    public string checkpointName;
	

	[Header("Player detection")]
	[SerializeField] float checkpointSize;
	[SerializeField] LayerMask player;

	[HideInInspector] public bool canSetCheckpoint;

	private void Update()
	{
		canSetCheckpoint = Physics.CheckSphere(transform.position, checkpointSize, player);


		if (canSetCheckpoint)
		{
			SetCheckPoint();
		}

	}

	void SetCheckPoint()
	{
			CheckpointManager.Instance.canSetCheckpoint = true;
			if (canSetCheckpoint)
				CheckpointManager.Instance.SetPlayerCheckPoint(checkpointName);
			else
				CheckpointManager.Instance.RemovePlayerCheckpoint();
			Debug.Log("Can we set a checkpoint?: " + canSetCheckpoint);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, checkpointSize);
	}
}
