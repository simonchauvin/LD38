using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public float unzoomSpeed;
    public float followSpeed;
    public float followHeightSpeed;
    public float rotationSpeed;
    public float smoothTime;
    public float minDistanceHeight;
    public float maxDistanceHeight;
    public float minDistance;
    public float maxDistance;

    private bool hasStarted;


    void Start ()
    {
        hasStarted = false;
    }

    public void init ()
    {
        transform.position = Player.instance.getFrontPosition() + new Vector3(0f, minDistanceHeight, 0f);
        hasStarted = true;
    }
	
	void Update ()
    {
        if (hasStarted)
        {
            // Follow player
            Vector3 playerPos = Player.instance.getFrontPosition();
            Vector3 direction = playerPos - transform.position;
            Vector3 planarDirection = new Vector3(direction.x, 0f, direction.z).normalized;
            Vector3 playerPlanarDirection = new Vector3(direction.x, playerPos.y, direction.z);
            if (playerPlanarDirection.magnitude >= maxDistance)
            {
                transform.position += planarDirection * followSpeed * Time.deltaTime;
            }
            else if (playerPlanarDirection.magnitude < minDistance)
            {
                transform.position -= planarDirection * followSpeed * Time.deltaTime;
            }

            if (transform.position.y - playerPos.y < minDistanceHeight)
            {
                transform.position += new Vector3(0f, followHeightSpeed * Time.deltaTime, 0f);
            }
            else if (transform.position.y - playerPos.y >= maxDistanceHeight)
            {
                transform.position -= new Vector3(0f, followHeightSpeed * Time.deltaTime, 0f);
            }

            // Looking at the player
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);

            // Unzooming
            //target.position = new Vector3(transform.position.x, transform.position.y + unzoomSpeed * Time.deltaTime, transform.position.z);
        }
    }
}
