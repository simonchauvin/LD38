using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player _instance;
    public static Player instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = GameObject.Find("Player").GetComponent<Player>();
			}
			return _instance;
		}
	}

    public float timeBetweenMove;
    public float rotationSpeed;
    public float startingForceMultiplier;
    public float forceMultiplierIncrease;
    public float sizeMultiplier;
    public float sizeChangeTime;
    public float minGroundDistance;
    public Link linkPrefab;

    private ThirdPersonCamera cam;
    private Link[] links;
    private List<Link> collectibleLinks;
    private Transform front;

    private float linkRadius;
    private float lastMoveTime;
    private float forceMultiplier;
    private bool sizeChange;
    private float originalSize;
    private float targetSize;
    private float sizeChangeTimer;
    private bool canStart;


    private void Awake()
    {
        cam = GameObject.FindObjectOfType<ThirdPersonCamera>();
        links = new Link[1];
        collectibleLinks = new List<Link>();
        front = new GameObject("Front").transform;
        front.parent = transform;

        links[0] = Instantiate(linkPrefab, transform.position, Quaternion.identity, transform);
        linkRadius = links[0].GetComponent<SphereCollider>().bounds.size.z;
        front.position = links[0].transform.position;

        Transform linksFolder = GameObject.Find("Links").transform;
        for (int i = 0; i < linksFolder.childCount; i++)
        {
            collectibleLinks.Add(linksFolder.GetChild(i).GetComponent<Link>());
        }

        lastMoveTime = 0f;
        forceMultiplier = startingForceMultiplier;
        sizeChange = false;
        originalSize = 1f;
        targetSize = 1f;
        sizeChangeTimer = 0f;
        canStart = false;
    }

    void Start ()
    {
        
	}
	
	void Update ()
    {
        if (canStart)
        {
            // Debug
            if (Input.GetKey(KeyCode.F))
            {
                addNewLink(Instantiate(linkPrefab));
            }

            // Size increase
            /*if (sizeChange)
            {
                float size = Mathf.Lerp(originalSize, targetSize, sizeChangeTimer / sizeChangeTime);
                head.transform.localScale = new Vector3(size, size, size);
                float oldRadius = linkRadius;
                linkRadius *= size;

                for (int i = 0; i < links.Count; i++)
                {
                    links[i].transform.localScale = new Vector3(size, size, size);
                    if (i - 1 >= 0)
                    {
                        links[i].transform.position -= (links[i - 1].transform.position - links[i].transform.position).normalized * (linkRadius - oldRadius);
                    }
                    else
                    {
                        links[i].transform.position -= (head.transform.position - links[i].transform.position).normalized * (linkRadius - oldRadius);
                    }
                }
                sizeChangeTimer += Time.deltaTime;

                if (sizeChangeTimer >= sizeChangeTime)
                {
                    sizeChange = false;
                }
            }*/

            front.position = links[0].transform.position;
        }
        else
        {
            canStart = true;
            for (int i = 0; i < links.Length; i++)
            {
                links[i].init();
            }
            for (int i = 0; i < collectibleLinks.Count; i++)
            {
                collectibleLinks[i].init();
            }
            front.position = links[0].transform.position;
            cam.init();
        }
    }

    private void FixedUpdate()
    {
        if (canStart)
        {
            // Heads movement
            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            front.Rotate(front.transform.up, input.x * rotationSpeed);

            // Body movement
            if (input.y > 0)
            {
                if (Time.time - lastMoveTime >= timeBetweenMove)
                {
                    if (links.Length > 1)
                    {
                        Link last = links[links.Length - 1];
                        last.removeJoint();
                        for (int i = links.Length - 1; i > 0; i--)
                        {
                            links[i] = links[i - 1];
                            links[i].init(i);
                        }
                        links[0] = last;
                        links[0].init(0);

                        //links[0].thisRigidbody.MovePosition(links[1].transform.position + linkRadius * front.TransformDirection(new Vector3(0f, 0f, input.y)).normalized);
                        links[0].transform.position = links[1].transform.position + linkRadius * front.TransformDirection(new Vector3(0f, 0f, input.y)).normalized;

                        links[1].addJoint();
                        links[1].connect(links[0]);
                    }
                    else
                    {
                        //links[0].thisRigidbody.MovePosition(links[0].transform.position + linkRadius * front.TransformDirection(new Vector3(0f, 0f, input.y)).normalized);
                        links[0].transform.position = links[0].transform.position + linkRadius * front.TransformDirection(new Vector3(0f, 0f, input.y)).normalized;
                    }

                    lastMoveTime = Time.time;
                }
            }
        }
    }

    public void addNewHead(Link newHead)
    {
        // TODO
        newHead.tag = "Head";
    }

    public void addNewLink (Link newLink)
    {
        newLink.tag = "Link";
        newLink.init(links.Length + 1);
        newLink.addJoint();
        newLink.transform.parent = transform;
        if (links.Length > 1)
        {
            Vector3 dir = (links[links.Length - 2].transform.position - links[links.Length - 1].transform.position).normalized;
            dir.y = 0f;
            newLink.transform.position = links[links.Length - 1].transform.position - dir * linkRadius;
        }
        else
        {
            Vector3 dir = links[0].transform.forward;
            dir.y = 0f;
            newLink.transform.position = links[0].transform.position - dir * linkRadius;
        }
        newLink.connect(links[links.Length - 1]);

        Link[] newLinks = new Link[links.Length + 1];
        for (int i = 0; i < links.Length; i++)
        {
            newLinks[i] = links[i];
        }
        newLinks[newLinks.Length - 1] = newLink;
        links = newLinks;
        collectibleLinks.Remove(newLink);

        // Update size
        sizeChange = true;
        sizeChangeTimer = 0f;
        originalSize = links[0].transform.localScale.x;
        targetSize = originalSize * sizeMultiplier;
        //head.transform.localScale *= sizeMultiplier;
        //linkRadius *= sizeMultiplier;
        //linkRadius = head.GetComponent<SphereCollider>().bounds.size.z;
        for (int i = 0; i < links.Length; i++)
        {
            //links[i].transform.localScale *= sizeMultiplier;
            //links[i].GetComponent<HingeJoint>().autoConfigureConnectedAnchor = false;
            if (i - 1 >= 0)
            {
                //Vector3 dist = links[i - 1].transform.position - links[i].transform.position;
                //dist.y = 0f;
                //links[i].transform.position -= (dist).normalized * linkRadius;
            }
            else
            {
                //Vector3 dist = head.transform.position - links[i].transform.position;
                //dist.y = 0f;
                //links[i].transform.position -= (dist).normalized * linkRadius;
            }
            //links[i].GetComponent<HingeJoint>().autoConfigureConnectedAnchor = true;
        }
        //increaseCollectiblesSize();

        // Update multipliers
        forceMultiplier += forceMultiplierIncrease;
    }

    public Vector3 getFirstHeadPosition ()
    {
        return front.transform.position;
    }

    public Vector3 getHeadsBarycenter ()
    {
        Vector3 barycenter = Vector3.zero;
        for (int i = 0; i < links.Length; i++)
        {
            barycenter += links[i].transform.position;
        }
        return barycenter / links.Length;
    }

    public void increaseCollectiblesSize ()
    {
        for (int i = 0; i < collectibleLinks.Count; i++)
        {
            collectibleLinks[i].transform.localScale = new Vector3(targetSize, targetSize, targetSize);
        }
    }
}
