using System.Collections;
using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    public static GoldUI Instance { get; private set; }

    public TMP_Text goldText;

    [Header("Gain popup")]
    public Color gainColor = new Color(1f, 0.85f, 0.2f, 1f);
    public float flyTime = 0.8f;
    public float countSpeed = 6f;

    private Canvas canvas;
    private RectTransform canvasRect;
    private Camera cam;
    private Vector3 baseScale;
    private float shown;
    private int pending;
    private float bump;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        canvas = goldText.canvas.rootCanvas;
        canvasRect = canvas.transform as RectTransform;
        cam = Camera.main;
        baseScale = goldText.transform.localScale;

        shown = GoldManager.Instance.Gold;
        goldText.text = $"{Mathf.RoundToInt(shown)}";
    }

    void Update()
    {
        if (GoldManager.Instance == null) return;

        float target = GoldManager.Instance.Gold - pending;
        float step = Mathf.Max(30f, Mathf.Abs(target - shown) * countSpeed) * Time.unscaledDeltaTime;
        shown = Mathf.MoveTowards(shown, target, step);
        goldText.text = $"{Mathf.RoundToInt(shown)}";

        bump = Mathf.MoveTowards(bump, 0f, 2f * Time.unscaledDeltaTime);
        goldText.transform.localScale = baseScale * (1f + bump);
    }

    public void ShowGain(Vector3 worldPos, int amount)
    {
        if (amount <= 0 || !isActiveAndEnabled) return;

        pending += amount;
        StartCoroutine(Fly(worldPos, amount));
    }

    IEnumerator Fly(Vector3 worldPos, int amount)
    {
        if (cam == null) cam = Camera.main;

        Vector2 size = goldText.rectTransform.rect.size;
        TMP_Text popup = Instantiate(goldText, canvasRect);
        popup.text = "+" + amount;
        popup.color = gainColor;
        popup.alignment = TextAlignmentOptions.Center;
        popup.raycastTarget = false;

        RectTransform rt = popup.rectTransform;
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.localScale = baseScale;

        Vector2 screen = cam.WorldToScreenPoint(worldPos);
        Camera uiCam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screen, uiCam, out Vector2 start);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / Mathf.Max(0.05f, flyTime);
            Vector3 end = canvasRect.InverseTransformPoint(goldText.transform.position);
            rt.localPosition = Vector3.Lerp(start, end, t * t);
            yield return null;
        }

        Destroy(popup.gameObject);
        pending -= amount;
        bump = 0.25f;
    }
}