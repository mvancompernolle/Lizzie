using UnityEngine;
using System.Collections;

public class SliderAI : EnemyController
{
    public float SlideInterval = 2.0f;
    
    private float ai_StunTimer;
    private float ai_CurrentTime = 0.0f;
    private bool ai_HasSlid = false;
    private bool ai_IsStunned = false;

    /* Setters */

    public void ai_Stun(float duration)
    {
        ai_IsStunned = true;
        ai_StunTimer = duration;
    }

    /* Updaters */

    void FixedUpdate()
    {
        if (!ai_IsStunned)
        {
            if (ai_GetDistToTar() < ai_GetAttackRange() && !ai_HasSlid)
            {
                Vector3 dontGoUp = ai_GetDir();
                WaitForLizzie = false;
                dontGoUp.y = 0.0f;
                ai_Me.AddForce(dontGoUp * Damage, ForceMode.Impulse);
            }
            else
            {
                if (ai_HasSlid)
                {
                    if (ai_GetDistToTar() >= 20) { ai_HasSlid = false; }
                    else { ai_Me.AddForce(ai_GetDir() * -Speed); }
                }
                else { ai_Me.AddForce(ai_GetDir() * Speed); }
            }
        }
        else if (ai_CurrentTime >= ai_StunTimer)
        {
            ai_IsStunned = false;
            ai_CurrentTime = 0.0f;
        }
    }

    void OnCollisionEnter(Collision lizzie)
    {
        if(lizzie.gameObject.CompareTag("Player"))
        {
            LizzieController Lizzie = lizzie.collider.GetComponent<LizzieController>();
            Vector3 force = ai_GetDir();

            force.y = 0.0f;
            ai_HasSlid = true;
            if (Lizzie != null)
            {
                Lizzie.applyHit((Damage / 100) * -ai_GetVel(), force);
                ai_Me.AddForce(ai_GetDir() * -4 * 7, ForceMode.Impulse);
            }
        }
    }
}
