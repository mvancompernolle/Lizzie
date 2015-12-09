using UnityEngine;
using System.Collections;

public class RamAI : MonoBehaviour
{
    public Rigidbody Target;
    public float Speed;
    public float Max_Jump_Power;
    public float Damage;
    public float Min_Jump_Dist;

    private Rigidbody ai_Ram;
    private Vector3 ai_TargetPos;
    private Vector3 ai_Position;
    private float ai_xVel;
    private float ai_zVel;
    private float ai_yVel;
    private float ai_hVel; //Horizontal Velocity
    private float ai_LastDist;
    private bool ai_CanJump;
    private bool ai_HasJumped;

    /* Functions for Calculations */

    float abs(float f)
    {
        if (f >= 0) { return f; }
        else { return f * -1; }
    }

    /************************************************\
     *   Changes the velocity to stay positive or   *
     *   be negative depedning on Lizzie's position *
     *   relative to the ram.                       *
     *                                              *
     *   0: X-Axis  1: Z-Axis                       *
    \************************************************/
    int ai_GetDirection(bool axisVel)
    {
        float diff = 0;

        switch(axisVel)
        {
            case false:
                diff = ai_Position.x - ai_TargetPos.x;
                break;
            case true:
                diff = ai_Position.z - ai_TargetPos.z;
                break;
        }

        if (diff < 0) { return 1; }
        else if (diff > 0) { return -1; }
        else { return 0; }
    }

    float ai_GetJumpVel()
    {
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

                return ai_hVel + 10; //10 is minimum to resist gravity, thus, I add it automatically.
            }
        }

        return 0.0f;
    }

    /* Updaters */

    //Called every frame to give the AI its position
    //and the location of its target.
    void ai_Update()
    {
        ai_TargetPos = Target.transform.position;
        ai_Position = transform.position;

        //This makes it so the AI can't maneuver unless it's on the ground.
        if(!ai_HasJumped)
        {
            ai_xVel = Speed * ai_GetDirection(false);
            ai_zVel = Speed * ai_GetDirection(true);
        }

        ai_yVel = ai_GetJumpVel();
        ai_hVel = Mathf.Sqrt(Mathf.Pow(ai_Ram.velocity.x, 2) + Mathf.Pow(ai_Ram.velocity.z, 2));

        if(ai_hVel > Max_Jump_Power) { ai_hVel = Max_Jump_Power; }
    }

    void Start ()
    {
        ai_CanJump = true;
        ai_HasJumped = false;
        ai_LastDist = Min_Jump_Dist + 1;
        ai_Ram = GetComponent<Rigidbody>();
        ai_Update();
    }
	
	void FixedUpdate ()
    {
        ai_Update();
        ai_Ram.AddForce(ai_xVel, ai_yVel, ai_zVel);
    }

    /* Collision Logic */

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            
        }

        if(other.gameObject.CompareTag("Floor"))
        {
            ai_CanJump = true;
            ai_HasJumped = false;
            ai_LastDist = Min_Jump_Dist + 1;
        }
    }
}