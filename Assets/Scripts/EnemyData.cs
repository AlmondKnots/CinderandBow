using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "TD/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int maxHealth = 30;
    public float moveSpeed = 2f;
    public int baseDamage = 3;
    public int goldReward = 5;

    [Header("Audio")]
    public AudioClip deathSound;
    [Range(0f, 1f)] public float deathVolume = 1f;
}