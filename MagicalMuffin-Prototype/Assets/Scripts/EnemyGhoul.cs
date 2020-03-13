using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyGhoul : MonoBehaviour
{
    //------------------------------------
    static public int ghoul_damge = 5;
    //------------------------------------
    public int forceKnockBack = 0; // Cere, este valor es la fuerza con el que se lanza al enemigo, 1 es en el sitio y 10 es muy lejos


    GameObject Geralt;
    GameObject Yennefer;
    bool attack_player1 = false;
    bool attack_player2 = false;
    public float speed = 10f;
    float constraintY = 0;

    Animator anim;
    public float attack_distance = 2;
    public float life = 30;
    Gamepad gamepad = null;

    float knockback_timer = 0;
    bool knockback = false;

    bool do_damage = false;
    float startAttackTime;
    Transform Kicker;
    bool kicked;
    Vector3 KickInFront;
    private AudioSource audioSource;


    public ParticleSystem BloodFXParticles;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        BloodFXParticles.gameObject.SetActive(false);

        // Decide Target
        Geralt = GameObject.FindGameObjectWithTag("Geralt");
        Yennefer = GameObject.FindGameObjectWithTag("Yennefer");
        float distance_player1 = Mathf.Sqrt(Mathf.Pow((Geralt.transform.position.x - transform.position.x), 2) + Mathf.Pow((Geralt.transform.position.z - transform.position.z), 2));
        float distance_player2 = Mathf.Sqrt(Mathf.Pow((Yennefer.transform.position.x - transform.position.x), 2) + Mathf.Pow((Yennefer.transform.position.z - transform.position.z), 2));

        if (distance_player1 < distance_player2)
        {
            attack_player1 = true;
            attack_player2 = false;
        }
        else
        {
            attack_player2 = true;
            attack_player1 = false;
        }
        constraintY = transform.position.y;

    }

    void Update()
    {
        if(attack_player1)
            GhoulBehaviour(Geralt);

        if (attack_player2)
            GhoulBehaviour(Yennefer);

        // ConstraintY
        transform.position = new Vector3(transform.position.x, constraintY, transform.position.z);
    }

    void GhoulBehaviour(GameObject target)
    {
        transform.LookAt(target.transform);

        // Calculate distance
        float distance_target = Mathf.Sqrt(Mathf.Pow((target.transform.position.x - transform.position.x), 2) + Mathf.Pow((target.transform.position.z - transform.position.z), 2));

        if (distance_target <= attack_distance)
        {
            anim.SetBool("AttackInRange", true);

            if (Time.time - startAttackTime > anim.GetCurrentAnimatorStateInfo(0).length && anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                do_damage = true;
                startAttackTime = Time.time;
            }

            if (do_damage)
            {
                if (target.tag == "Yennefer")
                {
                    YenneferStats yenneferStats;
                    yenneferStats = Yennefer.GetComponent<YenneferStats>();
                    yenneferStats.GetHit(ghoul_damge);

                }
                if (target.tag == "Geralt")
                    Geralt.GetComponent<GeraltStats>().GetHit(ghoul_damge);

                do_damage = false;
            }
        }
        else
        {
            // Walkaround Pathfinding
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            anim.SetBool("AttackInRange", false);

        }

        // Gethit
        if (knockback)
        {
            if(knockback_timer >= 0.3f)
            {
                knockback_timer = 0;
                knockback = false;
                anim.SetBool("GetHit", false);
                kicked = false;
            }
            else
            {
                //if (kicked)
                //{
                //    KickInFront = Kicker.forward;
                //    kicked = false;
                //}

                transform.position += transform.forward * -speed * forceKnockBack * Time.deltaTime;
                knockback_timer += Time.deltaTime;
            }
           
        }
    }

    void GetHit(float damage)
    {
        life -= damage;
        BloodFXParticles.gameObject.SetActive(true);
        BloodFXParticles.Play();
        BloodFXParticles.Emit(1);
        anim.SetBool("GetHit", true);
        knockback = true;
        audioSource.Play();
        if (life <= 0)
        {
            EnemyManager.EnemiesAlive.Remove(this.gameObject);
            Destroy(this.gameObject);
        }
    }


    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("GeraldHit") || collider.CompareTag("YenneferHit"))
        {
            playerController player = collider.transform.parent.GetComponentInParent<playerController>();
            GeraltAttacks playerCombos = collider.transform.parent.GetComponentInParent<GeraltAttacks>();

            float damage_recived = player.GetStrength().GetValue() * playerCombos.GetCurrentAttack().base_damage.GetValue();

            playerCombos.OnHit();
            GetHit(damage_recived);
        }
        //if (collision.transform.tag == "Geralt")
        //    Kicker = Geralt.transform;
        //if (collision.transform.tag == "Yennefer")
        //{
        //    Kicker = Yennefer.transform;
        //    Debug.Log(Kicker.forward);
        //}


    }
}
