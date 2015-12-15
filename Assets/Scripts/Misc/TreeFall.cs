using UnityEngine;
using System.Collections;

public class TreeFall : MonoBehaviour {


   // public float collapseAngle;
    public float maxCollapseSpeed = 0;
    public float collapseAcc = 0;
    public float maxBounceStrength;
    private float wobbleSpeed = 30;

    private float collapseSpeed = 0;
    private Vector3 rotationAxis;
    private float currentRotation = 0;
    private bool wigglePos;
    private int numWobbles;
    private bool lightHit = false;

    // Use this for initialization
    void OnTriggerEnter(Collider lizzie)
    {
        if (lizzie.gameObject.CompareTag("Player"))
        {
            LizzieController Lizzie = lizzie.GetComponent<LizzieController>();
            Vector3 direction = Vector3.Normalize(gameObject.transform.position - Lizzie.transform.position);
            direction.y = 0.0f;
            float hitStrength = Vector3.Magnitude(Lizzie.vel) / Lizzie.maxMovementSpeed;
            if (currentRotation == 0 || lightHit)
            {
                if (Vector3.Magnitude(Lizzie.vel) >= Lizzie.maxMovementSpeed / 2)
                {
                    lightHit = false;
                    rotationAxis = Vector3.Cross(Vector3.up, direction);
                    collapseSpeed = maxCollapseSpeed * hitStrength;
                }
                else if(!lightHit)
                {
                    lightHit = true;
                    wigglePos = true;
                    numWobbles = 0;
                    //determine whether to start wiggling left or right
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

    void FixedUpdate()
    {
        if (lightHit)
        {
            if( numWobbles < 5)
            {
                if (wigglePos == true)
                {
                    if (currentRotation < 10)
                    {
                        currentRotation += wobbleSpeed * Time.deltaTime;
                        if (currentRotation >= 10)
                        {
                            wigglePos = false;
                            numWobbles++;
                        }
                    }
                }
                else if (wigglePos == false)
                {
                    if (currentRotation > -10)
                    {
                        currentRotation -= wobbleSpeed * Time.deltaTime;
                        if (currentRotation <= -10)
                        {
                            wigglePos = true;
                            numWobbles++;
                        }
                    }
                }
            }
            else
            {
                currentRotation -= wobbleSpeed * Time.deltaTime;
                if ( Mathf.Abs(currentRotation) <= 0.5f)
                {
                    currentRotation = 0.0f;
                    lightHit = false;
                    collapseSpeed = 0.0f;
                }
            }
        }
        else if (currentRotation < 90 && collapseSpeed > 0 && !lightHit)
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
