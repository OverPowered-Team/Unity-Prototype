using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanger : MonoBehaviour
{
    GameObject Geralt;
    GameObject Yennefer;
    public GameObject BulletShell;
    public bool attack_geralt = false;
    public bool attack_yennefer = false;
    public float speed = 10;
    public float range = 10;
    float constraintY = 0;
    bool can_shot = false;
    float distance_geralt;
    float distance_yennefer;
    Animator anim;
    GameObject FollowTarget;
    public GameObject BulletPos;

    void Start()
    {
        BulletShell.SetActive(false);
        anim = GetComponentInChildren<Animator>();

        Geralt = GameObject.FindGameObjectWithTag("Geralt");
        Yennefer = GameObject.FindGameObjectWithTag("Yennefer");
        distance_geralt = Mathf.Sqrt(Mathf.Pow((Geralt.transform.position.x - transform.position.x), 2) + Mathf.Pow((Geralt.transform.position.z - transform.position.z), 2));
        distance_yennefer = Mathf.Sqrt(Mathf.Pow((Yennefer.transform.position.x - transform.position.x), 2) + Mathf.Pow((Yennefer.transform.position.z - transform.position.z), 2));

        if (distance_geralt < distance_yennefer)
        {
            attack_geralt = true;
            attack_yennefer = false;
            FollowTarget = Geralt;
        }
        else
        {
            attack_yennefer = true;
            attack_geralt = false;
            FollowTarget = Yennefer;
        }
        constraintY = transform.position.y;

        can_shot = true;
    }

    // Update is called once per frame
    void Update()
    {
        distance_geralt = Mathf.Sqrt(Mathf.Pow((Geralt.transform.position.x - transform.position.x), 2) + Mathf.Pow((Geralt.transform.position.z - transform.position.z), 2));
        distance_yennefer = Mathf.Sqrt(Mathf.Pow((Yennefer.transform.position.x - transform.position.x), 2) + Mathf.Pow((Yennefer.transform.position.z - transform.position.z), 2));


        transform.LookAt(FollowTarget.transform);


        if (attack_geralt && distance_geralt >= range)
        {
            ArcherBehaviour(Geralt);
        }
        else if(distance_geralt <= range && can_shot)
        {
            ArcherAttackBehaviour(Geralt);
        }
        if (attack_yennefer && distance_yennefer >= range)
        {
            ArcherBehaviour(Yennefer);
        }
        else if (distance_yennefer <= range)
        {
            ArcherAttackBehaviour(Yennefer);
        }



        transform.position = new Vector3(transform.position.x, constraintY, transform.position.z);
    }

    void ArcherBehaviour(GameObject target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        anim.SetBool("AttackInRange", false);
        anim.SetBool("NoAmmo", false);

    }

    void ArcherAttackBehaviour(GameObject target)
    {
        if(can_shot)
        {
            StartCoroutine("Shot");
            can_shot = false;
            anim.SetBool("AttackInRange", true);
        }
        else
        {
            anim.SetBool("NoAmmo", true);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Geralt" || collision.transform.tag == "Yennefer")
        {
            foreach (var item in EnemyManager.EnemiesAlive)
            {
                if (item != null)
                {
                    EnemyManager.EnemiesAlive.Remove(item);
                    break;
                }

            }

           gameObject.SetActive(false);
        }
    }


    IEnumerator Shot()
    {

        //Instantiate(BulletShell, transform.position, Quaternion.identity, transform/*GameObject.FindGameObjectWithTag("EnemyFolder").transform*/);
        BulletShell.SetActive(true);
        BulletShell.transform.position = BulletPos.transform.position;

        yield return new WaitForSeconds(2.5f);
        can_shot = true;
        anim.SetBool("NoAmmo", false);
    }
}
