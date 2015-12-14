using UnityEngine;
using System.Collections.Generic;

public class LizzieController : MonoBehaviour {
    public Rigidbody rbody;
    public GameObject bulletObject;
    public Transform bulletSpawnLocation;
    public List<GameObject> bulletTargets;
    public float bulletSpeed;
    public float tipStrength;
    public float maxMovementSpeed;
    public float maxTiltAngle;
    public float bulletOffset = 5.0f;
    public Vector3 vel, prevVel, goalVel;
    private float animationTime;
    private float currAnimationTime = 0.0f;
    private float mousePrecision = 2.0f;
    bool falling;

	// Use this for initialization
	void Start () {
        falling = false;
        vel = prevVel = goalVel = new Vector3(0.0f, 0.0f, 0.0f);
        bulletTargets = new List<GameObject>();
	}

    public void applyHit(float percentTilt, Vector3 direction, float snapTime = 0.1f)
    {
        if (!falling)
        {
            // set the new velocity
            prevVel = vel;
            currAnimationTime = 0.0f;
            animationTime = snapTime;
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
        if (Input.GetButtonDown("Fire1") ) 
        {
            // get mouse direction
            float mouseX = Input.mousePosition.x;
            float mouseY = Input.mousePosition.y;

            Ray ray = Camera.main.ViewportPointToRay(new Vector3(mouseX / Screen.width, mouseY / Screen.height));

            var hits = Physics.RaycastAll(ray);
            Vector3 target = new Vector3();
            if (hits.Length > 0)
            {
                target = hits[0].point;
                float dist = hits[0].distance;
                for (int i = 1; i < hits.Length; ++i)
                {
                    if (hits[i].distance > dist)
                    {
                        dist = hits[i].distance;
                        target = hits[i].point;
                    }
                }
            }

            GameObject closestEnemy = null;
            if (bulletTargets.Count > 0)
            {
                float closestDistance = Vector3.Distance(target, bulletTargets[0].GetComponent<Rigidbody>().transform.position);
                Debug.Log("enemy distance: " + closestDistance);
                if (closestDistance <= mousePrecision)
                {
                    closestEnemy = bulletTargets[0];
                }
                // determine if mouse is over enemy
                for (int i = 1; i < bulletTargets.Count; ++i)
                {
                    float enemyDist = Vector3.Distance(target, bulletTargets[1].GetComponent<Rigidbody>().transform.position);
                    Debug.Log("enemy distance: " + enemyDist);
                    if (enemyDist <= mousePrecision && enemyDist < closestDistance)
                    {
                        closestEnemy = bulletTargets[i];
                    }
                }
            }

            //Vector3 shootDirection = new Vector3(mouseX, 0.0f, mouseY) - new Vector3(Screen.width/2, 0.0f, Screen.height/2);
            Vector3 shootDirectionAngled = new Vector3(target.x, 2.0f, target.z) - bulletSpawnLocation.position;
            Vector3 shootDirection = new Vector3(target.x, 0.0f, target.z) - new Vector3(bulletSpawnLocation.position.x, 0.0f, bulletSpawnLocation.position.z);
            
            Vector3 normalized = Vector3.Normalize(shootDirection);

            // create new bullet and set position
            Vector3 position = new Vector3(
                transform.position.x,
                transform.position.y + GetComponent<BoxCollider>().bounds.extents.y,
                transform.position.z);
            GameObject newBullet = Instantiate(bulletObject, bulletSpawnLocation.position, transform.rotation) as GameObject;
            newBullet.transform.position = position + (normalized * bulletOffset);

            //Makes sure Lizzie's bullets don't collide with her.
            BulletController testing = newBullet.GetComponent<BulletController>();
            testing.Origin = rbody.GetComponent<Collider>();

            // apply force based on mase location and distance form center
            float distance = Vector3.Magnitude(shootDirection);
            float minLength = Screen.width < Screen.height ? Screen.width / 2.0f : Screen.height / 2.0f;
            float scaleVel = (distance / minLength);
            float speedScalar = scaleVel < 0.3f ? 0.3f : scaleVel;
            if (closestEnemy == null)
            {
                newBullet.GetComponent<Rigidbody>().AddForce((Vector3.Normalize(shootDirectionAngled) * bulletSpeed) * speedScalar + vel, ForceMode.Impulse);
                Debug.Log("free");
            }
            else
            {
                Debug.Log("locked on");
                newBullet.GetComponent<Rigidbody>().AddForce(
                    (Vector3.Normalize(closestEnemy.GetComponent<Rigidbody>().transform.position - newBullet.transform.position).normalized * bulletSpeed) * speedScalar, ForceMode.Impulse);
            }
            

            // cause the tower to move in the oposite direction
            applyHit(tipStrength * speedScalar, -normalized, 0.1f);
        }

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
