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

    public float startingSizeMultiplier;
    public float startingForceMultiplier;
    public float forceMultiplierIncrease;
    public float sizeMultiplierIncrease;
    public Rigidbody headPrefab;
    public Link linkPrefab;

    private Rigidbody head;
    private List<Link> links;
    private float linkRadius;
    private float forceMultiplier;
    private float sizeMultiplier;

    private void Awake()
    {
        head = Instantiate(headPrefab, transform.position, Quaternion.identity, transform);
        linkRadius = head.GetComponent<SphereCollider>().bounds.size.z;
        links = new List<Link>();
        links.Add(head.GetComponent<Link>());
        links.Add(Instantiate(linkPrefab, transform.position - new Vector3(0f, 0f, linkRadius), Quaternion.identity, transform));

        forceMultiplier = startingForceMultiplier;
        sizeMultiplier = startingSizeMultiplier;
    }

    void Start ()
    {
        links[1].connect(head);
	}
	
	void Update ()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        head.transform.Rotate(head.transform.up, input.x * 10f);

        head.AddForce(head.transform.TransformDirection(new Vector3(0f, 0f, input.y)) * forceMultiplier);
    }

    public void addNewLink (Link newLink)
    {
        if (!links.Contains(newLink))
        {
            Vector3 distance = (head.transform.position - links[links.Count - 1].transform.position).normalized * linkRadius;
            newLink.transform.parent = transform;
            head.transform.position += distance;
            newLink.transform.position = head.transform.position - distance;
            links[links.Count - 1].connect(newLink.GetComponent<Rigidbody>());
            newLink.connect(head);
            links.Add(newLink);

            forceMultiplier += forceMultiplierIncrease;
            sizeMultiplier += sizeMultiplierIncrease;
        }
    }

    public Transform getHead ()
    {
        return head.transform;
    }
}
