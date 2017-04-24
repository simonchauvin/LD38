using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;

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

    public float speed;
    public float rotationSpeed;
    public float sizeIncreaseFactor;
    public float minGroundDistance;
    public float maxSize;
    public Link linkPrefab;
    public Transform lerpLinkPrefab;

    private ThirdPersonCamera cam;
    private Link[] links;
    private List<Transform> collectibleLinks;
    private Transform front;
    private Transform lerpLink;

    private float sizeMultiplier;
    private bool canStart;
    private float moveAccu;


    private void Awake()
    {
        sizeMultiplier = 1;
        canStart = false;
        moveAccu = 0f;

        cam = GameObject.FindObjectOfType<ThirdPersonCamera>();
        links = new Link[1];
        collectibleLinks = new List<Transform>();
        front = new GameObject("Front").transform;
        front.parent = transform;
        lerpLink = Instantiate(lerpLinkPrefab, transform.position, Quaternion.identity, transform);

        links[0] = Instantiate(linkPrefab, transform.position, Quaternion.identity, transform);
        links[0].init(0, getSize(0, 1));
        front.position = links[0].transform.position;
        lerpLink.position = front.position + front.forward * links[0].size;
    }

    void Start ()
    {
        Transform linksFolder = GameObject.Find("Links").transform;
        for (int i = 0; i < linksFolder.childCount; i++)
        {
            collectibleLinks.Add(linksFolder.GetChild(i));
        }
    }
	
	void Update ()
    {
        if (canStart)
        {
            // Debug
            /*if (Input.GetKey(KeyCode.F))
            {
                addNewLink(null);
            }*/

            front.position = links[0].transform.position;
            lerpLink.position = front.position + front.forward * links[0].size;
        }
        else
        {
            canStart = true;
            for (int i = 0; i < links.Length; i++)
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(links[i].transform.position, Vector3.down, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground")))
                {
                    links[i].transform.position = hitInfo.point + new Vector3(0f, links[i].radius, 0f);
                }
            }
            for (int i = 0; i < collectibleLinks.Count; i++)
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(collectibleLinks[i].position, Vector3.down, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground")))
                {
                    collectibleLinks[i].localScale = new Vector3(maxSize, maxSize, maxSize);
                    collectibleLinks[i].position = hitInfo.point + new Vector3(0f, maxSize * 0.5f, 0f);
                }
            }
            front.position = links[0].transform.position;
            lerpLink.position = front.position + front.forward * links[0].size;
            cam.init();
        }
    }

    private void FixedUpdate()
    {
        if (canStart)
        {
            // Heads movement
            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            RaycastHit hitInfo;
            if (Physics.Raycast(front.transform.position, -front.transform.up, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                if (hitInfo.distance - links[0].size < minGroundDistance)
                {
                    front.forward = Vector3.ProjectOnPlane(front.transform.forward, hitInfo.normal);
                }
                else
                {
                    front.forward = Vector3.ProjectOnPlane(front.transform.forward, Vector3.up);
                }
            }
            front.Rotate(front.transform.up, input.x * rotationSpeed);

            // Body movement
            if (input.y > 0)
            {
                moveAccu += speed * Time.deltaTime;
                if (moveAccu >= (links[links.Length - 1].radius + links[0].radius))
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

                        links[0].transform.position = links[1].transform.position + (links[0].radius + links[1].radius) * front.forward.normalized;

                        links[1].addJoint();
                        links[1].connect(links[0]);
                    }
                    else
                    {
                        links[0].transform.position += front.forward.normalized * links[0].size;
                    }

                    moveAccu = 0f;
                    lerpLink.localScale = Vector3.one;
                }
                else
                {
                    float newScale = links[0].size * (moveAccu / (links[links.Length - 1].radius + links[0].radius));
                    lerpLink.localScale = new Vector3(newScale, newScale, newScale);
                }
            }
        }
    }

    private float getSize(int index, int maxIndex)
    {
        return (((maxIndex - index) * (maxSize * sizeMultiplier)) / maxIndex);
    }

    public void addNewLink (Transform collectibleLink)
    {
        Link newLink = Instantiate(linkPrefab, transform);
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

        if (collectibleLink != null)
        {
            collectibleLinks.Remove(collectibleLink);
            Destroy(collectibleLink.gameObject);
        }

        sizeMultiplier *= sizeIncreaseFactor;
    }

    public Vector3 getFrontPosition ()
    {
        return front.position;
    }

    public void increaseCollectiblesSize ()
    {
        for (int i = 0; i < collectibleLinks.Count; i++)
        {
            collectibleLinks[i].transform.localScale = new Vector3(maxSize, maxSize, maxSize) * sizeMultiplier;
        }
    }
}
