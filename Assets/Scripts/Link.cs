using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link : MonoBehaviour
{
    public float mass;
    public float drag;
    public float angularDrag;
    public float strengh;
    public float damper;

    public Rigidbody thisRigidbody { get; private set; }
    private SpringJoint thisJoint;

    private bool grounded;
    private int index;


    void Awake ()
    {
        thisRigidbody = GetComponent<Rigidbody>();
        thisRigidbody.mass = mass;
        thisRigidbody.drag = drag;
        thisRigidbody.angularDrag = angularDrag;

        grounded = false;
        index = 1;
    }

    void Start()
    {
        
    }

    public void init()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            transform.position = hitInfo.point;
        }
    }

    public void init (int index)
    {
        this.index = index;
    }

    void Update ()
    {
        
    }

    private void FixedUpdate()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, -transform.up, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            if (hitInfo.distance < Player.instance.minGroundDistance)
            {
                grounded = true;
                thisRigidbody.drag = drag;
                transform.forward = Vector3.ProjectOnPlane(transform.forward, hitInfo.normal);
            }
            else
            {
                grounded = false;
                thisRigidbody.drag = 0f;
            }
        }
    }

    public void addJoint()
    {
        gameObject.AddComponent<SpringJoint>();
        thisJoint = GetComponent<SpringJoint>();
        thisJoint.anchor = Vector3.zero;
        thisJoint.axis = Vector3.one;
        thisJoint.connectedAnchor = Vector3.zero;
        thisJoint.autoConfigureConnectedAnchor = true;
        thisJoint.connectedBody = null;
        thisJoint.spring = strengh;
        thisJoint.damper = damper;
    }

    public void removeJoint()
    {
        Destroy(thisJoint);
    }

    public void connect (Link link)
    {
        thisJoint.connectedBody = link.thisRigidbody;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (CompareTag("Link"))
        {
            if (collision.collider.CompareTag("CollectibleLink"))
            {
                Player.instance.addNewLink(collision.collider.GetComponent<Link>());
            }
        }
    }
}
