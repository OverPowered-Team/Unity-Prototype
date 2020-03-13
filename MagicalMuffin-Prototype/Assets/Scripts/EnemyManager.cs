using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    static public List<GameObject> EnemiesAlive = new List<GameObject>();

    public List<GameObject> Spawners = new List<GameObject>();
    public List<GameObject> ParticleSpawners = new List<GameObject>();

    public GameObject[] relics_pool;
    public Transform relic_spawn;

    public GameObject EnemyGhoul;
    public GameObject EnemyRange;

    public GameObject EnemyFolder;

    bool next_round;
    bool round_finished;
    int round = 0;

    void Start()
    {
        round = 1;
        next_round = true;

        foreach (var particle in ParticleSpawners)
        {
            particle.SetActive(false);
        }
    }

    void Update()
    {
        if (next_round)
        {
            switch (round)
            {
                case 1:
                    StartCoroutine("Round1");
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
            SpawnRelic();
            round++;
            next_round = true;
            round_finished = false;
        }
    }

    void SpawnRelic()
    {
        int random_index = Random.Range(0, relics_pool.Length);
        Instantiate(relics_pool[random_index], relic_spawn);
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

    IEnumerator Round1()
    {
        ParticleSpawners[2].SetActive(true);
        ParticleSpawners[1].SetActive(true);
        for (int i = 0; i <= 2; ++i)
        {
            yield return new WaitForSeconds(0.75f);
            SpawnGhoul(Spawners[2]);
            SpawnGhoul(Spawners[1]);
        }
        ParticleSpawners[2].SetActive(false);
        ParticleSpawners[1].SetActive(false);

        StopCoroutine("Round1");
        round_finished = true;
    }

    IEnumerator Round2()
    {
        ParticleSpawners[2].SetActive(true);
        ParticleSpawners[1].SetActive(true);
        for (int i = 0; i <= 3; ++i)
        {
            yield return new WaitForSeconds(0.75f);
            SpawnGhoul(Spawners[2]);
            SpawnGhoul(Spawners[1]);
        }
        ParticleSpawners[2].SetActive(false);
        ParticleSpawners[1].SetActive(false);

        StopCoroutine("Round2");
        round_finished = true;

    }

    IEnumerator Round3()
    {
        ParticleSpawners[0].SetActive(true);
        ParticleSpawners[3].SetActive(true);
        for (int i = 0; i <= 2; ++i)
        {
            yield return new WaitForSeconds(0.75f);
            SpawnGhoul(Spawners[0]);
            SpawnGhoul(Spawners[3]);
        }
        ParticleSpawners[0].SetActive(false);
        ParticleSpawners[3].SetActive(false);

        ParticleSpawners[2].SetActive(true);
        ParticleSpawners[1].SetActive(true);
        yield return new WaitForSeconds(0.75f);
        SpawnRange(Spawners[1]);
        SpawnRange(Spawners[2]);
        ParticleSpawners[2].SetActive(false);
        ParticleSpawners[1].SetActive(false);

        StopCoroutine("Round3");
        round_finished = true;

    }

    //IEnumerator Round4()
    //{
    //    for (int i = 0; i <= 2; ++i)
    //    {
    //        yield return new WaitForSeconds(1.75f);
    //        SpawnRange(Spawners[0]);
    //        SpawnRange(Spawners[3]);
    //    }

    //    for (int i = 0; i <= 2; ++i)
    //    {
    //        yield return new WaitForSeconds(0.5f);
    //        SpawnGhoul(Spawners[0]);
    //        SpawnGhoul(Spawners[3]);
    //    }


    //    StopCoroutine("Round4");
    //    round_finished = true;

    //}

    //IEnumerator Round5()
    //{
    //    for (int i = 0; i <= 5; ++i)
    //    {
    //        yield return new WaitForSeconds(0.75f);
    //        SpawnGhoul(Spawners[0]);
    //        SpawnGhoul(Spawners[3]);
    //    }
        
    //    StopCoroutine("Round5");
    //    round_finished = true;

    //}

}
