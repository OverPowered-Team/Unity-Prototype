using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyGhoul : MonoBehaviour
{

    GameObject player1;
    GameObject player2;
    bool attack_player1 = false;
    bool attack_player2 = false;
    public float speed = 10;
    float constraintY = 0;

    Animator anim;
    public float attack_distance = 2;
    public int life = 30;
    Gamepad gamepad = null;

    float knockback_timer = 0;
    bool knockback = false;

    public ParticleSystem BloodFXParticles;
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        BloodFXParticles.gameObject.SetActive(false);

        //Decide Target
        player1 = GameObject.FindGameObjectWithTag("Player");
        player2 = GameObject.FindGameObjectWithTag("Player2");
        float distance_player1 = Mathf.Sqrt(Mathf.Pow((player1.transform.position.x - transform.position.x), 2) + Mathf.Pow((player1.transform.position.z - transform.position.z), 2));
        float distance_player2 = Mathf.Sqrt(Mathf.Pow((player2.transform.position.x - transform.position.x), 2) + Mathf.Pow((player2.transform.position.z - transform.position.z), 2));

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
            GhoulBehaviour(player1);

        if (attack_player2)
            GhoulBehaviour(player2);

        //ConstraintY
        transform.position = new Vector3(transform.position.x, constraintY, transform.position.z);

        
    }

    void GhoulBehaviour(GameObject target)
    {
        transform.LookAt(target.transform);

        //Calculate distance
        float distance_target = Mathf.Sqrt(Mathf.Pow((target.transform.position.x - transform.position.x), 2) + Mathf.Pow((target.transform.position.z - transform.position.z), 2));

        if (distance_target <= attack_distance)
        {
            anim.SetBool("AttackInRange", true);
        }
        else
        {
            //Walkaround Pathfinding
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            anim.SetBool("AttackInRange", false);

        }

        //Gethit
        if (knockback)
        {
            if(knockback_timer >= 0.3f)
            {
                knockback_timer = 0;
                knockback = false;
                anim.SetBool("GetHit", false);
            }
            else
            {
                transform.position += transform.forward * -speed * 7* Time.deltaTime;
                knockback_timer += Time.deltaTime;
            }
           
        }
    }

    void GetHit()
    {
        
        life -= 10; //Change this for var "player attack value"
        BloodFXParticles.gameObject.SetActive(true);
        BloodFXParticles.Play();
        BloodFXParticles.Emit(1);
        if (life <= 0)
        {
            gameObject.SetActive(false);
        }
        anim.SetBool("GetHit", true);
        knockback = true;
       
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player" || collision.transform.tag == "Player2")
        {
            GetHit();
            foreach (var item in EnemyManager.EnemiesAlive)
            {
                if (item != null)
                {
                    EnemyManager.EnemiesAlive.Remove(item);
                    break;
                }
            }
        }
    }

}
