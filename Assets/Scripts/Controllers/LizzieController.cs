using UnityEngine;
using System.Collections.Generic;

public class LizzieController : MonoBehaviour {
    public Rigidbody rbody;
    public List<GameObject> bulletTargets = new List<GameObject>();
    public float maxMovementSpeed;
    public float maxTiltAngle;
    public Vector3 vel, prevVel, goalVel;
    private float animationTime;
    private float currAnimationTime = 0.0f;
    bool falling;

	// Use this for initialization
	void Start () {
        falling = false;
        vel = prevVel = goalVel = new Vector3(0.0f, 0.0f, 0.0f);
	}

    public void applyHit(float percentTilt, Vector3 direction, float snapTime = 0.1f)
    {
        if (!falling)
        {
            // set the new velocity
            prevVel = vel;
            currAnimationTime = 0.0f;
            animationTime = snapTime;
            direction.y = 0.0f;
            goalVel += direction * percentTilt * maxMovementSpeed;
            float newSpeed = Vector3.Magnitude(goalVel);
            Vector3 normVel = Vector3.Normalize(goalVel);
            if (newSpeed > maxMovementSpeed)
            {
                goalVel = normVel * maxMovementSpeed;
                falling = true;
                rbody.isKinematic = false;
                rbody.AddForce(vel * 10, ForceMode.Impulse);
                rbody.AddTorque(new Vector3(Random.Range(10, 100), Random.Range(10, 100), Random.Range(10, 100)), ForceMode.Impulse );
                rbody.useGravity = true;
            }
        }
    }

    private void applyRotation()
    {
        if (!falling)
        {
            // set the tilt angle
            float newSpeed = Vector3.Magnitude(vel);
            Vector3 normVel = Vector3.Normalize(vel);
            float rotationAngle = maxTiltAngle * (newSpeed / maxMovementSpeed);
            rotationAngle = rotationAngle < maxTiltAngle ? rotationAngle : maxTiltAngle;
            //Debug.Log("Angle: " + rotationAngle);
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, normVel);
            //Debug.Log("Axis: " + rotationAxis);
            float yRotation = Vector3.Angle(Vector3.forward, normVel);
            if (vel.x < 0)
            {
                yRotation = 360.0f - yRotation;
            }
            transform.rotation = Quaternion.identity;
            transform.Rotate(rotationAxis, rotationAngle, Space.World);
            transform.Rotate(Vector3.up, yRotation, Space.Self);
        }
    }

	// Update is called once per frame
	void Update () {
        // move the player and steer velocity
        if (!falling)
        {
            if (vel != goalVel && currAnimationTime < animationTime)
            {
                currAnimationTime += Time.deltaTime;        // hotdog
                if (currAnimationTime >= animationTime)
                {
                    currAnimationTime = 0.0f;
                    vel = goalVel;
                }
                else
                {
                    vel = Vector3.Lerp(prevVel, goalVel, currAnimationTime / animationTime);
                }
            }
            transform.Translate(vel * Time.deltaTime, Space.World);
        }
	}

    void LateUpdate()
    {
        if (!falling)
        {
            applyRotation();
        }

    }
}
