using UnityEngine;
using System.Collections;

/*****************************************************************************\
 *  Note:                                                                    *
 *                                                                           *
 *  Latchers DO NOT modify Lizzie's behavior or "physics" in any way... YET  *
\*****************************************************************************/

public class LatcherAI : EnemyController
{
    public float LatchStrength = 5;
    
    private Vector3 ai_Offset;
    private Transform ai_OT; //This PKMN's OT
    private Quaternion ai_Rotation;
    private bool ai_HasLatched = false;
    private bool ai_HasJumped = false;
    private bool ai_HitLizzie = false;

    /* Setters */

    public void ai_SetParent(Transform parent) { ai_Me.transform.parent = parent; }

    /* Updaters */

    public override void Start()
    {
        base.Start();

        if (ai_Me.transform.parent == null)
        {
            ai_OT = GameObject.Find("Enemies/Latchers").transform;
            transform.parent = ai_OT;
        }
        else
        {
            ai_OT = ai_Me.transform.parent;
        }
        ai_Rotation = transform.rotation;
    }

    void FixedUpdate()
    {
        if (ai_GetDistToTar() < ai_GetAttackRange() && !ai_HasJumped)
        {
            ai_Me.AddForce(Vector3.up * 3, ForceMode.Impulse);
            WaitForLizzie = false;
            ai_HasJumped = true;
        }
        else if (ai_HitLizzie && !ai_HasJumped)
        {
            if (ai_GetDistToTar() > 20) { ai_HitLizzie = false; }
            ai_Me.AddForce(ai_GetDir() * -Speed);
        }
        else
        {
            ai_Me.AddForce(ai_GetDir() * Speed);
        }
    }

    void Update()
    {
        if (!ai_HasLatched)
        {
            

            ai_Me.transform.parent = ai_OT;
            ai_Me.useGravity = true;
        }
        else { ai_Me.useGravity = false; }
    }

    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Floor")) { ai_HasJumped = false; ai_HasLatched  = false; ai_Me.rotation = ai_Rotation; }
    }

    void OnCollisionEnter(Collision lizzie)
    {
        if (lizzie.gameObject.CompareTag("Player") && !ai_HasLatched)
        {
            LizzieController Lizzie = lizzie.collider.GetComponent<LizzieController>();
            Vector3 force = ai_GetDir();

            force.y = 0.0f;
            ai_HitLizzie = true;
            ai_HasLatched = true;
            ai_SetParent(lizzie.transform);
            if(Lizzie != null) { Lizzie.applyHit((Damage / 100) * ai_GetVel(), force); }
        }
    }
}
