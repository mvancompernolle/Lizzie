using UnityEngine;
using System.Collections;

public class LizzieController : MonoBehaviour {
    public Rigidbody rbody;
    public GameObject bulletObject;
    public float speed, bulletSpeed;
    public float tipStrength;
    public float bulletOffset = 5.0f;

	// Use this for initialization
	void Start () {
	
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
            Debug.Log(shootDirection);

            // create new bullet and set position
            GameObject newBullet = Instantiate(bulletObject, transform.position, transform.rotation) as GameObject;
            newBullet.transform.position = transform.position + (normalized * bulletOffset);

            // apply force based on mase location and distance form center
            float distance = Vector3.Magnitude(shootDirection);
            float minLength = Screen.width < Screen.height ? Screen.width / 2.0f : Screen.height / 2.0f;
            float scaleVel = (distance / minLength);
            float speedScalar = scaleVel < 0.5f ? 0.5f : scaleVel;
            newBullet.GetComponent<Rigidbody>().AddForce((normalized * bulletSpeed) * speedScalar, ForceMode.Impulse);

            // cause the tower to move in the oposite direction
            this.rbody.velocity += -(normalized * tipStrength * speedScalar);
        }
        transform.Translate(rbody.velocity * Time.deltaTime);
	}
}
