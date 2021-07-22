using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[Header("References")]
	[SerializeField] PlayerMovement playerMovement;
	[SerializeField] MouseLook mouseLook;
	[SerializeField] WallRun wallRun;
	[SerializeField] Transform startOfLevel;
    
	[Header("Keybinds")]
    public KeyCode setCheckpointKey = KeyCode.F;

    [Header("Checkpoint")]
    public Checkpoint currentCheckpoint;
	[SerializeField] float respawnDelay;
    public bool canSetCheckpoint = true;

	[Header("Dying")]
	[SerializeField] float deathHeight;
	bool died = false;


   
    void Update()
    {
		if (Input.GetKeyDown(setCheckpointKey) && canSetCheckpoint)
		{
            CheckpointManager.Instance.SetNewCheckpoint();
		}

		if(transform.position.y <= deathHeight && !died)
		{
			Die();
		}
    }

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Void"))
		{
			Die();
		}
	}



	void Die()
	{
		died = true;
		playerMovement.enabled = false;
		mouseLook.enabled = false;
		wallRun.enabled = false;

		StartCoroutine(Respawn());
	}

	IEnumerator Respawn()
	{
		yield return new WaitForSeconds(respawnDelay);

		if (currentCheckpoint != null)
			transform.position = currentCheckpoint.transform.position;
		else
			transform.position = startOfLevel.position;


		playerMovement.enabled = true;
		mouseLook.enabled = true;
		died = false;
		yield return new WaitForSeconds(0.2f);
		wallRun.enabled = true;
		
	}
}
