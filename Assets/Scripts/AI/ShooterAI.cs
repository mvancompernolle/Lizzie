using UnityEngine;
using System.Collections;

public class ShooterAI : EnemyController
{
    public float Range = 20.0f;
    public float ProjectileSpeed = 5.0f;
    public float ShootInterval = 2.0f;
    public GameObject Projectile;
    public bool ProjectileHasLifetime = true;
    public float ProjectileLifetime = 5.0f;
    public bool UseAmmo = false;
    public int Ammo;

    private Transform ai_BulletSpawn;
    private float ai_CurrentTime;
    private int ai_CurrentAmmo = 0;
    private bool ai_HasAmmo = true;

    /* Thinky Bits */

    private bool ai_CanShoot()
    {
        if (ai_CurrentAmmo <= 0) { ai_HasAmmo = false; return false; }
        if (ai_GetDistToTar() <= Range) { return true; }
        return false;
    }

    private void ai_Fire()
    {
        if(UseAmmo && ai_HasAmmo) { ai_CurrentAmmo--; }

        GameObject newBullet = Instantiate(Projectile, ai_BulletSpawn.position, transform.rotation) as GameObject;
        newBullet.transform.position = ai_BulletSpawn.position;
        newBullet.GetComponent<Rigidbody>().AddForce(ai_GetDir() * ProjectileSpeed, ForceMode.Impulse);

        if (newBullet.GetComponent<BulletController>() != null) { newBullet.GetComponent<BulletController>().Damage = Damage; }
        else if (newBullet.GetComponent<LatcherAI>() != null) { newBullet.GetComponent<LatcherAI>().Damage = Damage; }

        //Overrides any and all stats specified as an override
        if (ProjectileHasLifetime)
        {
            if (newBullet.GetComponent<DestroyByTimer>() != null) { newBullet.GetComponent<DestroyByTimer>().timeLeft = Damage; }
            else { newBullet.AddComponent<DestroyByTimer>().timeLeft = ProjectileLifetime; }
        }

        ai_CurrentTime = 0.0f;
    }

    /* Updaters */

    void Start()
    {
        if(UseAmmo) { ai_CurrentAmmo = Ammo; }
        ai_BulletSpawn = GetComponent<Transform>();
        ai_BulletSpawn = ai_Me.transform.FindChild("BulletSpawn").transform;
        Projectile = GetComponent<GameObject>();
    }

    void Update()
    {
        if(ai_CanShoot() && ai_CurrentTime >= ShootInterval) { ai_Fire(); }
        else { ai_CurrentTime += Time.deltaTime; }
    }
}
