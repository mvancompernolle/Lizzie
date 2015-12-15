using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public float Speed;
    public float Damage;
    public bool WaitForLizzie;
    public float TriggerDistance;

    protected Rigidbody ai_Me;
    protected Transform ai_Target;

    private GameObject plyr_Player;

    /* Setters */

    public void ai_ApplyHit(Vector3 force) { ai_Me.AddForce(force, ForceMode.Impulse); }

    /* Getters */

    public Rigidbody ai_GetRBody() { return ai_Me; }
    public Vector3 ai_GetDir() { return (ai_Target.position - ai_Me.position).normalized; }

    protected float ai_GetVel() { return new Vector3(ai_Me.velocity.x, 0.0f, ai_Me.velocity.z).magnitude; }
    protected float ai_GetDistToTar() { return Vector3.Distance(ai_Me.transform.position, ai_Target.transform.position); } 
    protected float ai_GetAttackRange()
    {
        if(WaitForLizzie) { return TriggerDistance; }
        return ai_GetVel() / 2;
    }

    /* Updaters */

    public virtual void Start ()
    {
        LizzieController lizzieController;

        plyr_Player = GameObject.FindGameObjectWithTag("Player");
        ai_Target = plyr_Player.GetComponent<Rigidbody>().transform;

        lizzieController = plyr_Player.GetComponent<LizzieController>();
        lizzieController.bulletTargets.Add(gameObject);

        ai_Me = GetComponent<Rigidbody>();
    }
}
