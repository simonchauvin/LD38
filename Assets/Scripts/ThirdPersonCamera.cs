using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public float unzoomSpeed;
    public float followSpeed;
    public float followHeightSpeed;
    public float minDistanceHeight;
    public float minDistance;
    public float maxDistance;

    private Transform blobsCam;

    private bool hasStarted;


	void Start ()
    {
        blobsCam = GameObject.Find("BlobsCamera").transform;

        hasStarted = false;
    }

    public void init ()
    {
        transform.position = Player.instance.getFirstHeadPosition() + new Vector3(0f, minDistanceHeight, 0f);
        hasStarted = true;
    }
	
	void Update ()
    {
        if (hasStarted)
        {
            // Follow player
            Vector3 direction = Player.instance.getHeadsBarycenter() - transform.position;
            if (new Vector3(direction.x, 0f, direction.z).magnitude > maxDistance)
            {
                transform.position += new Vector3(direction.normalized.x, 0f, direction.normalized.z) * followSpeed * Time.deltaTime;
            }
            else if (new Vector3(direction.x, 0f, direction.z).magnitude < minDistance)
            {
                transform.position -= new Vector3(direction.normalized.x, 0f, direction.normalized.z) * followSpeed * Time.deltaTime;
            }

            if (transform.position.y - direction.y < minDistanceHeight)
            {
                transform.position += new Vector3(0f, followHeightSpeed * Time.deltaTime, 0f);
            }

            // Looking at the player
            transform.LookAt(Player.instance.getHeadsBarycenter());

            // Unzooming
            transform.position = new Vector3(transform.position.x, transform.position.y + unzoomSpeed * Time.deltaTime, transform.position.z);

            // Update blobs camera
            blobsCam.position = transform.position;
            blobsCam.rotation = transform.rotation;
        }
    }
}
