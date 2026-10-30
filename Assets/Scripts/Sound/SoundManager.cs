using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Range(0f, 1f)] public float masterVolume = 1f;
    public int poolSize = 12;
    [Tooltip("Random pitch change so repeated sounds feel less repetitive")]
    public float pitchVariation = 0.05f;

    private AudioSource[] sources;
    private int next;

    void Awake()
    {
        Instance = this;

        sources = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            sources[i] = gameObject.AddComponent<AudioSource>();
            sources[i].playOnAwake = false;
        }
    }

    // Hook this to the sound effects slider (done for you by AudioSettingsUI)
    public void SetVolume(float v)
    {
        masterVolume = Mathf.Clamp01(v);
    }

    public static void Play(AudioClip clip, float volume = 1f)
    {
        if (clip == null || Instance == null) return;

        AudioSource s = Instance.sources[Instance.next];
        Instance.next = (Instance.next + 1) % Instance.sources.Length;

        s.pitch = 1f + Random.Range(-Instance.pitchVariation, Instance.pitchVariation);
        s.PlayOneShot(clip, volume * Instance.masterVolume);
    }
}