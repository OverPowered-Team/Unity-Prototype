using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    float constrainY = 0;
    float speed = 7;
    EnemyRanger parentRanger;
    GameObject EnemyFolder;
    void Start()
    {
        EnemyFolder = GameObject.FindGameObjectWithTag("EnemyFolder");
        constrainY = transform.position.y;
        parentRanger = GetComponentInParent<EnemyRanger>();

        if (parentRanger.attack_player1)
        {
            transform.LookAt(GameObject.FindGameObjectWithTag("Geralt").transform);
            transform.SetParent(EnemyFolder.transform);
        }
        if (parentRanger.attack_player2)
        {
            transform.LookAt(GameObject.FindGameObjectWithTag("Yennefer").transform);
            transform.SetParent(EnemyFolder.transform);
        }
    }

    void Update()
    {
       
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
