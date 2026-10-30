using System.Collections;
using UnityEngine;

public class TowerVisuals : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite idleSprite;
    public Sprite[] shootSprites;
    public float frameTime = 0.08f;

    private Coroutine routine;
    private bool looping;

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (idleSprite == null)
            idleSprite = spriteRenderer.sprite;
    }

    // One shot: used by the Aether Lance
    public void PlayShoot()
    {
        if (shootSprites == null || shootSprites.Length == 0) return;
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(ShootRoutine());
    }

    // Loops while true: used by the Cinder Obelisk
    public void SetFiring(bool firing)
    {
        if (firing == looping) return;
        looping = firing;

        if (routine != null) StopCoroutine(routine);

        if (looping && shootSprites != null && shootSprites.Length > 0)
            routine = StartCoroutine(LoopRoutine());
        else
        {
            routine = null;
            spriteRenderer.sprite = idleSprite;
        }
    }

    IEnumerator ShootRoutine()
    {
        foreach (Sprite s in shootSprites)
        {
            spriteRenderer.sprite = s;
            yield return new WaitForSeconds(frameTime);
        }
        spriteRenderer.sprite = idleSprite;
        routine = null;
    }

    IEnumerator LoopRoutine()
    {
        int i = 0;
        while (true)
        {
            spriteRenderer.sprite = shootSprites[i];
            i = (i + 1) % shootSprites.Length;
            yield return new WaitForSeconds(frameTime);
        }
    }
}