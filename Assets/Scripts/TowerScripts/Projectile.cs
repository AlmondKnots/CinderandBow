using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float hitRadius = 0.2f;
    public bool rotateToDirection = true;
    public float rotationOffset = 0f;

    [Header("Retargeting")]
    public bool retarget = true;
    public float retargetRadius = 3f;

    public bool debugLogs = false;

    private Enemy target;
    private int damage;
    private Color? damageColor;

    public void Init(Enemy newTarget, int newDamage, Color? newDamageColor = null)
    {
        target = newTarget;
        damage = newDamage;
        damageColor = newDamageColor;
    }

    void Update()
    {
        if (target == null || target.IsDead)
        {
            target = retarget ? FindNewTarget() : null;

            if (target == null)
            {
                if (debugLogs) Debug.Log("Projectile lost its target and found no other enemy.");
                Destroy(gameObject);
                return;
            }
        }

        Vector3 toTarget = target.AimPosition - transform.position;
        toTarget.z = 0f;
        float step = speed * Time.deltaTime;

        if (toTarget.magnitude <= Mathf.Max(step, hitRadius))
        {
            Hit();
            return;
        }

        transform.position += toTarget.normalized * step;

        if (rotateToDirection)
        {
            float angle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg + rotationOffset;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    Enemy FindNewTarget()
    {
        Enemy best = null;
        float bestDist = retargetRadius * retargetRadius;

        foreach (Enemy e in Enemy.Active)
        {
            if (e == null || e.IsDead) continue;

            float d = (e.AimPosition - transform.position).sqrMagnitude;
            if (d < bestDist)
            {
                best = e;
                bestDist = d;
            }
        }
        return best;
    }

    void Hit()
    {
        target.TakeDamage(damage, damageColor);

        if (debugLogs)
            Debug.Log($"Projectile hit {target.name} for {damage}. Health left: {target.CurrentHealth}");

        Destroy(gameObject);
    }
}