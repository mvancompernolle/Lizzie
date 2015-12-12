using UnityEngine;
using System.Collections;

public class LegController : MonoBehaviour {
    public GameObject player;
    public Transform legBase;
    public float maxAngle;
    private float currentAngle = 0.0f;
    public bool startForward;
    private bool moveForward;
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
        LizzieController playerController = player.GetComponent<LizzieController>();
        float angleOffset = (Vector3.Magnitude(playerController.vel) / playerController.maxMovementSpeed) * -30;

        float degMovement = degreePerSpeed * Vector3.Magnitude(playerController.vel) * Time.deltaTime;
        if (moveForward)
        {
            currentAngle += degMovement;
            if (currentAngle > maxAngle + angleOffset)
            {
                currentAngle -= (currentAngle - (maxAngle + angleOffset));
                moveForward = false;
            }
        }
        else
        {
            currentAngle -= degMovement;
            if (currentAngle < -maxAngle + angleOffset)
            {
                currentAngle += ((-maxAngle + angleOffset) - currentAngle);
                moveForward = true;
            }
        }
	}

    void FixedUpdate()
    {
        //legBase.rotation = Quaternion.identity;
        legBase.rotation = player.transform.rotation;
        legBase.Rotate(Vector3.right, currentAngle, Space.Self);
    }
}
