using TMPro;
using UnityEngine;

public class WaveUI : MonoBehaviour
{
    [Header("Texts")]
    public TMP_Text waveText;
    public TMP_Text timerText;     
    public TMP_Text enemiesText;

    [Header("Options")]
    [Tooltip("ON: gameplay timer stops while the wave countdown is showing.")]
    public bool pauseTimerDuringCountdown = false;

    private float elapsed;
    private float countdownLeft;
    private bool counting;       
    private bool finished;       

    void Start()
    {
        var wm = WaveManager.Instance;

        wm.OnWaveStarted += (cur, total) =>
        {
            if (waveText) waveText.text = $"Wave {cur}/{total}";
        };

        wm.OnEnemiesAliveChanged += n =>
        {
            if (enemiesText) enemiesText.text = $"Enemies: {n}";
        };

        wm.OnCountdown += t =>
        {
            countdownLeft = t;
            counting = t > 0f;
        };

        wm.OnAllWavesCompleted += () => finished = true;

        if (waveText) waveText.text = $"Wave 0/{wm.TotalWaves}";
        if (enemiesText) enemiesText.text = "Enemies: 0";
    }

    void Update()
    {
        if (timerText == null) return;

        if (!finished && !(counting && pauseTimerDuringCountdown))
            elapsed += Time.deltaTime;

        if (counting)
            timerText.text = $"00:0{Mathf.CeilToInt(countdownLeft)}";
        else
            timerText.text = FormatTime(elapsed);
    }

    string FormatTime(float seconds)
    {
        int m = (int)(seconds / 60f);
        int s = (int)(seconds % 60f);
        return $"{m:00}:{s:00}";
    }

    public float ElapsedTime => elapsed;
}