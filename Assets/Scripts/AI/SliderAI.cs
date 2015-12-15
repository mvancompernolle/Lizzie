﻿using UnityEngine;
using System.Collections;

public class SliderAI : MonoBehaviour
{
    private GameObject player;
    public Transform Target;
    public float SlideForce;
    public float Speed;
    public float SlideInterval = 2.0f;

    private Rigidbody ai_Slider;
    private float ai_StunTimer;
    private float ai_CurrentTime = 0.0f;
    private bool ai_HasSlid;
    private bool ai_HitLizzie;
    private bool ai_IsStunned = false;
    private bool playerSet = false;

    /* Setters */

    public void ai_Stun(float duration)
    {
        ai_IsStunned = true;
        ai_StunTimer = duration;
    }

    public void ai_ApplyHit(Vector3 force)
    {
        ai_Slider.AddForce(force, ForceMode.Impulse);
    }

    /* Getters */
    public Rigidbody ai_GetRBody() { return ai_Slider; }

    private Vector3 ai_GetDirection() { return (Target.position - transform.position).normalized; }
    private float ai_GetVelocity() { return Mathf.Sqrt(Mathf.Pow(ai_Slider.velocity.x, 2) + Mathf.Pow(ai_Slider.velocity.z, 2)); }
    private float ai_GetDistance() { return Vector3.Distance(transform.position, Target.position); }

    /* Updaters */
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Target = player.GetComponent<Rigidbody>().transform;
        ai_Slider = GetComponent<Rigidbody>();
        ai_HasSlid = false;
    } 

    void FixedUpdate()
    {
        if (!playerSet)
        {
            LizzieController lizzieController = player.GetComponent<LizzieController>();
            lizzieController.bulletTargets.Add(gameObject);
            playerSet = true;
        }

        if(!ai_IsStunned)
        {
            if (ai_GetDistance() < ai_GetVelocity() / 2 && !ai_HasSlid)
            {
                Vector3 dontGoUp = ai_GetDirection();
                dontGoUp.y = 0.0f;
                ai_Slider.AddForce(dontGoUp * SlideForce, ForceMode.Impulse);
            }
            else
            {
                if (ai_HasSlid)
                {
                    if (ai_GetDistance() >= 20) { ai_HasSlid = false; }
                    else
                    {
                        ai_Slider.AddForce(ai_GetDirection() * -Speed);
                    }
                }
                else { ai_Slider.AddForce(ai_GetDirection() * Speed); }
            }
        }
        else if(ai_CurrentTime >= ai_StunTimer)
        {
            ai_IsStunned = false;
            ai_CurrentTime = 0.0f;
        }
    }

    void OnCollisionEnter(Collision lizzie)
    {
        //hotdog
        if(lizzie.gameObject.CompareTag("Player") && !ai_HitLizzie)
        {
            LizzieController Lizzie = lizzie.collider.GetComponent<LizzieController>();
            Vector3 force = ai_GetDirection();

            force.y = 0.0f;
            ai_HasSlid = true;
            float hVal = new Vector3(ai_Slider.velocity.x, 0f, ai_Slider.velocity.z).magnitude;
            Lizzie.applyHit((SlideForce / 100) * -hVal, force);
            ai_Slider.AddForce(ai_GetDirection() * -4 * 7, ForceMode.Impulse);
        }
    }
}