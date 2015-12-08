using UnityEngine;
using System.Collections;

public class LizzieController : MonoBehaviour {
    public Rigidbody rbody;
    public GameObject bulletObject;
    public float speed, bulletSpeed;
    public float tipStrength;
    public float maxMovementSpeed;
    public float maxTiltAngle;
    public float bulletOffset = 5.0f;
    public Vector3 vel;

	// Use this for initialization
	void Start () {
        vel = new Vector3(0.0f, 0.0f, 0.0f);
	}

    public void applyHit(float percentTilt, Vector3 direction)
    {
        // set the new velocity
        vel += direction * percentTilt * maxMovementSpeed;
        float newSpeed = Vector3.Magnitude(vel);
        Vector3 normVel = Vector3.Normalize(vel);
        if (newSpeed > maxMovementSpeed)
        {
            vel = normVel * maxMovementSpeed;
        }
    }

    private void applyRotation()
    {
        // set the tilt angle
        float newSpeed = Vector3.Magnitude(vel);
        Vector3 normVel = Vector3.Normalize(vel);
        float rotationAngle = maxTiltAngle * (newSpeed / maxMovementSpeed);
        rotationAngle = rotationAngle < maxTiltAngle ? rotationAngle : maxTiltAngle;
        Debug.Log("Angle: " + rotationAngle);
        Vector3 rotationAxis = Vector3.Cross(Vector3.up, normVel);
        Debug.Log("Axis: " + rotationAxis);
        float yRotation = Vector3.Angle(Vector3.forward, normVel);
        if (vel.x < 0)
        {
            yRotation = 360.0f - yRotation;
        }
        Debug.Log(yRotation);
        transform.rotation = Quaternion.identity;
        transform.Rotate(rotationAxis, rotationAngle, Space.World);
        transform.Rotate(Vector3.up, yRotation, Space.Self);
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1") ) 
        {
            // get mouse direction
            float mouseX = Input.mousePosition.x;
            float mouseY = Input.mousePosition.y;
            Vector3 shootDirection = new Vector3(mouseX, 0.0f, mouseY) - new Vector3(Screen.width/2, 0.0f, Screen.height/2);
            Vector3 normalized = Vector3.Normalize(shootDirection);

            // create new bullet and set position

            Vector3 position = new Vector3(
                transform.position.x,
                transform.position.y + GetComponent<BoxCollider>().bounds.extents.y,
                transform.position.z);
            GameObject newBullet = Instantiate(bulletObject, position, transform.rotation) as GameObject;
            newBullet.transform.position = transform.position + (normalized * bulletOffset);

            // apply force based on mase location and distance form center
            float distance = Vector3.Magnitude(shootDirection);
            float minLength = Screen.width < Screen.height ? Screen.width / 2.0f : Screen.height / 2.0f;
            float scaleVel = (distance / minLength);
            float speedScalar = scaleVel < 0.5f ? 0.5f : scaleVel;
            newBullet.GetComponent<Rigidbody>().AddForce((normalized * bulletSpeed) * speedScalar + vel, ForceMode.Impulse);

            // cause the tower to move in the oposite direction
            applyHit(tipStrength * speedScalar, -normalized);
            Debug.Log(vel);
        }
        transform.Translate(vel * Time.deltaTime, Space.World);
	}

    void LateUpdate()
    {
        applyRotation();
    }
}
