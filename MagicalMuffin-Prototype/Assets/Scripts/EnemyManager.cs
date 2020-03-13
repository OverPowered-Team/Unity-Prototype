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

    public float timeBetweenRounds;
    private bool startedTimeBetwenRounds = false;
    private float startTimeBetweenRounds;

    private float spawnOffset = 5f;

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

        Debug.Log(EnemiesAlive.Count);

        if (EnemiesAlive.Count <= 0 && finishedSpawning && startedTimeBetwenRounds == false)
        {
            startedTimeBetwenRounds = true;
            startTimeBetweenRounds = Time.time;
            SpawnRelic();
        }

        if (startedTimeBetwenRounds && Time.time - startTimeBetweenRounds > timeBetweenRounds)
        {
            startedTimeBetwenRounds = false;
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

    void SpawnGhoul(Vector3 spawnPosition)
    {
        EnemiesAlive.Add(Instantiate(EnemyGhoul, spawnPosition, Quaternion.identity, EnemyFolder.transform));
    }

    void SpawnRange(Vector3 spawnPosition)
    {
        EnemiesAlive.Add(Instantiate(EnemyRange, spawnPosition, Quaternion.identity, EnemyFolder.transform));
    }

    void SpawnMiniBoss(Vector3 spawnPosition)
    {
        EnemiesAlive.Add(Instantiate(EnemyMiniBoss, spawnPosition, Quaternion.identity, EnemyFolder.transform));
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
            SpawnGhoul(randomSpawn[Random.Range(0, randomSpawn.Length)].transform.position + new Vector3(Random.Range(-spawnOffset, spawnOffset), 0f, Random.Range(-spawnOffset, spawnOffset)));
        }

        //Number of ranged
        int rangedNum = (int)Mathf.Floor(((float)round - 1f) * 0.5f);
        for (int i = 0; i < rangedNum; ++i)
        {
            SpawnRange(randomSpawn[Random.Range(0, randomSpawn.Length)].transform.position + new Vector3(Random.Range(-spawnOffset, spawnOffset), 0f, Random.Range(-spawnOffset, spawnOffset)));
        }

        //Number of mutant
        int mutantNum = (int)Mathf.Floor((float)round * 0.20f);
        for (int i = 0; i < mutantNum; ++i)
        {
            SpawnMiniBoss(randomSpawn[Random.Range(0, randomSpawn.Length)].transform.position + new Vector3(Random.Range(-spawnOffset, spawnOffset), 0f, Random.Range(-spawnOffset, spawnOffset)));
        }
        yield return new WaitForSeconds(0.75f);

        randomSpawn[0].SetActive(false);
        randomSpawn[1].SetActive(false);

        finishedSpawning = true;
        StopCoroutine("Round");
    }
}
