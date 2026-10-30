using UnityEngine;

public class ProjectileTower : Tower
{
    [Header("Rotation (X-Bow only, leave empty for Lance)")]
    public Transform rotatePart;
    public float rotationOffset = 0f;
    public float turnSpeed = 720f;
    public float fireAngleTolerance = 5f;

    [Header("Firing")]
    public Transform firePoint;

    private float cooldown;

    void Update()
    {
        cooldown = Mathf.Max(cooldown - Time.deltaTime, 0f);

        Enemy target = GetTarget();
        if (target == null) return;

        if (rotatePart != null)
        {
            Vector3 dir = target.AimPosition - rotatePart.position;
            float remaining = Aim(rotatePart, dir, rotationOffset, turnSpeed);
            if (remaining > fireAngleTolerance) return;
        }

        if (cooldown > 0f) return;

        Fire(target);
        cooldown = data.reloadTime;
    }

    void Fire(Enemy target)
    {
        Vector3 pos = firePoint != null ? firePoint.position : transform.position;
        Projectile p = Instantiate(data.projectilePrefab, pos, Quaternion.identity);
        p.Init(target, data.damage, data.damageColor);

        SoundManager.Play(data.fireSound, data.fireVolume);

        if (visuals != null) visuals.PlayShoot();
    }
}