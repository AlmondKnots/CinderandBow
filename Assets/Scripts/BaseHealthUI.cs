using UnityEngine;
using UnityEngine.UI;

public class BaseHealthUI : MonoBehaviour
{
    [Header("References")]
    public Image healthFilled;      

    [Header("Animation")]
    public bool smoothFill = true;
    public float fillSpeed = 2f;

    private float targetFill = 1f;

    void Start()
    {
        BaseHealth.Instance.OnHealthChanged += UpdateBar;
        UpdateBar(BaseHealth.Instance.CurrentHealth, BaseHealth.Instance.maxHealth);
        healthFilled.fillAmount = targetFill;   
    }

    void OnDestroy()
    {
        if (BaseHealth.Instance != null)
            BaseHealth.Instance.OnHealthChanged -= UpdateBar;
    }

    void Update()
    {
        if (!smoothFill) return;

        healthFilled.fillAmount = Mathf.MoveTowards(
            healthFilled.fillAmount, targetFill, fillSpeed * Time.unscaledDeltaTime);
    }

    void UpdateBar(int current, int max)
    {
        targetFill = (float)current / max;

        if (!smoothFill)
            healthFilled.fillAmount = targetFill;
    }
}