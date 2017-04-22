using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link : MonoBehaviour
{
    private Rigidbody thisRigidbody;
    private HingeJoint thisJoint;


	void Awake ()
    {
        thisRigidbody = GetComponent<Rigidbody>();
        thisJoint = GetComponent<HingeJoint>();
    }
	
	void Update ()
    {
		
	}

    public void connect (Rigidbody linkRigidbody)
    {
        thisJoint.connectedBody = linkRigidbody;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Link"))
        {
            Player.instance.addNewLink(collision.collider.GetComponent<Link>());
        }
    }
}
