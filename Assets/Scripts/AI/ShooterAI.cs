using UnityEngine;
using System.Collections;

public class ShooterAI : MonoBehaviour
{
    public Transform Target;
    public float Range = 20.0f;
    public float ShootInterval = 2.0f;
    public bool UseAmmo = false;
    public int Ammo;
    public bool CanTripLizzie = false;
    public Rigidbody Projectile;
    public bool OverrideProjectileStats;
    public float DamageOverride;
    public float SpeedOverride;

    private Rigidbody ai_Shooter;
    private int ai_CurrentAmmo = -1;
    private bool ai_HasShot = false;
    private bool ai_HasAmmo = false;

    /* Getters */

    

    private float ai_GetDistToTarget() { return Vector3.Distance(Target.position, transform.position); }
    private Vector3 ai_GetShotDirection() { return (Target.position - transform.position).normalized; }

    /* Thinky Bits */

    private bool ai_CanShoot()
    {
        if(ai_GetDistToTarget() <= Range) { return true; }
        return false;
    }

    private void ai_Fire()
    {
        if(UseAmmo)
        {
            if(ai_HasAmmo) { ai_CurrentAmmo--; }
            else if(ai_CurrentAmmo == -1) { ai_CurrentAmmo = Ammo; }
        }

        Vector3 shootHere = ai_GetShotDirection();


    }

    /* Updaters */

    void Update()
    {
        if(ai_CanShoot()) { ai_Fire(); }
    }
}
