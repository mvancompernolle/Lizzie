using UnityEngine;
using System.Collections;

public class TreeFall : MonoBehaviour {


   // public float collapseAngle;
    public float maxCollapseSpeed = 0;
    public float collapseAcc = 0;
    public float maxBounceStrength;

    private float collapseSpeed = 0;
    private Vector3 rotationAxis;
    private float currentRotation = 0;

    // Use this for initialization
    void OnTriggerEnter(Collider lizzie)
    {
        if (lizzie.gameObject.CompareTag("Player"))
        {
            LizzieController Lizzie = lizzie.GetComponent<LizzieController>();
            Vector3 direction = Vector3.Normalize(gameObject.transform.position - Lizzie.transform.position);
            direction.y = 0.0f;
            float hitStrength = Vector3.Magnitude(Lizzie.vel) / Lizzie.maxMovementSpeed;
            if (currentRotation == 0)
            {
                if (Vector3.Magnitude(Lizzie.vel) >= Lizzie.maxMovementSpeed / 2)
                {
                    rotationAxis = Vector3.Cross(Vector3.up, direction);
                    collapseSpeed = maxCollapseSpeed * hitStrength;
                }
                else
                {
                    rotationAxis = Vector3.Cross(Vector3.up, direction);
                    collapseSpeed = maxCollapseSpeed * hitStrength;
                }
                Lizzie.applyHit(maxBounceStrength * hitStrength, -direction, .6f);
            }
            else if (currentRotation == 90.0f)
            {
                Vector3 randomDirection = new Vector3(Random.value, 0.0f, Random.value);
                Lizzie.applyHit(0.3f, randomDirection, 0.6f);
            }
        }
    }
    void ApplyRotation()
    {
        transform.parent.transform.rotation = Quaternion.identity;
        transform.parent.transform.Rotate(rotationAxis, currentRotation, Space.World);
    }

    void ApplyWiggle()
    {
        transform.parent.transform.rotation = Quaternion.identity;
        transform.parent.transform.Rotate(rotationAxis, currentRotation, Space.World);
    }

    void Update()
    {
        if (currentRotation < 90 && collapseSpeed > 0)
        {
            collapseSpeed += collapseAcc * Time.deltaTime;
            currentRotation += collapseSpeed * Time.deltaTime;
            if( currentRotation > 90)
            {
                currentRotation = 90;
            }
        }
        ApplyRotation();
    }
}
