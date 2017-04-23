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
    public float size { get; private set; }
    public float radius { get; private set; }


    void Awake ()
    {
        thisRigidbody = GetComponent<Rigidbody>();
        thisRigidbody.mass = mass;
        thisRigidbody.drag = drag;
        thisRigidbody.angularDrag = angularDrag;

        grounded = false;
        index = 0;
    }

    void Start()
    {
        
    }

    public void init (int index, float size)
    {
        this.index = index;
        this.size = size;
        this.radius = size / 2f;
        transform.localScale = new Vector3(size, size, size);
    }

    void Update ()
    {
        
    }

    private void FixedUpdate()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, -transform.up, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            if (hitInfo.distance - radius < Player.instance.minGroundDistance)
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

    public void OnTriggerEnter (Collider collider)
    {
        if (collider.CompareTag("CollectibleLink"))
        {
            Player.instance.addNewLink(collider.transform);
        }
    }
}
