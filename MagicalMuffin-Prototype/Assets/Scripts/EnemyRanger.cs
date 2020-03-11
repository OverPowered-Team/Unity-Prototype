using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanger : MonoBehaviour
{
    GameObject player1;
    GameObject player2;
    public GameObject BulletShell;
    public bool attack_player1 = false;
    public bool attack_player2 = false;
    public float speed = 10;
    public float range = 10;
    float constraintY = 0;
    bool can_shot = false;
    float distance_player1;
    float distance_player2;

    void Start()
    {
        player1 = GameObject.FindGameObjectWithTag("Geralt");
        player2 = GameObject.FindGameObjectWithTag("Yennefer");
        distance_player1 = Mathf.Sqrt(Mathf.Pow((player1.transform.position.x - transform.position.x), 2) + Mathf.Pow((player1.transform.position.z - transform.position.z), 2));
        distance_player2 = Mathf.Sqrt(Mathf.Pow((player2.transform.position.x - transform.position.x), 2) + Mathf.Pow((player2.transform.position.z - transform.position.z), 2));

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

        can_shot = true;
    }

    // Update is called once per frame
    void Update()
    {
        distance_player1 = Mathf.Sqrt(Mathf.Pow((player1.transform.position.x - transform.position.x), 2) + Mathf.Pow((player1.transform.position.z - transform.position.z), 2));
        distance_player2 = Mathf.Sqrt(Mathf.Pow((player2.transform.position.x - transform.position.x), 2) + Mathf.Pow((player2.transform.position.z - transform.position.z), 2));

        if (attack_player1 && distance_player1 >= range)
        {
            transform.LookAt(player1.transform);
            transform.position = Vector3.MoveTowards(transform.position, player1.transform.position, speed * Time.deltaTime);
        }
        else if(distance_player1 <= range && can_shot)
        {
            transform.LookAt(player1.transform);
            StartCoroutine("Shot");
            can_shot = false;
        }
        if (attack_player2 && distance_player2 >= range)
        {
            transform.LookAt(player2.transform);
            transform.position = Vector3.MoveTowards(transform.position, player2.transform.position, speed * Time.deltaTime);
        }
        else if(distance_player2 <= range && can_shot)
        {
            transform.LookAt(player2.transform);
            StartCoroutine("Shot");
            can_shot = false;
        }
        transform.position = new Vector3(transform.position.x, constraintY, transform.position.z);
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
        Instantiate(BulletShell, transform.position, Quaternion.identity, transform/*GameObject.FindGameObjectWithTag("EnemyFolder").transform*/);
        yield return new WaitForSeconds(2.5f);
        can_shot = true;
    }
}
