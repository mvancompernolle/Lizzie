using UnityEngine;
using System.Collections;

public class BuildingController : MonoBehaviour
{
    public float Resistance;
    public float Hardness;
    public bool IsRoof = false;

    private GameObject bld_Me;
    private float plyr_Speed;
    private int bld_MaxWiggleCount = 5;
    private int bld_WiggleCount = 0;
    private bool bld_WiggleRight = true;
    private bool bld_StartWiggle = false;

    /* Getters */

    private float bld_GetCollisionSpeed(Rigidbody collider)
    {
        return new Vector3(collider.velocity.x, 0.0f, collider.velocity.z).magnitude;
    }

    /* Updaters */

	void Start ()
    {
        bld_Me = GetComponent<GameObject>();
        bld_Me.AddComponent<Rigidbody>().isKinematic = true;
	}

    void FixedUpdate()
    {
        if(bld_StartWiggle && !IsRoof)
        {

            if(bld_WiggleRight)
            {

            }

            if(bld_WiggleCount >= bld_MaxWiggleCount)
            {

            }
        }
    }

	void Update ()
    {

	}

    void OnCollisionEnter(Collision lizzie)
    {
        if(lizzie.collider.CompareTag("Player"))
        {
            if(bld_GetCollisionSpeed(lizzie.collider.GetComponent<Rigidbody>()) > Resistance)
            {
                Vector3 upImpulse = new Vector3(0.0f, bld_GetCollisionSpeed(lizzie.collider.GetComponent<Rigidbody>()) / 2, 0.0f);
                bld_Me.GetComponent<Rigidbody>().isKinematic = false;
                bld_Me.GetComponent<Rigidbody>().AddForce(upImpulse, ForceMode.Impulse);
            }
            else
            {
                bld_StartWiggle = true;
            }

            lizzie.collider.GetComponent<LizzieController>().applyHit(Hardness, -(lizzie.collider.GetComponent<Rigidbody>().velocity), 0.5f);
        }
    }
}
