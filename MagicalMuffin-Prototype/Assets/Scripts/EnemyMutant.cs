using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMutant : MonoBehaviour
{
    GameObject player1;
    GameObject player2;
    bool attack_player1 = false;
    bool attack_player2 = false;
    public float speed = 10;
    float constraintY = 0;

    void Start()
    {
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
        if (attack_player1)
        {
            transform.LookAt(player1.transform);
            transform.position = Vector3.MoveTowards(transform.position, player1.transform.position, speed * Time.deltaTime);
        }
        if (attack_player2)
        {
            transform.LookAt(player2.transform);
            transform.position = Vector3.MoveTowards(transform.position, player2.transform.position, speed * Time.deltaTime);
        }
        transform.position = new Vector3(transform.position.x, constraintY, transform.position.z);

        //Todo: if playerX discance > Y Retarget(here enemy will compare player distances, the lowest one will win "attack_playerZ = true")
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player" || collision.transform.tag == "Player2")
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
}
