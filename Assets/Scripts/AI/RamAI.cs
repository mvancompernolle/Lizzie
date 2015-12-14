using UnityEngine;

using System.Collections;

public class RamAI : MonoBehaviour
{
    public Transform Target;
    public float Speed;
    public float JumpForce;
    public bool WaitForLizzie = false;
    public float TriggerDistance = 0.0f;

    private Rigidbody ai_Ram;
    private bool ai_HasJumped;
    private bool ai_HitLizzie;

    /* Intakes */

    /********************************************************************\
     *  Mostly used by BulletController objects, though, theorectically *
     *  Lizzie could call this to knock back Rammers she runs into.     *
    \********************************************************************/
    public void applyHit(float force, Vector3 direction)
    {
        ai_Ram.AddForce(direction * force, ForceMode.Impulse);
        ai_HasJumped = true;
    }

    /* Getters */

    //For use with BulletController, mostly. Might prove useful elsewhere.
    public Vector3 ai_GetPos() { return transform.position; }

    float ai_GetJumpRange()
    {
        if(WaitForLizzie) { return TriggerDistance; }
        float velocity = Mathf.Sqrt(Mathf.Pow(ai_Ram.velocity.x, 2) + Mathf.Pow(ai_Ram.velocity.z, 2));
        return velocity / 2;
    }
    float ai_GetDistance()
    {
        return Vector3.Distance(transform.position, Target.position);
    }
    Vector3 ai_GetDir() { return (Target.position - transform.position).normalized; }

    /* Updaters */

    void Start ()
    {
        ai_Ram = GetComponent<Rigidbody>();
        ai_HasJumped = false;
        ai_HitLizzie = false;
    }

    void FixedUpdate ()
    {
        if (ai_GetDistance() < ai_GetJumpRange() && !ai_HasJumped)
        {
            if (WaitForLizzie)
            {
                ai_Ram.AddForce(ai_GetDir() * Speed, ForceMode.Impulse);
                WaitForLizzie = false;
            }
            ai_Ram.AddForce(Vector3.up * 3, ForceMode.Impulse);
            ai_HasJumped = true;
        }
        else if (!WaitForLizzie)
        {
            if (ai_HitLizzie && !ai_HasJumped)
            {
                if (ai_GetDistance() > 20) { ai_HitLizzie = false; }
                ai_Ram.AddForce(ai_GetDir() * -Speed);
            }
            else if(!ai_HasJumped) { ai_Ram.AddForce(ai_GetDir() * Speed); }
        }
    }

    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Floor")) { ai_HasJumped = false; }
    }

    void OnCollisionEnter(Collision lizzie)
    {
        if(lizzie.gameObject.CompareTag("Player"))
        {
            LizzieController Lizzie = lizzie.collider.GetComponent<LizzieController>();
            Vector3 force = ai_GetDir();

            force.y = 0.0f;
            ai_HitLizzie = true;
            Lizzie.applyHit((JumpForce / 100) * (ai_GetJumpRange() * 2), force);
        }
    }
}