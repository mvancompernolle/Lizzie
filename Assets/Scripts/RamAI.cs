using UnityEngine;

using System.Collections;

public class RamAI : MonoBehaviour
{
    public Transform Target;
    public float Speed;
    public float Jump_Force;

    private Rigidbody ai_Ram;
    private bool ai_HasJumped;

    float getJumpRange()
    {
        float velocity = Mathf.Sqrt(Mathf.Pow(ai_Ram.velocity.x, 2) + Mathf.Pow(ai_Ram.velocity.z, 2));

        return velocity / 2;
    }

    Vector3 ai_GetDir()
    {
<<<<<<< HEAD
        return (Target.position - transform.position).normalized;
=======
        //Determines distance between the rammer and Lizzie using the X and Z Axies
        float dist = Mathf.Sqrt(Mathf.Pow(ai_Position.x - ai_TargetPos.x, 2) + Mathf.Pow(ai_Position.z - ai_TargetPos.z, 2));

        if(ai_HasJumped)
        {
            return -(ai_hVel);
        }

        if (dist <= Min_Jump_Dist && ai_CanJump)
        {
            if(ai_xVel != 0 || ai_zVel != 0)
            {
                if (ai_LastDist < dist)
                {
                    ai_HasJumped = true;
                    ai_CanJump = false;
                }

                ai_LastDist = dist;

                return (ai_hVel / 2) + 10; //10 is minimum to resist gravity, thus, I add it automatically.
            }
        }

        return 0.0f;
>>>>>>> origin/master
    }

    /* Updaters */

    void Start ()
    {
        ai_Ram = GetComponent<Rigidbody>();
        ai_HasJumped = false;
    }

    void FixedUpdate ()
    {
        if(Vector3.Distance(transform.position, Target.position) < getJumpRange() && !ai_HasJumped)
        {
            ai_Ram.AddForce(Vector3.up * Jump_Force, ForceMode.Impulse);
            ai_HasJumped = true;
        }
        else
        {
            ai_Ram.AddForce(ai_GetDir() * Speed);
        }
    }

    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            ai_HasJumped = false;
        }
    }
}