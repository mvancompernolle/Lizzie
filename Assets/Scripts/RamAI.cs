using UnityEngine;

using System.Collections;

public class RamAI : MonoBehaviour
{
    public Transform Target;
    public float Speed;
    public float Jump_Force;

    private Rigidbody ai_Ram;
    private bool ai_HasJumped;

    float ai_getJumpRange()
    {
        float velocity = Mathf.Sqrt(Mathf.Pow(ai_Ram.velocity.x, 2) + Mathf.Pow(ai_Ram.velocity.z, 2));

        return velocity / 2;
    }

    Vector3 ai_GetDir()
    {
        return (Target.position - transform.position).normalized;
    }

    /* Updaters */

    void Start ()
    {
        ai_Ram = GetComponent<Rigidbody>();
        ai_HasJumped = false;
    }

    void FixedUpdate ()
    {
        if(Vector3.Distance(transform.position, Target.position) < ai_getJumpRange() && !ai_HasJumped)
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

    void OnTriggerEnter(Collider lizzie)
    {
        if(lizzie.gameObject.CompareTag("Player"))
        {
            LizzieController Lizzie = lizzie.GetComponent<LizzieController>();
            Vector3 force = ai_GetDir();

            force.y = 0.0f;
            Lizzie.applyHit((Jump_Force / 100) * (ai_getJumpRange() * 2), force);
        }
    }
}