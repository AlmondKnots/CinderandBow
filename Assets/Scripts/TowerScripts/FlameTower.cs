using UnityEngine;

public class FlameTower : Tower
{
    [Header("Flames")]
    public FlameProjectile flamePrefab;
    public Transform firePoint;

    [Header("Aiming (optional)")]
    public Transform aimPivot;
    public float rotationOffset = 0f;
    public float turnSpeed = 720f;

    private float tickTimer;
    private Vector3 aimDir = Vector3.right;
    private AudioSource loopSource;

    void Start()
    {
        if (data.fireSound != null)
        {
            loopSource = gameObject.AddComponent<AudioSource>();
            loopSource.clip = data.fireSound;
            loopSource.loop = true;
            loopSource.playOnAwake = false;
            loopSource.volume = data.fireVolume;
        }
    }

    void Update()
    {
        Enemy primary = GetTarget();
        bool firing = primary != null;

        if (visuals != null) visuals.SetFiring(firing);
        SetLoopSound(firing);

        if (!firing)
        {
            tickTimer = 0f;
            return;
        }

        Vector3 origin = firePoint != null ? firePoint.position : transform.position;
        aimDir = (primary.AimPosition - origin).normalized;

        if (aimPivot != null)
            Aim(aimPivot, aimDir, rotationOffset, turnSpeed);

        tickTimer -= Time.deltaTime;
        if (tickTimer <= 0f)
        {
            tickTimer += data.tickInterval;
            SpawnFlame(origin);
        }
    }

    void SetLoopSound(bool on)
    {
        if (loopSource == null) return;

        if (on && !loopSource.isPlaying) loopSource.Play();
        else if (!on && loopSource.isPlaying) loopSource.Stop();
    }

    void SpawnFlame(Vector3 origin)
    {
        if (flamePrefab == null) return;

        float spread = Random.Range(-data.coneAngle * 0.5f, data.coneAngle * 0.5f);
        Vector3 dir = Quaternion.Euler(0f, 0f, spread) * aimDir;

        FlameProjectile f = Instantiate(flamePrefab, origin, Quaternion.identity);
        f.Init(dir, data.damage, data.range, data.damageColor);
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        if (data == null) return;

        Gizmos.color = Color.red;
        float half = data.coneAngle * 0.5f;
        Vector3 right = Quaternion.Euler(0, 0, half) * aimDir * data.range;
        Vector3 left = Quaternion.Euler(0, 0, -half) * aimDir * data.range;
        Gizmos.DrawLine(transform.position, transform.position + right);
        Gizmos.DrawLine(transform.position, transform.position + left);
    }
}