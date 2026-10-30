using UnityEngine;

[CreateAssetMenu(fileName = "NewTowerData", menuName = "TD/Tower Data")]
public class TowerData : ScriptableObject
{
    public string towerName;
    public int cost = 150;
    public float range = 4f;
    public int damage = 10;
    public float reloadTime = 1f;

    [Header("Damage number color")]
    public Color damageColor = Color.yellow;

    [Header("Projectile towers (X-Bow, Aether Lance)")]
    public Projectile projectilePrefab;

    [Header("Flame tower (Cinder Obelisk)")]
    public float tickInterval = 0.1f;
    public float coneAngle = 60f;

    [Header("Audio")]
    public AudioClip fireSound;
    public AudioClip placeSound;
    [Range(0f, 1f)] public float fireVolume = 1f;
}