using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    static public List<GameObject> EnemiesAlive = new List<GameObject>();
    public GameObject Spawner1;
    public GameObject Spawner2;
    public GameObject Spawner3;
    public GameObject Spawner4;

    public GameObject EnemyGhoul;
    public GameObject EnemyRange;
    //public GameObject EnemyMiniBoss;

    public GameObject EnemyFolder;

    bool next_round;
    bool round_finished;
    int round = 0;

    void Start()
    {
        round = 1;
        next_round = true;
    }

    void Update()
    {
        if(next_round)
        {
            switch (round)
            {
                case 1:
                    SpawnGhoul(Spawner2);
                    SpawnGhoul(Spawner3);
                    round_finished = true;
                    break;
                case 2:
                    StartCoroutine("Round2");
                    break;
                case 3:
                    StartCoroutine("Round3");
                    break;
                case 4:

                    break;
                case 5:

                    break;
                case 6:

                    break;
                case 7:

                    break;
            }
            next_round = false;
        }

        if(EnemiesAlive.Count <= 0 && round_finished)
        {
            round++;
            next_round = true;
            round_finished = false;
        }
    }

    void SpawnGhoul(GameObject spawnPosition)
    {
        Instantiate(EnemyGhoul, spawnPosition.transform.position, Quaternion.identity,EnemyFolder.transform);
        EnemiesAlive.Add(EnemyGhoul);
    }

    void SpawnRange(GameObject spawnPosition)
    {
        Instantiate(EnemyRange, spawnPosition.transform.position, Quaternion.identity, EnemyFolder.transform);
        EnemiesAlive.Add(EnemyRange);
        EnemyRange.SetActive(true);

    }

    void SpawnMiniBoss(GameObject spawnPosition)
    {
        //Instantiate(EnemyMiniBoss, spawnPosition.transform.position, Quaternion.identity, EnemyFolder.transform);
    }

    IEnumerator Round2()
    {
        for (int i = 0; i <= 3; ++i)
        {
            yield return new WaitForSeconds(0.75f);
            SpawnGhoul(Spawner3);
            SpawnGhoul(Spawner2);
        }

        StopCoroutine("Round2");
        round_finished = true;

    }

    IEnumerator Round3()
    {
        for (int i = 0; i <= 2; ++i)
        {
            yield return new WaitForSeconds(0.75f);
            SpawnGhoul(Spawner1);
            SpawnGhoul(Spawner4);
        }

        yield return new WaitForSeconds(0.75f);
        SpawnRange(Spawner2);
        SpawnRange(Spawner3);
        

        StopCoroutine("Round3");
        round_finished = true;

    }

    IEnumerator Round4()
    {
        for (int i = 0; i <= 2; ++i)
        {
            yield return new WaitForSeconds(1.75f);
            SpawnRange(Spawner1);
            SpawnRange(Spawner4);
        }

        for (int i = 0; i <= 2; ++i)
        {
            yield return new WaitForSeconds(0.5f);
            SpawnGhoul(Spawner1);
            SpawnGhoul(Spawner4);
        }


        StopCoroutine("Round3");
        round_finished = true;

    }

    IEnumerator Round5()
    {
        for (int i = 0; i <= 5; ++i)
        {
            yield return new WaitForSeconds(0.75f);
            SpawnGhoul(Spawner1);
            SpawnGhoul(Spawner4);
        }
        
        StopCoroutine("Round3");
        round_finished = true;

    }

}
