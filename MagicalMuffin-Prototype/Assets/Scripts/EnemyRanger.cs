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
    float arrow_attack_timer;
    bool prepare_arrow;
    Vector2 constraintRotation;

    public float life = 50;
    bool knockback = false;
    public ParticleSystem BloodFXParticles;
    public ParticleSystem FireFXParticles;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        BloodFXParticles.gameObject.SetActive(false);

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


        // Constraint Rotation X and Y
        constraintRotation = new Vector2(transform.rotation.x, transform.rotation.z);
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
        else if(distance_geralt <= range)
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
        transform.rotation = new Quaternion(constraintRotation.x, transform.rotation.y, constraintRotation.y, transform.rotation.w);

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
            can_shot = false;
            anim.SetBool("AttackInRange", true);
            prepare_arrow = true;

        }
        else
        {
            anim.SetBool("NoAmmo", true);
        }

        if(prepare_arrow)
        {
            float timer = Time.time - arrow_attack_timer;
            if (timer > anim.GetCurrentAnimatorStateInfo(0).length && anim.GetCurrentAnimatorStateInfo(0).IsName("Shot"))
            {
                arrow_attack_timer = Time.time;
                Instantiate(BulletShell, new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z), Quaternion.identity, transform/*GameObject.FindGameObjectWithTag("EnemyFolder").transform*/);

                can_shot = true;
                anim.SetBool("NoAmmo", false);

                prepare_arrow = false;
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

    public void OnFire()
    {
        FireFXParticles.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "GeraldHit" || collider.transform.tag == "YenneferHit")
        {
            playerController player = collider.gameObject.GetComponentInParent<playerController>();
            GeraltAttacks playerCombos = collider.gameObject.GetComponentInParent<GeraltAttacks>();

            float damage_recived = player.GetStrength().GetValue() * playerCombos.GetCurrentAttack().base_damage.GetValue();

            playerCombos.OnHit(this.gameObject);
            GetHit(damage_recived);

            if (life <= 0)
            {
                EnemyManager.EnemiesAlive.Remove(this.gameObject);
            }
        }
        //if (collision.transform.tag == "Geralt")
        //    Kicker = Geralt.transform;
        //if (collision.transform.tag == "Yennefer")
        //{
        //    Kicker = Yennefer.transform;
        //    Debug.Log(Kicker.forward);
        //}


    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.transform.tag == "Geralt" || collision.transform.tag == "Yennefer")
    //    {
    //        foreach (var item in EnemyManager.EnemiesAlive)
    //        {
    //            if (item != null)
    //            {
    //                EnemyManager.EnemiesAlive.Remove(item);
    //                break;
    //            }

    //        }

    //       gameObject.SetActive(false);
    //    }
    //}


    IEnumerator Shot()
    {
        BulletShell.SetActive(true);
        Instantiate(BulletShell, new Vector3(transform.position.x, transform.position.y - 2, transform.position.z) , Quaternion.identity, transform/*GameObject.FindGameObjectWithTag("EnemyFolder").transform*/);

        yield return new WaitForSeconds(2.5f);
        can_shot = true;
        anim.SetBool("NoAmmo", false);
    }
}
