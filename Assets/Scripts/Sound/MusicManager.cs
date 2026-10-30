using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Tracks")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip victoryMusic;     // optional
    public AudioClip gameOverMusic;    // optional

    [Header("Settings")]
    [Range(0f, 1f)] public float volume = 0.5f;
    public float fadeTime = 1f;

    private AudioSource a, b;
    private AudioSource current;
    private Coroutine fadeRoutine;

    void Awake()
    {
        Instance = this;

        a = gameObject.AddComponent<AudioSource>();
        b = gameObject.AddComponent<AudioSource>();
        foreach (var s in new[] { a, b })
        {
            s.playOnAwake = false;
            s.volume = 0f;
        }
        current = a;
    }

    public void PlayMenu()
    {
        // No menu track assigned: stop the game music instead of letting it keep playing
        if (menuMusic == null) StopMusic();
        else Play(menuMusic, true);
    }

    public void PlayGame() { Play(gameMusic, true); }
    public void PlayVictory() { Play(victoryMusic, false); }
    public void PlayGameOver() { Play(gameOverMusic, false); }

    // Instantly stops all music
    public void StopMusic()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = null;

        a.Stop();
        b.Stop();
        a.volume = 0f;
        b.volume = 0f;
    }

    // Hook this to the music slider (done for you by AudioSettingsUI)
    public void SetVolume(float v)
    {
        volume = Mathf.Clamp01(v);

        // If a crossfade is running it reads "volume" every frame, so only
        // set it directly when nothing is fading
        if (fadeRoutine == null && current != null)
            current.volume = volume;
    }

    void Play(AudioClip clip, bool loop)
    {
        if (clip == null) return;
        if (current.clip == clip && current.isPlaying) return;

        AudioSource next = (current == a) ? b : a;
        next.clip = clip;
        next.loop = loop;
        next.volume = 0f;
        next.Play();

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(Crossfade(current, next));
        current = next;
    }

    IEnumerator Crossfade(AudioSource from, AudioSource to)
    {
        float t = 0f;
        float startVol = from.volume;

        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            float k = fadeTime > 0f ? t / fadeTime : 1f;
            from.volume = Mathf.Lerp(startVol, 0f, k);
            to.volume = Mathf.Lerp(0f, volume, k);
            yield return null;
        }

        from.Stop();
        from.volume = 0f;
        to.volume = volume;
        fadeRoutine = null;
    }
}