using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public float followSpeed;
    public float minDistance;
    public float maxDistance;

    private Transform playerHead;


	void Start ()
    {
        playerHead = Player.instance.getHead();
    }
	
	void Update ()
    {
        Vector3 direction = playerHead.position - transform.position;
        if (direction.magnitude > maxDistance)
        {
            transform.position += new Vector3(direction.normalized.x, 0f, direction.normalized.z) * followSpeed * Time.deltaTime;
        }
        else if (direction.magnitude < minDistance)
        {
            transform.position -= new Vector3(direction.normalized.x, 0f, direction.normalized.z) * followSpeed * Time.deltaTime;
        }

        transform.LookAt(playerHead);
    }
}
