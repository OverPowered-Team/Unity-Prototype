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

        if (parentRanger.attack_geralt)
        {
            transform.LookAt(GameObject.FindGameObjectWithTag("Geralt").transform);
            transform.SetParent(EnemyFolder.transform);
        }
        if (parentRanger.attack_yennefer)
        {
            transform.LookAt(GameObject.FindGameObjectWithTag("Yennefer").transform);
            transform.SetParent(EnemyFolder.transform);
        }


    }

    void Update()
    {
       
        transform.position += transform.forward * speed * Time.deltaTime;

        transform.position = new Vector3(transform.position.x, constrainY, transform.position.z);

        transform.rotation = new Quaternion(0, transform.rotation.y, transform.rotation.z, transform.rotation.w);
    }
}
