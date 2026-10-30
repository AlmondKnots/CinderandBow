using System.Collections.Generic;
using UnityEngine;

public class FlameProjectile : MonoBehaviour
{
    public float speed = 6f;
    public float hitRadius = 0.4f;
    public bool rotateToDirection = true;
    public float rotationOffset = 0f;

    private Vector3 direction;
    private int damage;
    private Color? damageColor;
    private float maxDistance;
    private float traveled;
    private readonly HashSet<Enemy> alreadyHit = new HashSet<Enemy>();

    private static readonly List<Enemy> buffer = new List<Enemy>();

    public void Init(Vector3 dir, int newDamage, float distance, Color? newDamageColor = null)
    {
        direction = dir.normalized;
        damage = newDamage;
        damageColor = newDamageColor;
        maxDistance = distance;

        if (rotateToDirection)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position += direction * step;
        traveled += step;

        buffer.Clear();
        buffer.AddRange(Enemy.Active);

        float radiusSqr = hitRadius * hitRadius;

        for (int i = 0; i < buffer.Count; i++)
        {
            Enemy e = buffer[i];
            if (e == null || e.IsDead || alreadyHit.Contains(e)) continue;

            Vector3 d = e.AimPosition - transform.position;
            d.z = 0f;
            if (d.sqrMagnitude <= radiusSqr)
            {
                alreadyHit.Add(e);
                e.TakeDamage(damage, damageColor);
            }
        }

        buffer.Clear();

        if (traveled >= maxDistance)
            Destroy(gameObject);
    }
}