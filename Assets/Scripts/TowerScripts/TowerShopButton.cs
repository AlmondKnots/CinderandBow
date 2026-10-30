using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerShopButton : MonoBehaviour
{
    public int towerIndex;          // 0 = X-Bow, 1 = Aether Lance, 2 = Cinder Obelisk (matches the Tower Prefabs list)
    public Button button;
    public TMP_Text costText;       // optional

    private int cost;

    void Start()
    {
        cost = TowerPlacer.Instance.towerPrefabs[towerIndex].data.cost;
        if (costText != null) costText.text = cost.ToString();

        button.onClick.AddListener(() => TowerPlacer.Instance.SelectTower(towerIndex));

        GoldManager.Instance.OnGoldChanged += Refresh;
        Refresh(GoldManager.Instance.Gold);
    }

    void OnDestroy()
    {
        if (GoldManager.Instance != null)
            GoldManager.Instance.OnGoldChanged -= Refresh;
    }

    void Refresh(int gold)
    {
        button.interactable = gold >= cost;   // greys out when you can't afford it
    }
}