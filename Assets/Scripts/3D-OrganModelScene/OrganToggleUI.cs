using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class OrganToggleUI : MonoBehaviour
{
    public static OrganToggleUI Instance;

    [Header("UI Elements")]
    public Button toggleButton;
    public Button refreshButton;

    

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (toggleButton == null)
        {
            Debug.LogError("Toggle Button not assigned.");
            return;
        }

        toggleButton.onClick.AddListener(OnToggleClicked);
        // Hide the toggle button by default when scene starts
        HideToggleButton();

        // Setup refresh button
        if (refreshButton != null)
        {
            refreshButton.onClick.AddListener(ClearAllModels);
        }
    }

    private void OnToggleClicked()
    {
        GameObject organ = GameObject.FindGameObjectWithTag("BASIC");
        if (organ == null)
            organ = GameObject.FindGameObjectWithTag("DETAILED");

        if (organ == null)
        {
            Debug.LogWarning("No organ found with tag BASIC or DETAILED.");
            return;
        }

        OrganTarget organTarget = organ.GetComponentInParent<OrganTarget>();
        if (organTarget != null)
        {
            organTarget.ToggleOrgan();
        }
        else
        {
            Debug.LogWarning("OrganTarget component not found on organ's parent.");
        }
    }

    public void ShowToggleButton()
    {
        toggleButton.gameObject.SetActive(true);
    }

    public void HideToggleButton()
    {
        toggleButton.gameObject.SetActive(false);
    }

    public void ClearAllModels()
    {
        string[] targetTags = { "BASIC", "DETAILED" };

        foreach (string tag in targetTags)
        {
            GameObject[] models = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject model in models)
            {
                Destroy(model);
            }
        }

        if (OrganToggleUI.Instance != null)
        {
            OrganToggleUI.Instance.HideToggleButton();
        }

        Debug.Log("All BASIC and DETAILED models cleared!");
    }

}
