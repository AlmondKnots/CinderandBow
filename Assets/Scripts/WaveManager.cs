using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class SpawnGroup
{
    public GameObject enemyPrefab;   
    public int count = 5;
    public float spawnInterval = 1f;
    public float delayAfterGroup = 2f;
}

[Serializable]
public class Wave
{
    public string waveName;
    public SpawnGroup[] groups;
}

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("Setup")]
    public Transform spawnPoint;
    public Wave[] waves;

    [Header("Timing")]
    public float firstWaveDelay = 5f;
    public float timeBetweenWaves = 10f;
    [Tooltip("ON: countdown starts after all enemies are dead. OFF: starts once the wave finishes spawning.")]
    public bool waitForClear = false;

    public int CurrentWave { get; private set; }     
    public int TotalWaves => waves.Length;
    public int EnemiesAlive { get; private set; }

    public event Action<int, int> OnWaveStarted;     
    public event Action<float> OnCountdown;          
    public event Action<int> OnEnemiesAliveChanged;
    public event Action OnAllWavesCompleted;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        yield return Countdown(firstWaveDelay);

        for (int i = 0; i < waves.Length; i++)
        {
            CurrentWave = i + 1;
            OnWaveStarted?.Invoke(CurrentWave, TotalWaves);

            yield return SpawnWave(waves[i]);

            bool isLast = (i == waves.Length - 1);

            if (waitForClear || isLast)
                yield return new WaitUntil(() => EnemiesAlive <= 0);

            if (isLast) break;

            yield return Countdown(timeBetweenWaves);
        }

        OnAllWavesCompleted?.Invoke();
    }

    IEnumerator SpawnWave(Wave wave)
    {
        foreach (SpawnGroup group in wave.groups)
        {
            for (int i = 0; i < group.count; i++)
            {
                Instantiate(group.enemyPrefab, spawnPoint.position, Quaternion.identity);
                EnemiesAlive++;
                OnEnemiesAliveChanged?.Invoke(EnemiesAlive);
                yield return new WaitForSeconds(group.spawnInterval);
            }
            yield return new WaitForSeconds(group.delayAfterGroup);
        }
    }

    IEnumerator Countdown(float seconds)
    {
        float t = seconds;
        while (t > 0f)
        {
            OnCountdown?.Invoke(t);
            t -= Time.deltaTime;
            yield return null;
        }
        OnCountdown?.Invoke(0f);
    }

    public void EnemyRemoved()
    {
        EnemiesAlive = Mathf.Max(EnemiesAlive - 1, 0);
        OnEnemiesAliveChanged?.Invoke(EnemiesAlive);
    }
}