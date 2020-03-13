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
    public GameObject EnemyMiniBoss;

    public GameObject EnemyFolder;

    bool next_round;
    bool finishedSpawning;
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
            StartCoroutine("Round");
            next_round = false;
        }

        if(EnemiesAlive.Count <= 0 && finishedSpawning)
        {
            SpawnRelic();
            round++;
            next_round = true;
            finishedSpawning = false;
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
        Instantiate(EnemyMiniBoss, spawnPosition.transform.position, Quaternion.identity, EnemyFolder.transform);
        EnemiesAlive.Add(EnemyRange);
    }

    IEnumerator Round()
    {
        //Activate two random spawners
        GameObject[] randomSpawn = new GameObject[2];
        randomSpawn[0] = ParticleSpawners[Random.Range(0, Spawners.Count)];
        randomSpawn[1] = ParticleSpawners[Random.Range(0, Spawners.Count)];

        randomSpawn[0].SetActive(true);
        randomSpawn[1].SetActive(true);

        //Number of ghouls
        int ghoulNum = (int)Mathf.Floor((float)round * 2f);
        for (int i = 0; i < ghoulNum; ++i)
        {
            SpawnGhoul(randomSpawn[Random.Range(0, randomSpawn.Length)]);
        }

        //Number of ranged
        int rangedNum = (int)Mathf.Floor(((float)round - 1f) * 0.5f);
        for (int i = 0; i < rangedNum; ++i)
        {
            SpawnRange(randomSpawn[Random.Range(0, randomSpawn.Length)]);
        }

        //Number of mutant
        int mutantNum = (int)Mathf.Floor((float)round * 0.20f);
        for (int i = 0; i < mutantNum; ++i)
        {
            SpawnMiniBoss(randomSpawn[Random.Range(0, randomSpawn.Length)]);
        }
        yield return new WaitForSeconds(0.75f);

        randomSpawn[0].SetActive(false);
        randomSpawn[1].SetActive(false);

        finishedSpawning = true;
        StopCoroutine("Round");
    }
}
