using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode setCheckpointKey = KeyCode.F;

    [Header("Checkpoint")]
    public Checkpoint currentCheckpoint;
	[SerializeField] float teleportDelay;

	[Header("References")]
	[SerializeField] PlayerMovement playerMovement;
	[SerializeField] MouseLook mouseLook;
	[SerializeField] WallRun wallRun;
	[SerializeField] Transform startOfLevel;

    public bool canSetCheckpoint = true;
   
    void Update()
    {
		if (Input.GetKeyDown(setCheckpointKey) && canSetCheckpoint)
		{
            CheckpointManager.Instance.SetNewCheckpoint();
		}
    }

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Void"))
		{
			StartCoroutine(Die());
		}
	}

	IEnumerator Die()
	{
		playerMovement.enabled = false;
		mouseLook.enabled = false;
		wallRun.enabled = false;

		yield return new WaitForSeconds(teleportDelay);

		if(currentCheckpoint != null)
			transform.position = currentCheckpoint.transform.position;
		else
			transform.position = startOfLevel.position;


		playerMovement.enabled = true;
		mouseLook.enabled = true;
		yield return new WaitForSeconds(0.2f);
		wallRun.enabled = true;
	}
}
