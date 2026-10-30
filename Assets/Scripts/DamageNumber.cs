using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    private TextMeshPro tmp;
    private float riseSpeed;
    private float lifetime;
    private float age;
    private Color startColor;

    public static void Spawn(Vector3 position, string text, Color color, float fontSize,
                             float riseSpeed, float lifetime, int sortingLayerID, int sortingOrder)
    {
        GameObject go = new GameObject("DamageNumber");
        go.transform.position = position;

        DamageNumber dn = go.AddComponent<DamageNumber>();
        dn.tmp = go.AddComponent<TextMeshPro>();
        dn.tmp.text = text;
        dn.tmp.fontSize = fontSize;
        dn.tmp.alignment = TextAlignmentOptions.Center;
        dn.tmp.fontStyle = FontStyles.Bold;
        dn.tmp.color = color;
        dn.tmp.sortingLayerID = sortingLayerID;
        dn.tmp.sortingOrder = sortingOrder;

        dn.startColor = color;
        dn.riseSpeed = riseSpeed;
        dn.lifetime = Mathf.Max(0.05f, lifetime);
    }

    void Update()
    {
        age += Time.deltaTime;

        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        float t = Mathf.Clamp01((age / lifetime - 0.5f) * 2f);
        Color c = startColor;
        c.a = 1f - t;
        tmp.color = c;

        if (age >= lifetime) Destroy(gameObject);
    }
}