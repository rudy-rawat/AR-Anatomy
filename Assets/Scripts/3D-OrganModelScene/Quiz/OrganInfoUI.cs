using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrganInfoUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI organNameText;
    public TextMeshProUGUI organInfoText;
    public Button closeButton;

    private void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);
    }

    public void ShowOrganInfo(string organType)
    {
        if (string.IsNullOrEmpty(organType))
        {
            Debug.LogWarning("No organ type provided!");
            return;
        }

        OrganVariant variant = OrganRegistry.Instance.GetOrganVariant(organType);
        if (variant != null)
        {
            if (organNameText != null)
                organNameText.text = variant.organName.ToUpper();

            if (organInfoText != null)
                organInfoText.text = variant.organInfo;
        }
        else
        {
            Debug.LogWarning($"No organ variant found for: {organType}");
        }
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}