using UnityEngine;
using System.Collections;

public class LegController : MonoBehaviour {
    public GameObject player;
    public float maxAngle;
    private float currentAngle = 0.0f;
    public bool startForward;
    private bool moveForward;
    public Vector3 offset;
    public float degreePerSpeed = 1.0f;

	// Use this for initialization
	void Start () {
        if (startForward)
        {
            moveForward = false;
            currentAngle = maxAngle;
        }
        else
        {
            moveForward = true;
            currentAngle = -maxAngle;
        }
	}
	
	// Update is called once per frame
	void Update () {
        float degMovement = degreePerSpeed * Vector3.Magnitude(player.GetComponent<LizzieController>().vel) * Time.deltaTime;
        Debug.Log(degMovement + ", " + currentAngle);
        if (moveForward)
        {
            currentAngle += degMovement;
            if (currentAngle > maxAngle)
            {
                moveForward = false;
            }
        }
        else
        {
            currentAngle -= degMovement;
            if (currentAngle < -maxAngle)
            {
                moveForward = true;
            }
        }
	}

    void FixedUpdate()
    {
        transform.rotation = Quaternion.identity;
        //transform.position = new Vector3(player.transform.position.x, 0.5f, player.transform.position.z) + offset;
        transform.Rotate(Vector3.forward, currentAngle, Space.World);
    }
}
