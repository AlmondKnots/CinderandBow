using UnityEngine;
using UnityEngine.UI;

public class ButtonSounds : MonoBehaviour
{
    public AudioClip clickSound;
    [Range(0f, 1f)] public float volume = 1f;

    void Awake()
    {
        foreach (Button b in GetComponentsInChildren<Button>(true))
            b.onClick.AddListener(PlayClick);
    }

    void PlayClick()
    {
        SoundManager.Play(clickSound, volume);
    }
}