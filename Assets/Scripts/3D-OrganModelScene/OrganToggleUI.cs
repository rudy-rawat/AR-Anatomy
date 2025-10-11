using UnityEngine;
using UnityEngine.UI;

public class OrganToggleUI : MonoBehaviour
{
    public static OrganToggleUI Instance;

    [Header("UI Buttons")]
    public Button toggleButton;
    public Button refreshButton;
    public Button toggleLabelsButton;
    public Button infoButton;
    public Button quizButton;

    [Header("UI Panels")]
    public GameObject infoPanel;
    public GameObject quizPanel;

    // Cache to avoid repeated FindObjectsByType calls
    private OrganLabelManager[] cachedLabelManagers;
    private bool labelsVisible = true;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Force panels to be inactive immediately
        if (infoPanel != null)
            infoPanel.SetActive(false);
        if (quizPanel != null)
            quizPanel.SetActive(false);
    }

    private void Start()
    {
        // Setup toggle button
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(OnToggleClicked);
            Debug.Log("Toggle button listener added");
        }
        else
            Debug.LogError("Toggle Button not assigned.");

        // Setup refresh button
        if (refreshButton != null)
        {
            refreshButton.onClick.AddListener(ClearAllModels);
            Debug.Log("Refresh button listener added");
        }

        // Setup toggle labels button
        if (toggleLabelsButton != null)
        {
            toggleLabelsButton.onClick.AddListener(OnToggleLabelsClicked);
            Debug.Log("Toggle Labels button listener added");
        }
        else
            Debug.LogWarning("Toggle Labels Button not assigned.");

        // Setup info button
        if (infoButton != null)
        {
            infoButton.onClick.AddListener(OnInfoClicked);
            Debug.Log("Info button listener added");
        }
        else
            Debug.LogWarning("Info Button not assigned.");

        // Setup quiz button
        if (quizButton != null)
        {
            quizButton.onClick.AddListener(OnQuizClicked);
            Debug.Log("Quiz button listener added");
        }
        else
            Debug.LogWarning("Quiz Button not assigned.");

        // Hide buttons by default
        HideToggleButton();
        HideExtraButtons();

        // Hide panels by default
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
            Debug.Log("Info panel set to inactive");
        }
        else
            Debug.LogWarning("Info Panel not assigned.");

        if (quizPanel != null)
        {
            quizPanel.SetActive(false);
            Debug.Log("Quiz panel set to inactive");
        }
        else
            Debug.LogWarning("Quiz Panel not assigned.");
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

    private void OnToggleLabelsClicked()
    {
        // Find all label managers and toggle them
        OrganLabelManager[] labelManagers = UnityEngine.Object.FindObjectsByType<OrganLabelManager>(FindObjectsSortMode.None);
        foreach (OrganLabelManager manager in labelManagers)
        {
            manager.ToggleLabels(!manager.showLabels);
        }
    }

    private void OnInfoClicked()
    {
        if (infoPanel != null)
        {
            bool newState = !infoPanel.activeSelf;
            infoPanel.SetActive(newState);

            // Load info for current organ
            if (newState)
            {
                OrganInfoUI infoUI = infoPanel.GetComponent<OrganInfoUI>();
                if (infoUI != null)
                {
                    string organType = GetCurrentOrganType();
                    infoUI.ShowOrganInfo(organType);
                }
            }
        }
    }

    private void OnQuizClicked()
    {
        Debug.Log("=== QUIZ BUTTON CLICKED ===");

        if (quizPanel == null)
        {
            Debug.LogError("Quiz Panel not assigned in OrganToggleUI!");
            return;
        }

        Debug.Log($"Quiz Panel current state: {quizPanel.activeSelf}");

        bool newState = !quizPanel.activeSelf;
        Debug.Log($"Setting Quiz Panel to: {newState}");

        quizPanel.SetActive(newState);

        Debug.Log($"Quiz Panel state after set: {quizPanel.activeSelf}");

        // Start quiz for current organ
        if (newState)
        {
            QuizUI quizUI = quizPanel.GetComponent<QuizUI>();
            if (quizUI != null)
            {
                string organType = GetCurrentOrganType();
                Debug.Log($"Starting quiz for organ: {organType}");
                quizUI.StartQuiz(organType);
            }
            else
            {
                Debug.LogError("QuizUI component not found on Quiz Panel!");
            }
        }

        Debug.Log("=== QUIZ BUTTON CLICK END ===");
    }

    private string GetCurrentOrganType()
    {
        GameObject organ = GameObject.FindGameObjectWithTag("BASIC");
        if (organ == null)
            organ = GameObject.FindGameObjectWithTag("DETAILED");

        if (organ != null)
        {
            OrganTarget organTarget = organ.GetComponentInParent<OrganTarget>();
            if (organTarget != null)
            {
                Debug.Log($"Found organ type: {organTarget.organType}");
                return organTarget.organType;
            }
            else
            {
                Debug.LogWarning("OrganTarget component not found!");
            }
        }
        else
        {
            Debug.LogWarning("No organ with BASIC or DETAILED tag found!");
        }

        return "";
    }

    public void ShowToggleButton()
    {
        if (toggleButton != null)
            toggleButton.gameObject.SetActive(true);
    }

    public void HideToggleButton()
    {
        if (toggleButton != null)
            toggleButton.gameObject.SetActive(false);
    }

    public void ShowExtraButtons()
    {
        if (toggleLabelsButton != null)
            toggleLabelsButton.gameObject.SetActive(true);
        if (infoButton != null)
            infoButton.gameObject.SetActive(true);
        if (quizButton != null)
            quizButton.gameObject.SetActive(true);
    }

    public void HideExtraButtons()
    {
        if (toggleLabelsButton != null)
            toggleLabelsButton.gameObject.SetActive(false);
        if (infoButton != null)
            infoButton.gameObject.SetActive(false);
        if (quizButton != null)
            quizButton.gameObject.SetActive(false);
    }

    public void ClearAllModels()
    {
        // Clear organ models
        string[] targetTags = { "BASIC", "DETAILED" };

        foreach (string tag in targetTags)
        {
            GameObject[] models = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject model in models)
            {
                Destroy(model);
            }
        }

        // Clear spawned labels from all label managers
        OrganLabelManager[] labelManagers = UnityEngine.Object.FindObjectsByType<OrganLabelManager>(FindObjectsSortMode.None);
        foreach (var manager in labelManagers)
        {
            manager.ClearLabels();
        }

        // Hide all buttons
        HideToggleButton();
        HideExtraButtons();

        // Hide panels
        if (infoPanel != null)
            infoPanel.SetActive(false);
        if (quizPanel != null)
            quizPanel.SetActive(false);

        Debug.Log("All models, labels, and panels cleared!");
    }
}