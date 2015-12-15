using UnityEngine;
using System.Collections;

public class RamAI : EnemyController
{
    private bool ai_HasJumped;
    private bool ai_HitLizzie;

    /* Setters */
    
    public void applyHit(float force, Vector3 direction)
    {
        ai_Me.AddForce(direction * force, ForceMode.Impulse);
        ai_HasJumped = true;
    }

    /* Updaters */

    public override void Start () 
    {
        base.Start();
        ai_HasJumped = false;
        ai_HitLizzie = false;
    }

    void FixedUpdate ()
    {
        if (ai_GetDistToTar() < ai_GetAttackRange() && !ai_HasJumped)
        {
            if (WaitForLizzie)
            {
                ai_Me.AddForce(ai_GetDir() * Speed, ForceMode.Impulse);
                WaitForLizzie = false;
            }
            ai_Me.AddForce(Vector3.up * 3, ForceMode.Impulse);
            ai_HasJumped = true;
        }
        else if (!WaitForLizzie)
        {
            if (ai_HitLizzie && !ai_HasJumped)
            {
                if (ai_GetDistToTar() > 20) { ai_HitLizzie = false; }
                ai_Me.AddForce(ai_GetDir() * -Speed);
            }
            else if(!ai_HasJumped) { ai_Me.AddForce(ai_GetDir() * Speed); }
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
            if (Lizzie != null) { Lizzie.applyHit((Damage / 100) * ai_GetVel(), force); }
        }
    }
}