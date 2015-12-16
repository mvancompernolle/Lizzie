using UnityEngine;
using System.Collections;


public class ShootController : MonoBehaviour {

    public GameObject player;
    private LizzieController lizzieController;
    public GameObject bulletObject;
    public Transform bulletSpawnLocation;
    private Transform floor;
    public float tipStrength;
    public float bulletSpeed;
    private float mousePrecision = 2.0f;
    public float bulletOffset = 5.0f;

	// Use this for initialization
	void Start () {
        lizzieController = player.GetComponent<LizzieController>();
        floor = GameObject.FindGameObjectWithTag("Floor").transform;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1"))
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
            if (lizzieController.bulletTargets.Count > 0)
            {
                float closestDistance = Vector3.Distance(target, lizzieController.bulletTargets[0].GetComponent<Rigidbody>().transform.position);
                if (closestDistance <= mousePrecision)
                {
                    closestEnemy = lizzieController.bulletTargets[0];
                }
                // determine if mouse is over enemy
                for (int i = 1; i < lizzieController.bulletTargets.Count; ++i)
                {
                    float enemyDist = Vector3.Distance(target, lizzieController.bulletTargets[1].GetComponent<Rigidbody>().transform.position);
                    if (enemyDist <= mousePrecision && enemyDist < closestDistance)
                    {
                        closestEnemy = lizzieController.bulletTargets[i];
                    }
                }
            }

            // create new bullet and set position
            Vector3 position = new Vector3(
                bulletSpawnLocation.position.x,
                bulletSpawnLocation.position.y + GetComponent<BoxCollider>().bounds.extents.y,
                bulletSpawnLocation.position.z);

            Vector3 direction = Vector3.Normalize(gameObject.transform.position - lizzieController.transform.position);
            Vector3 shootDirectionAngled = new Vector3(target.x, 2.0f, target.z) - position;
            Vector3 shootDirection = new Vector3(target.x, 0.0f, target.z) - new Vector3(position.x, 0.0f, position.z);

            Vector3 normalized = Vector3.Normalize(shootDirection);

            GameObject newBullet = Instantiate(bulletObject, bulletSpawnLocation.position, transform.rotation) as GameObject;
            newBullet.transform.position = position + (normalized * bulletOffset);
            ArrowMovementScript bulletMovement = newBullet.GetComponent<ArrowMovementScript>();
            float distance = Vector3.Magnitude(new Vector2(mouseX, mouseY) - new Vector2(Screen.width / 2.0f, Screen.height / 2.0f));
            float minLength = Screen.width < Screen.height ? Screen.width / 2.0f : Screen.height / 2.0f;
            //Debug.Log("distance: " + distance + " minLength: " + minLength);
            float scaleVel = (distance / minLength);
            scaleVel = Mathf.Clamp(scaleVel, 0.3f, 1.0f);
            bulletMovement.speed = 50 * scaleVel;
            if (closestEnemy != null)
            {
                //Debug.Log("locked on");
                bulletMovement.target = closestEnemy;
                bulletMovement.targetPos = closestEnemy.transform.position;
            }
            else
            {
                //Debug.Log("free shot");
                bulletMovement.target = null;
                bulletMovement.targetPos = new Vector3(target.x, 0.0f, target.z);
            }
            // move arrow
            bulletMovement.vel = Vector3.Normalize(bulletMovement.targetPos - bulletMovement.transform.position) * bulletMovement.speed;
            Vector3 dir = bulletMovement.targetPos - newBullet.transform.position;
            bulletMovement.currentRotation = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg + 90;
            BulletController bulletController = newBullet.GetComponent<BulletController>();
            bulletController.Origin = lizzieController.rbody.GetComponent<Collider>();

            // cause the tower to move in the oposite direction
            lizzieController.applyHit(tipStrength * scaleVel, -normalized, 0.1f);
        }
	}
}
