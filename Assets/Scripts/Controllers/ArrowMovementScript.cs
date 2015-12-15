using UnityEngine;
using System.Collections;

public class ArrowMovementScript : MonoBehaviour {

    public GameObject target;
    private Rigidbody targetRbody;
    public Vector3 vel, targetPos;
    public float speed = 10;
    public float currentRotation;
    private float desiredRotation;
    public float turnRate = 100.0f;
    bool landed = false;

	// Use this for initialization
	void Start () {
        if (target != null)
        {
            targetRbody = target.GetComponent<Rigidbody>();
        }
	}

    /*void OnCollisionEnter(Collision hitBy)
    {
        Debug.Log("on collision enter");
        if (hitBy.gameObject.tag.Equals("Floor"))
        {
            Destroy(gameObject, 5.0f);
        }
        else if (target != null && hitBy.gameObject == target)
        {
            Debug.Log("hit target");
            Destroy(gameObject);
        }
    }*/
	
	// Update is called once per frame
	void Update () {
        // steer towards target
        if (target != null)
        {
            float relativeSpeed = Mathf.Abs(Vector3.Magnitude(vel) - Vector3.Magnitude(targetRbody.velocity));
            float distance = Vector3.Distance(target.transform.position, gameObject.transform.position);
            float time = 0.0f;
            if (relativeSpeed != 0.0f)
            {
                time = distance / relativeSpeed;
            }
            targetPos = target.transform.position + targetRbody.velocity * time;
            Vector3 posDiff = new Vector3(targetPos.x - gameObject.transform.position.x, 0.0f, targetPos.z - gameObject.transform.position.z);
            desiredRotation = Mathf.Atan2(posDiff.z, posDiff.x) * Mathf.Rad2Deg + 90;
            desiredRotation = desiredRotation % 360;
            if (desiredRotation < 0) { desiredRotation += 360; }

            if (Mathf.Abs(desiredRotation - currentRotation) < turnRate * Time.deltaTime)
            {
                currentRotation = desiredRotation;
            }
            else
            {
                // turn lizzie towards target
                if ((desiredRotation > currentRotation && desiredRotation - currentRotation <= 180) || (desiredRotation < currentRotation && currentRotation - desiredRotation >= 180))
                {
                    currentRotation += turnRate * Time.deltaTime;
                }
                else
                {
                    currentRotation -= turnRate * Time.deltaTime;
                }
                currentRotation = currentRotation % 360;
                if (currentRotation < 0) { currentRotation += 360; }
            }
        }

        Vector3 dir = targetPos - transform.position;
        float downRotation = Mathf.Atan2(dir.y, dir.z) * Mathf.Rad2Deg + 90;
        if (downRotation >= 0)
            downRotation -= 90;
        //Debug.Log(downRotation);
        //Debug.Log("target: " + targetPos);

        // move arrow
        vel = Vector3.Normalize(targetPos - transform.position) * speed;
        //Debug.Log("rotation: " + currentRotation + " vel: " + vel + " speed: " + speed);
        transform.Translate(vel * Time.deltaTime, Space.World);
        transform.rotation = Quaternion.identity;
        transform.Rotate(Vector3.up, -currentRotation, Space.World);
        transform.Rotate(Vector3.right, downRotation, Space.Self);
	}
}
