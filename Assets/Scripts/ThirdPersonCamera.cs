using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public float unzoomSpeed;
    public float unzoomFactor;
    public float followSpeed;
    public float followHeightSpeed;
    public float rotationSpeed;
    public float startMinDistance;
    public float startMinDistanceHeight;
    public Vector2 rotationRange;

    private Quaternion originalRotation;
    private Vector3 followAngles;
    private Vector3 followVelocity;
    private bool hasStarted;
    private float minDistance;
    private float minDistanceHeight;


    void Start ()
    {
        hasStarted = false;
    }

    public void init ()
    {
        minDistance = startMinDistance * Player.instance.maxSize;
        minDistanceHeight = startMinDistanceHeight * Player.instance.maxSize;

        transform.position = Player.instance.getFrontPosition() + new Vector3(minDistance, minDistanceHeight, minDistance);
        transform.rotation = Quaternion.LookRotation(Player.instance.getFrontPosition() - transform.position);
        originalRotation = transform.rotation;
        hasStarted = true;
    }
	
	void Update ()
    {
        if (hasStarted)
        {
            // Follow player
            Vector3 playerPos = Player.instance.getInterpolatedFrontPosition();
            Vector3 direction = playerPos - transform.position;
            Debug.DrawRay(transform.position, direction, Color.red);
            Vector3 planarDirection = new Vector3(direction.x, 0f, direction.z);
            Debug.DrawRay(transform.position, planarDirection, Color.blue);
            
            transform.position = Vector3.Lerp(transform.position, playerPos - planarDirection.normalized * minDistance + new Vector3(0f, minDistanceHeight, 0f), Time.deltaTime * followSpeed);

            // Looking at the player
            lookAt(playerPos);

            // Unzooming
            //target.position = new Vector3(transform.position.x, transform.position.y + unzoomSpeed * Time.deltaTime, transform.position.z);
            //minDistanec *= unzoomspeed * time.deltatime;
        }
    }

    protected void lookAt (Vector3 playerPos)
    {
        // we make initial calculations from the original local rotation
        transform.localRotation = originalRotation;

        // tackle rotation around Y first
        Vector3 localTarget = transform.InverseTransformPoint(playerPos);
        float yAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

        yAngle = Mathf.Clamp(yAngle, -rotationRange.y * 0.5f, rotationRange.y * 0.5f);
        transform.localRotation = originalRotation * Quaternion.Euler(0, yAngle, 0);

        // then recalculate new local target position for rotation around X
        localTarget = transform.InverseTransformPoint(playerPos);
        float xAngle = Mathf.Atan2(localTarget.y, localTarget.z) * Mathf.Rad2Deg;
        xAngle = Mathf.Clamp(xAngle, -rotationRange.x * 0.5f, rotationRange.x * 0.5f);
        var targetAngles = new Vector3(followAngles.x + Mathf.DeltaAngle(followAngles.x, xAngle),
                                       followAngles.y + Mathf.DeltaAngle(followAngles.y, yAngle));

        // smoothly interpolate the current angles to the target angles
        followAngles = Vector3.SmoothDamp(followAngles, targetAngles, ref followVelocity, rotationSpeed);


        // and update the gameobject itself
        transform.localRotation = originalRotation * Quaternion.Euler(-followAngles.x, followAngles.y, 0);
    }

    public void updateZoom ()
    {
        minDistance *= unzoomFactor;
        minDistanceHeight *= unzoomFactor;
    }
}
