using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    public TowerData data;
    public TowerVisuals visuals;

    protected Enemy GetTarget()
    {
        Enemy best = null;
        float bestProgress = float.MinValue;
        float rangeSqr = data.range * data.range;

        foreach (Enemy e in Enemy.Active)
        {
            if (e == null || e.IsDead) continue;
            if ((e.AimPosition - transform.position).sqrMagnitude > rangeSqr) continue;

            if (e.PathProgress > bestProgress)
            {
                best = e;
                bestProgress = e.PathProgress;
            }
        }
        return best;
    }

    protected float Aim(Transform part, Vector3 dir, float offset, float turnSpeed)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + offset;
        Quaternion targetRot = Quaternion.Euler(0f, 0f, angle);
        part.rotation = Quaternion.RotateTowards(part.rotation, targetRot, turnSpeed * Time.deltaTime);
        return Quaternion.Angle(part.rotation, targetRot);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (data == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.range);
    }
}