using UnityEngine;
using System.Collections;

/************************************************************************************\
 *    This fleshed out BulletController (looking at you, Matt) is a general purpose *
 *  script, and allows anything that spawns a projectile to not need to have its    *
 *  own internal reasoning. However, it should be noted that this general purpose   *
 *  approach needs to be updated for each new entity using a unique tag. If you add *
 *  a new tag, please add it to the referencetable below if it is used for bullet   *
 *  collision.                                                                      *
 *    Another side-effect is that any bullet spawned by anything can collide with   *
 *  ANYTHING ELSE in this approach. I plan to add an ignore system in, so you can   *
 *  declare tags to exclude from the collision logic. Until then, I wouldn't make   *
 *  liberal use of this if bullet-entity interaction gets more complicated than     *
 *  an "applyHit()".                                                                *
\************************************************************************************/

public class BulletController : MonoBehaviour
{ 
    public Collider Origin;
    public float Damage;

    private Rigidbody blt_Me;
    private bool blt_DespawnNextFrame = false;

    /* Getters */

    private Vector3 blt_GetDirection(Vector3 target) { return (target - transform.position).normalized; }
    private float blt_GetHorVel() { return Mathf.Sqrt(Mathf.Pow(blt_Me.velocity.x, 2) + Mathf.Pow(blt_Me.velocity.z, 2));  }

    /* Updaters */

    void Update()
    {
        if(blt_DespawnNextFrame) { Destroy(blt_Me); }
    }

	void OnCollisionEnter(Collision blerg)
    {
        /*****************************************************************\
         *  'blerg' is checked against 'Origin' so that the bullet won't *
         *   collide with the thing that is shooting it                  *
        \*****************************************************************/

        if (blerg.collider != Origin)
        {
            if (blerg.gameObject.CompareTag("Player")) //Lizzie will be lizzie. 
            {
                LizzieController plyr_Lizzie = blerg.collider.GetComponent<LizzieController>();
                Vector3 force = blt_GetDirection(blerg.transform.position);

                force.y = 0.0f;
                blt_DespawnNextFrame = true;
                plyr_Lizzie.applyHit((Damage / 100) * (blt_GetHorVel()), force);
            }
            else if (blerg.gameObject.CompareTag("Rammer")) //Rams receive a pretty powerful kick
            {
                RamAI ai_Ram = blerg.collider.GetComponent<RamAI>();
                Vector3 direction = blt_GetDirection(ai_Ram.ai_GetPos());

                blt_DespawnNextFrame = true;
                ai_Ram.applyHit(Damage, direction);
            }
            else if(blerg.gameObject.CompareTag("Slider")) //Sliders get stunned
            {
                Vector3 force = blt_Me.velocity;
                SliderAI ai_Slider = blerg.collider.GetComponent<SliderAI>();

                force.x = force.z = 0.0f;
                blt_Me.AddForce(force, ForceMode.Impulse);
                ai_Slider.ai_Stun(5.0f);
                force = blt_Me.velocity; force.y = 0.0f;
                ai_Slider.ai_ApplyHit(force * Damage);
                blt_DespawnNextFrame = true;
            }
            else if(blerg.gameObject.CompareTag("Latcher")) //When hit, Latchers become connected to the bullet
            {
                LatcherAI ai_Latcher = blerg.collider.GetComponent<LatcherAI>();
                Rigidbody rb_Latcher = ai_Latcher.ai_GetRBody().GetComponent<Rigidbody>();
                Vector3 force = rb_Latcher.velocity;

                force.y = 0.0f;
               // ai_Latcher.ai_SetParent(blt_Me.transform);
                blt_Me.AddForce(force * ai_Latcher.JumpForce, ForceMode.Impulse);
            }
        }
    }
}
