using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
	[Header("References")]
	[SerializeField] GameObject checkpoint;

	[Header("Player detection")]
	[SerializeField] float checkpointSize;
	[SerializeField] LayerMask player;

	bool canSetCheckpoint;

	private void Update()
	{
		canSetCheckpoint = Physics.CheckSphere(transform.position, checkpointSize, player);

		if (canSetCheckpoint)
			checkpoint.SetActive(true);
		else 
			checkpoint.SetActive(false);
	}
}
