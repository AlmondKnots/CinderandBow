using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static readonly List<Enemy> Active = new List<Enemy>();

    public EnemyData data;
    public float rotateSpeed = 540f;
    public bool smoothRotation = true;

    public Transform aimPoint;
    public Vector3 AimPosition => aimPoint != null ? aimPoint.position : transform.position;

    public int CurrentHealth { get; private set; }
    public bool IsDead => removed;

    public float PathProgress
    {
        get
        {
            if (path == null || index >= path.Length) return index * 1000f;
            return index * 1000f - Vector3.Distance(transform.position, path[index].transform.position);
        }
    }

    private Waypoint[] path;
    private int index = 0;
    private Quaternion targetRotation;
    private bool removed;
    private EnemyHealthBar healthBar;

    void OnEnable() { Active.Add(this); }
    void OnDisable() { Active.Remove(this); }

    void Start()
    {
        CurrentHealth = data.maxHealth;
        path = WaypointPath.Instance.waypoints;
        targetRotation = transform.rotation;

        healthBar = GetComponent<EnemyHealthBar>();
        if (healthBar == null) healthBar = gameObject.AddComponent<EnemyHealthBar>();
    }

    void Update()
    {
        if (path == null || index >= path.Length) return;

        Transform target = path[index].transform;
        transform.position = Vector3.MoveTowards(
            transform.position, target.position, data.moveSpeed * Time.deltaTime);

        if (smoothRotation)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            OnReachedWaypoint(path[index]);
        }
    }

    void OnReachedWaypoint(Waypoint wp)
    {
        if (index == path.Length - 1)
        {
            ReachBase();
            return;
        }

        if (wp.changeRotation)
        {
            targetRotation = Quaternion.Euler(0f, 0f, wp.rotationZ);
            if (!smoothRotation) transform.rotation = targetRotation;
        }

        index++;
    }

    void ReachBase()
    {
        if (BaseHealth.Instance != null)
            BaseHealth.Instance.TakeDamage(data.baseDamage);

        Remove();
    }

    public void TakeDamage(int amount, Color? numberColor = null)
    {
        if (removed) return;
        CurrentHealth -= amount;

        if (healthBar != null) healthBar.ShowDamage(amount, numberColor);

        if (CurrentHealth <= 0) Die();
    }

    void Die()
    {
        SoundManager.Play(data.deathSound, data.deathVolume);

        if (GoldUI.Instance != null)
            GoldUI.Instance.ShowGain(AimPosition, data.goldReward);

        if (GoldManager.Instance != null)
            GoldManager.Instance.Add(data.goldReward);

        Remove();
    }

    void Remove()
    {
        if (removed) return;
        removed = true;

        if (WaveManager.Instance != null)
            WaveManager.Instance.EnemyRemoved();

        Destroy(gameObject);
    }
}