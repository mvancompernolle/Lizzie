using UnityEngine;
using System.Collections;

/*****************************************************************************\
 *  Note:                                                                    *
 *                                                                           *
 *  Latchers DO NOT modify Lizzie's behavior or "physics" in any way... YET  *
\*****************************************************************************/

public class LatcherAI : MonoBehaviour
{
    public Rigidbody Target;
    public float Speed = 10;
    public float JumpForce = 1;
    public float LatchStrength = 5;

    private Rigidbody ai_Latcher;
    private Vector3 ai_Offset;
    private Transform ai_OT; //This PKMN's OT
    private bool ai_HasLatched;
    private bool ai_HasJumped;
    private bool ai_HitLizzie;

    /* Getters */

    private Vector3 ai_GetDir() { return (Target.position - transform.position).normalized; }
    private float ai_GetDistance() { return Vector3.Distance(transform.position, Target.position); }
    private float ai_GetJumpRange()
    {
        float velocity = Mathf.Sqrt(Mathf.Pow(ai_Latcher.velocity.x, 2) + Mathf.Pow(ai_Latcher.velocity.z, 2));
        return velocity / 2;
    }


    /* Updaters */

    void Start()
    {
        ai_Latcher = GetComponent<Rigidbody>();
        ai_OT = ai_Latcher.transform.parent;
        ai_HasLatched = false;
        ai_HasJumped = false;
        ai_HitLizzie = false;
    }

    void FixedUpdate()
    {
        if (ai_GetDistance() < ai_GetJumpRange() && !ai_HasJumped)
        {
            ai_Latcher.AddForce(Vector3.up * 3, ForceMode.Impulse);
            ai_HasJumped = true;
        }
        else if (ai_HitLizzie && !ai_HasJumped)
        {
            if (ai_GetDistance() > 20) { ai_HitLizzie = false; }
            ai_Latcher.AddForce(ai_GetDir() * -Speed);
        }
        else
        {
            ai_Latcher.AddForce(ai_GetDir() * Speed);
        }
    }

    void Update()
    {
        if (!ai_HasLatched)
        {
            ai_Latcher.transform.parent = ai_OT;
            ai_Latcher.useGravity = true;
        }
        else
        {
            ai_Latcher.useGravity = false;
        }
    }

    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Floor")) { ai_HasJumped = false; ai_HasLatched  = false; }
    }

    void OnCollisionEnter(Collision lizzie)
    {
        if (lizzie.gameObject.CompareTag("Player") && !ai_HasLatched)
        {
            LizzieController Lizzie = lizzie.collider.GetComponent<LizzieController>();
            Vector3 force = ai_GetDir();

            ai_Latcher.transform.parent = lizzie.transform;

            force.y = 0.0f;
            ai_HitLizzie = true;
            ai_HasLatched = true;
            
            Lizzie.applyHit((JumpForce / 100) * (ai_GetJumpRange() * 2), force);
        }
    }
}
