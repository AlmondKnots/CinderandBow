using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class TowerPlacer : MonoBehaviour
{
    public static TowerPlacer Instance { get; private set; }

    [Header("Towers (order = shop button index)")]
    public Tower[] towerPrefabs;     

    [Header("Placement rules")]
    public float pathHalfWidth = 0.6f;
    public float towerRadius = 0.4f;
    public float towerSpacing = 0.9f;
    public LayerMask blockedLayers;        
    public bool keepPlacing = false;

    [Header("Preview")]
    public Color validColor = new Color(0.4f, 1f, 0.4f, 0.65f);
    public Color invalidColor = new Color(1f, 0.3f, 0.3f, 0.65f);

    [Header("Cancel hint text")]
    public bool showCancelHint = true;
    public string cancelHintMessage = "Press \"X\" to cancel";
    public float hintFontSize = 3f;       
    public float hintHeightOffset = 0.3f;  
    public Color hintColor = Color.white;
    public int hintSortingOrder = 100;

    [Header("Range circle")]
    public bool showRange = true;
    public Color validRangeColor = new Color(0.4f, 1f, 0.4f, 1f);
    public Color invalidRangeColor = new Color(1f, 0.3f, 0.3f, 1f);
    public int rangeSortingOrder = 0;      

    private Tower selected;
    private GameObject ghost;
    private SpriteRenderer[] ghostRenderers;
    private SpriteRenderer rangeRenderer;
    private readonly List<Vector3> placed = new List<Vector3>();
    private Camera cam;
    private static Sprite circleSprite;

    void Awake()
    {
        Instance = this;
        cam = Camera.main;
    }

    Vector2 MousePosition()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
#else
        return Input.mousePosition;
#endif
    }

    bool LeftClicked()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

    bool CancelPressed()
    {
#if ENABLE_INPUT_SYSTEM
        return (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame) ||
               (Keyboard.current != null && Keyboard.current.xKey.wasPressedThisFrame);
#else
        return Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.X);
#endif
    }

    public void SelectTower(int index)
    {
        if (index < 0 || index >= towerPrefabs.Length) return;

        Cancel();
        selected = towerPrefabs[index];

        ghost = Instantiate(selected.gameObject);
        foreach (MonoBehaviour mb in ghost.GetComponentsInChildren<MonoBehaviour>())
            mb.enabled = false;   
        ghostRenderers = ghost.GetComponentsInChildren<SpriteRenderer>();

        if (showRange) CreateRangeCircle();
        if (showCancelHint) CreateCancelHint();
    }

    public void Cancel()
    {
        selected = null;
        if (ghost != null) Destroy(ghost);   
        ghost = null;
        rangeRenderer = null;
    }

    void CreateRangeCircle()
    {
        GameObject go = new GameObject("RangeCircle");
        go.transform.SetParent(ghost.transform, false);
        go.transform.localPosition = Vector3.zero;

        float diameter = selected.data.range * 2f;
        Vector3 parentScale = ghost.transform.lossyScale;
        go.transform.localScale = new Vector3(
            diameter / Mathf.Max(parentScale.x, 0.0001f),
            diameter / Mathf.Max(parentScale.y, 0.0001f), 1f);

        rangeRenderer = go.AddComponent<SpriteRenderer>();
        rangeRenderer.sprite = GetCircleSprite();

        if (ghostRenderers != null && ghostRenderers.Length > 0)
            rangeRenderer.sortingLayerID = ghostRenderers[0].sortingLayerID;
        rangeRenderer.sortingOrder = rangeSortingOrder;
    }

    void CreateCancelHint()
    {
        GameObject go = new GameObject("CancelHint");
        go.transform.SetParent(ghost.transform, false);

        Vector3 parentScale = ghost.transform.lossyScale;
        float sx = 1f / Mathf.Max(parentScale.x, 0.0001f);
        float sy = 1f / Mathf.Max(parentScale.y, 0.0001f);
        go.transform.localScale = new Vector3(sx, sy, 1f);

        float topOffset = 0.5f;
        if (ghostRenderers != null && ghostRenderers.Length > 0)
        {
            Bounds b = ghostRenderers[0].bounds;
            for (int i = 1; i < ghostRenderers.Length; i++) b.Encapsulate(ghostRenderers[i].bounds);
            topOffset = b.max.y - ghost.transform.position.y;
        }
        go.transform.localPosition = new Vector3(0f, (topOffset + hintHeightOffset) * sy, 0f);

        TextMeshPro tmp = go.AddComponent<TextMeshPro>();
        tmp.text = cancelHintMessage;
        tmp.fontSize = hintFontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = hintColor;
        tmp.fontStyle = FontStyles.Bold;

        if (ghostRenderers != null && ghostRenderers.Length > 0)
            tmp.sortingLayerID = ghostRenderers[0].sortingLayerID;
        tmp.sortingOrder = hintSortingOrder;
    }

    static Sprite GetCircleSprite()
    {
        if (circleSprite != null) return circleSprite;

        int size = 256;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        float r = size * 0.5f - 1f;
        Vector2 c = new Vector2(size * 0.5f, size * 0.5f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), c);
                float a = 0f;
                if (d <= r) a = 0.18f;                 
                if (d <= r && d >= r - 4f) a = 0.85f;  
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        }
        tex.Apply();

        circleSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        return circleSprite;
    }

    void Update()
    {
        if (selected == null) return;

        if (GameManager.Instance != null &&
            (GameManager.Instance.IsFinished || !GameManager.Instance.IsPlaying))
        {
            Cancel();
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.IsPaused)
            return;

        if (CancelPressed())
        {
            Cancel();
            return;
        }

        Vector3 pos = cam.ScreenToWorldPoint(MousePosition());
        pos.z = 0f;

        ghost.transform.position = pos;

        bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        bool canAfford = GoldManager.Instance.CanAfford(selected.data.cost);
        bool valid = !overUI && canAfford && IsValidSpot(pos);

        foreach (SpriteRenderer sr in ghostRenderers)
            sr.color = valid ? validColor : invalidColor;

        if (rangeRenderer != null)
            rangeRenderer.color = valid ? validRangeColor : invalidRangeColor;

        if (valid && LeftClicked())
            Place(pos);
    }

    void Place(Vector3 pos)
    {
        if (!GoldManager.Instance.TrySpend(selected.data.cost)) return;

        SoundManager.Play(selected.data.placeSound);

        Instantiate(selected.gameObject, pos, Quaternion.identity);
        placed.Add(pos);

        if (!keepPlacing) Cancel();
    }

    bool IsValidSpot(Vector3 pos)
    {
        if (DistanceToPath(pos) < pathHalfWidth + towerRadius) return false;

        if (Physics2D.OverlapCircle(pos, towerRadius, blockedLayers) != null) return false;

        foreach (Vector3 p in placed)
            if (Vector3.Distance(p, pos) < towerSpacing) return false;

        return true;
    }

    float DistanceToPath(Vector3 pos)
    {
        var path = WaypointPath.Instance.waypoints;
        float best = float.MaxValue;

        if (WaveManager.Instance != null && WaveManager.Instance.spawnPoint != null && path.Length > 0)
            best = Mathf.Min(best, DistToSegment(pos,
                WaveManager.Instance.spawnPoint.position, path[0].transform.position));

        for (int i = 0; i < path.Length - 1; i++)
            best = Mathf.Min(best, DistToSegment(pos,
                path[i].transform.position, path[i + 1].transform.position));

        return best;
    }

    float DistToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float len = ab.sqrMagnitude;
        float t = len > 0f ? Mathf.Clamp01(Vector2.Dot(p - a, ab) / len) : 0f;
        return Vector2.Distance(p, a + ab * t);
    }
}