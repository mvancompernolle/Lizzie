using UnityEngine;
using System.Collections;

public class ShooterAI : MonoBehaviour
{
    public Transform Target;
    public float Range = 20.0f;
    public float ProjectileSpeed = 5.0f;
    public float ShootInterval = 2.0f;
    public GameObject Projectile;
    public bool OverrideProjectileStats; public float DamageOverride;
    public float TimerOverride;
    public bool UseAmmo;
    public int Ammo;

    private GameObject ai_Shooter;
    private Transform ai_BulletSpawn;
    private float ai_CurrentTime;
    private int ai_CurrentAmmo = -1;
    private bool ai_HasAmmo = true;

    /* Getters */

    private float ai_GetDistToTarget() { return Vector3.Distance(Target.position, transform.position); }
    private Vector3 ai_GetShotDirection() { return (Target.position - transform.position).normalized; }

    /* Thinky Bits */

    private bool ai_CanShoot()
    {
        if (ai_CurrentAmmo <= 0) { ai_HasAmmo = false; return false; }
        if (ai_GetDistToTarget() <= Range) { return true; }
        return false;
    }

    private void ai_Fire()
    {
        if(UseAmmo && ai_HasAmmo) { ai_CurrentAmmo--; }
        Debug.Log(ai_CurrentAmmo);

        Vector3 shootHere = ai_GetShotDirection();

        GameObject newBullet = Instantiate(Projectile, ai_BulletSpawn.position, transform.rotation) as GameObject;
        newBullet.transform.position = ai_BulletSpawn.position;
        newBullet.GetComponent<Rigidbody>().AddForce(shootHere * ProjectileSpeed, ForceMode.Impulse);

        //checks to see if the tower is shooting an AI
        if (newBullet.GetComponent<LatcherAI>() != null) { newBullet.GetComponent<LatcherAI>().Target = FindObjectOfType<LizzieController>().GetComponent<Rigidbody>(); }

        //Overrides any and all stats specified as an override
        if (OverrideProjectileStats)
        {
            if (newBullet.GetComponent<BulletController>() != null) { newBullet.GetComponent<BulletController>().Damage = DamageOverride; }
            else if(newBullet.GetComponent<LatcherAI>() != null) { newBullet.GetComponent<LatcherAI>().JumpForce = DamageOverride; }

            if (newBullet.GetComponent<DestroyByTimer>() != null) { newBullet.GetComponent<DestroyByTimer>().timeLeft = TimerOverride; }
            else { newBullet.AddComponent<DestroyByTimer>().timeLeft = TimerOverride; }
        }

        ai_CurrentTime = 0.0f;
    }

    /* Updaters */

    void Start()
    {
        if(UseAmmo) { ai_CurrentAmmo = Ammo; }
        ai_BulletSpawn = GetComponent<Transform>();
        ai_BulletSpawn = ai_Shooter.transform.FindChild("BulletSpawn").transform;
        ai_Shooter = GetComponent<GameObject>();
        Projectile = GetComponent<GameObject>();
    }

    void Update()
    {
        if(ai_CanShoot() && ai_CurrentTime >= ShootInterval) { ai_Fire(); }
        else { ai_CurrentTime += Time.deltaTime; }
    }
}
