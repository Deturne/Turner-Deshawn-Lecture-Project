using UnityEngine;
using UnityEngine.UI;

public class RadialHealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider; // Ensure this is serialized
    [SerializeField] private Image fillImage;
    [SerializeField] public Slider xpSlider; // Ensure this is serialized
    [SerializeField] public Image xpfillImage;
    public bool xpBar = false;

    void Awake()
    {
        // Dynamically find the Fill Image in the instantiated prefab
        if (healthSlider != null && healthSlider.fillRect != null)
        {
            fillImage = healthSlider.fillRect.GetComponent<Image>();
            if (fillImage == null)
            {
                Debug.LogError("Fill Image is not assigned or missing in the Slider component!");
            }
        }

        // Dynamically find the Fill Image in the instantiated prefab
        if (xpSlider != null && xpSlider.fillRect != null)
        {
            xpfillImage = xpSlider.fillRect.GetComponent<Image>();
            if (xpfillImage == null)
            {
                Debug.LogError("XP Fill Image is not assigned or missing in the Slider component!");
            }
        }
    }

    void Start()
    {
        if (xpSlider != null)
        {
            xpSlider.value = 0;
            xpSlider.maxValue = Team.nextRankThreshold; // Set the max value for the XP slider
            Debug.Log($"XP Slider Max Value: {xpSlider.maxValue}");
        }
    }

    public void UpdateHealthBar(float percentage)
    {
        if (healthSlider != null)
        {
            healthSlider.value = percentage;
            Debug.Log($"HealthSlider updated: {healthSlider.value}");
        }

        if (fillImage != null)
        {
            fillImage.fillAmount = percentage;
            Debug.Log($"FillImage updated: {fillImage.fillAmount}");
        }
    }

    public void IncreaseXP(float xpAmount)
    {
        if (xpSlider != null)
        {
            xpSlider.value += xpAmount; // Increment the slider value
            Debug.Log($"XP Slider updated: {xpSlider.value}/{xpSlider.maxValue}");
        }

        if (xpfillImage != null)
        {
            xpfillImage.fillAmount = xpSlider.value / xpSlider.maxValue; // Update the fill amount
            Debug.Log($"XP Fill Image updated: {xpfillImage.fillAmount}");
        }

        // Check if the XP exceeds the threshold for the next rank
        if (xpSlider != null && xpSlider.value >= xpSlider.maxValue)
        {
            Debug.Log("Rank up! Resetting XP bar.");
            xpSlider.value = 0; // Reset the slider value
            xpfillImage.fillAmount = 0; // Reset the fill amount
            xpSlider.maxValue = Team.nextRankThreshold; // Update the max value for the next rank
        }
    }

    public void SetMaxHealth(float maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
        }
    }

    public void SetHealth(float health)
    {
        if (healthSlider != null)
        {
            healthSlider.value = health;
            UpdateHealthBar(health / healthSlider.maxValue);
        }
    }
}
