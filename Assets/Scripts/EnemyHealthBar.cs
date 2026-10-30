using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("Bar")]
    public float width = 0.8f;
    public float height = 0.1f;
    public float border = 0.03f;
    public float heightOffset = 0.6f;
    public bool hideWhenFull = true;
    public Color backgroundColor = new Color(0f, 0f, 0f, 0.75f);
    public Color fullColor = new Color(0.3f, 1f, 0.3f, 1f);
    public Color lowColor = new Color(1f, 0.25f, 0.25f, 1f);
    public int sortingOrder = 50;

    [Header("Damage numbers")]
    public bool showDamageNumbers = true;
    public Color damageColor = Color.yellow;
    public float damageFontSize = 3f;
    public float riseSpeed = 1f;
    public float damageLifetime = 0.8f;

    private Enemy enemy;
    private GameObject root;
    private Transform fill;
    private SpriteRenderer fillRenderer;
    private int sortingLayerID;

    private static Sprite barSprite;

    static Sprite GetBarSprite()
    {
        if (barSprite != null) return barSprite;

        Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, Color.white);
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        barSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0f, 0.5f), 1f);
        return barSprite;
    }

    void Start()
    {
        enemy = GetComponent<Enemy>();

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sortingLayerID = sr.sortingLayerID;

        root = new GameObject("HealthBar");

        CreatePart("Background", backgroundColor, sortingOrder,
            new Vector3(-width * 0.5f - border, 0f, 0f),
            new Vector3(width + border * 2f, height + border * 2f, 1f));

        fillRenderer = CreatePart("Fill", fullColor, sortingOrder + 1,
            new Vector3(-width * 0.5f, 0f, 0f),
            new Vector3(width, height, 1f));
        fill = fillRenderer.transform;
    }

    SpriteRenderer CreatePart(string partName, Color color, int order, Vector3 localPos, Vector3 localScale)
    {
        GameObject go = new GameObject(partName);
        go.transform.SetParent(root.transform, false);
        go.transform.localPosition = localPos;
        go.transform.localScale = localScale;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = GetBarSprite();
        sr.color = color;
        sr.sortingLayerID = sortingLayerID;
        sr.sortingOrder = order;
        return sr;
    }

    void LateUpdate()
    {
        if (root == null || enemy == null || enemy.data == null) return;

        float pct = Mathf.Clamp01(enemy.CurrentHealth / (float)enemy.data.maxHealth);

        root.SetActive(!(hideWhenFull && pct >= 1f));
        root.transform.position = transform.position + Vector3.up * heightOffset;

        fill.localScale = new Vector3(width * pct, height, 1f);
        fillRenderer.color = Color.Lerp(lowColor, fullColor, pct);
    }

    public void ShowDamage(int amount, Color? color = null)
    {
        if (!showDamageNumbers) return;

        Vector3 pos = enemy != null ? enemy.AimPosition : transform.position;
        pos += new Vector3(Random.Range(-0.2f, 0.2f), 0.2f, 0f);

        DamageNumber.Spawn(pos, amount.ToString(), color ?? damageColor, damageFontSize,
            riseSpeed, damageLifetime, sortingLayerID, sortingOrder + 10);
    }

    void OnDestroy()
    {
        if (root != null) Destroy(root);
    }
}