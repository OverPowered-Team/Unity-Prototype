using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMutant : MonoBehaviour
{
    GameObject Geralt;
    GameObject Yennefer;
    bool attack_geralt = false;
    bool attack_yennefer = false;
    public float speed = 10f;

    float constraintY = 0;
    Vector2 constraintRotation;
    public float attack_distance = 2f;

    Animator anim;
    bool attack_rotation;
    float timer_attack_rotation;

    public float chargedjump_distance = 12f;
    bool can_jump;
    bool charged_jump;
    float timer_charging_jump;
    float collider_attack_timer;

    Vector3 PosJump;
    public BoxCollider ChargedJumpCollider;
    public BoxCollider BasicAttackCollider;

    void Start()
    {
        ChargedJumpCollider.enabled = false;
        BasicAttackCollider.enabled = false;

        anim = GetComponentInChildren<Animator>();
        Geralt = GameObject.FindGameObjectWithTag("Geralt");
        Yennefer = GameObject.FindGameObjectWithTag("Yennefer");
        float distance_geralt = Mathf.Sqrt(Mathf.Pow((Geralt.transform.position.x - transform.position.x), 2) + Mathf.Pow((Geralt.transform.position.z - transform.position.z), 2));
        float distance_yennefer = Mathf.Sqrt(Mathf.Pow((Yennefer.transform.position.x - transform.position.x), 2) + Mathf.Pow((Yennefer.transform.position.z - transform.position.z), 2));

        if (distance_geralt < distance_yennefer)
        {
            attack_geralt = true;
            attack_yennefer = false;
            PosJump = Geralt.transform.position;
        }
        else
        {
            attack_yennefer = true;
            attack_geralt = false;
            PosJump = Yennefer.transform.position;
        }
        constraintY = transform.position.y;

         
        can_jump = true;

        // Constraint Rotation X and Y
        constraintRotation = new Vector2(transform.rotation.x, transform.rotation.z);
        
    }

    void Update()
    {

        if (attack_geralt)
        {
            MutantBehaviour(Geralt);
           
        }
        if (attack_yennefer)
        {
            MutantBehaviour(Yennefer);
        }


        // Constrains
        transform.position = new Vector3(transform.position.x, constraintY, transform.position.z);
        transform.rotation = new Quaternion(constraintRotation.x, transform.rotation.y, constraintRotation.y, transform.rotation.w);




        //playerX discance > Y Retarget(here enemy will compare player distances, the lowest one will win "attack_playerZ = true")
    }

    void MutantBehaviour(GameObject target)
    {

        // Calculate distance
        float distance = Mathf.Sqrt(Mathf.Pow((target.transform.position.x - transform.position.x), 2) + Mathf.Pow((target.transform.position.z - transform.position.z), 2));

        // Charged Jump 
        if(distance >= chargedjump_distance)
        {
            //ReTarget();

            can_jump = true;

            if (timer_charging_jump >= 2)
            {
                timer_charging_jump = 0;
                PosJump = target.transform.position;
                charged_jump = true;
            }
            else
            {
                timer_charging_jump += Time.deltaTime;
                //TODO Particles
                anim.SetBool("JumpInRange", true);
                LerpRotation(target);

            }
        }

        // Attack and Movement
        else if(!can_jump)
        {
            if (distance <= attack_distance)
            {
                anim.SetBool("AttackInRange", true);


                float timer = Time.time - collider_attack_timer;

                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    BasicAttackCollider.enabled = false;
                    LerpRotation(target);
                    collider_attack_timer = Time.time;
                }
                else if (timer > anim.GetCurrentAnimatorStateInfo(0).length/2.5f && anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    collider_attack_timer = Time.time;
                    BasicAttackCollider.enabled = true;
                }
            }
            else
            {
                anim.SetBool("AttackInRange", false);

                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    LerpRotation(target);

                    // Move Towards
                    transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
                }
            }
        }


        // Mutant Ultra Jump
        if(can_jump && charged_jump)
        {
            transform.position = Vector3.MoveTowards(transform.position, PosJump, speed *2* Time.deltaTime);
            ChargedJumpCollider.enabled = true;

            float distance_PosJump = Mathf.Sqrt(Mathf.Pow((PosJump.x - transform.position.x), 2) + Mathf.Pow((PosJump.z - transform.position.z), 2));
            if (distance_PosJump <= 1)
            {
                can_jump = false;
                charged_jump = false;
                anim.SetBool("JumpInRange", false);
                ChargedJumpCollider.enabled = false;

            }
        }

    }

    void ReTarget()
    {
        attack_geralt = !attack_geralt;
        attack_yennefer = !attack_yennefer;
    }

    void LerpRotation(GameObject target)
    {
        // Look at Lerp
        Vector3 relativePos = target.transform.position - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 5 * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Geralt" || collision.transform.tag == "Yennefer")
        {
            foreach (var item in EnemyManager.EnemiesAlive)
            {
                if (item != null)
                {
                   // EnemyManager.EnemiesAlive.Remove(item);
                    break;
                }

            }

            //gameObject.SetActive(false);
        }
    }
}
