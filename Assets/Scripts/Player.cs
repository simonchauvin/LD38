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
    public float sizeIncreaseFactor;
    public float sizeChangeTime;
    public float minGroundDistance;
    public float maxSize;
    public Link linkPrefab;

    private ThirdPersonCamera cam;
    private Link[] links;
    private List<Link> collectibleLinks;
    private Transform front;

    private float lastMoveTime;
    private float sizeMultiplier;
    private bool sizeChange;
    private float sizeChangeTimer;
    private bool canStart;


    private void Awake()
    {
        lastMoveTime = 0f;
        sizeMultiplier = 1;
        sizeChange = false;
        sizeChangeTimer = 0f;
        canStart = false;

        cam = GameObject.FindObjectOfType<ThirdPersonCamera>();
        links = new Link[1];
        collectibleLinks = new List<Link>();
        front = new GameObject("Front").transform;
        front.parent = transform;

        links[0] = Instantiate(linkPrefab, transform.position, Quaternion.identity, transform);
        links[0].init(0, getSize(0, 1));
        front.position = links[0].transform.position;

        Transform linksFolder = GameObject.Find("Links").transform;
        for (int i = 0; i < linksFolder.childCount; i++)
        {
            collectibleLinks.Add(linksFolder.GetChild(i).GetComponent<Link>());
        }
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

            front.position = links[0].transform.position;
        }
        else
        {
            canStart = true;
            for (int i = 0; i < links.Length; i++)
            {
                links[i].spawn();
            }
            for (int i = 0; i < collectibleLinks.Count; i++)
            {
                collectibleLinks[i].spawn();
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
                            links[i].init(i, getSize(i, links.Length));
                        }
                        links[0] = last;
                        links[0].init(0, getSize(0, links.Length));

                        links[0].transform.position = links[1].transform.position + (links[0].radius + links[1].radius) * front.TransformDirection(new Vector3(0f, 0f, input.y)).normalized;

                        links[1].addJoint();
                        links[1].connect(links[0]);
                    }
                    else
                    {
                        links[0].transform.position += links[0].size * front.TransformDirection(new Vector3(0f, 0f, input.y)).normalized;
                    }

                    lastMoveTime = Time.time;
                }
            }
        }
    }

    private float getSize(int index, int maxIndex)
    {
        return (((maxIndex - index) * (maxSize * sizeMultiplier)) / maxIndex);
    }

    public void addNewLink (Link newLink)
    {
        newLink.tag = "Link";
        newLink.init(links.Length, getSize(links.Length, links.Length + 1));
        newLink.addJoint();
        newLink.transform.parent = transform;
        if (links.Length > 1)
        {
            Vector3 dir = (links[links.Length - 2].transform.position - links[links.Length - 1].transform.position).normalized;
            dir.y = 0f;
            newLink.transform.position = links[links.Length - 1].transform.position - dir * (links[links.Length - 1].radius + newLink.radius);
        }
        else
        {
            Vector3 dir = links[0].transform.forward;
            dir.y = 0f;
            newLink.transform.position = links[0].transform.position - dir * (links[0].radius + newLink.radius);
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
            collectibleLinks[i].transform.localScale = new Vector3(maxSize, maxSize, maxSize) * sizeMultiplier;
        }
    }
}
