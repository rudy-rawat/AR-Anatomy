using UnityEngine;
using Vuforia;
using System.Collections;

public class OrganTarget : MonoBehaviour
{
    [Header("Organ Info")]
    public string organType;
    public bool haveDetailVersion;

    [Header("Label System")]
    public OrganLabelManager labelManager;  // Assign in inspector

    private GameObject currentOrgan;
    private bool showingDetailed = false;
    private ObserverBehaviour observer;
    public float fadeDuration = 0.5f;

    void Awake()
    {
        observer = GetComponent<ObserverBehaviour>();
        if (observer != null)
            observer.OnTargetStatusChanged += OnTargetStatusChanged;
    }

    void OnDestroy()
    {
        if (observer != null)
            observer.OnTargetStatusChanged -= OnTargetStatusChanged;
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED)
        {
            SpawnOrgan(showingDetailed);

            if (haveDetailVersion)
            {
                if (OrganToggleUI.Instance != null)
                    OrganToggleUI.Instance.ShowToggleButton();
                else
                    Debug.LogWarning("OrganToggleUI Instance is null!");
            }
            else
            {
                if (OrganToggleUI.Instance != null)
                    OrganToggleUI.Instance.HideToggleButton();
                else
                    Debug.LogWarning("OrganToggleUI Instance is null!");
            }
        }
        else
        {
            RemoveOrganImmediate();
        }
    }

    private void SpawnOrgan(bool detailed)
    {
        // Destroy existing organ if it exists
        if (currentOrgan != null)
        {
            Destroy(currentOrgan);
            currentOrgan = null;
        }

        GameObject prefab = detailed ? OrganRegistry.Instance.GetDetailed(organType) : OrganRegistry.Instance.GetBasic(organType);

        if (prefab != null)
        {
            currentOrgan = Instantiate(prefab, transform);
            currentOrgan.tag = detailed ? "DETAILED" : "BASIC";
            StartCoroutine(FadeInModel(currentOrgan));

            // Add PinchToZoom component
            if (currentOrgan.GetComponent<PinchToZoom>() == null)
            {
                currentOrgan.AddComponent<PinchToZoom>();
            }

            // Setup labels - automatically find anchors in the spawned model
            SetupLabelsForCurrentOrgan(detailed);
        }

        showingDetailed = detailed;
    }

    private void SetupLabelsForCurrentOrgan(bool detailed)
    {
        if (labelManager == null)
        {
            Debug.LogWarning("Label manager not assigned!");
            return;
        }

        if (currentOrgan == null)
        {
            Debug.LogWarning("No current organ to setup labels for!");
            return;
        }

        // Get the appropriate label definitions from registry
        OrganVariant variant = OrganRegistry.Instance.GetOrganVariant(organType);
        if (variant != null)
        {
            LabelPoint[] labelDefinitions = detailed ? variant.detailedLabels : variant.basicLabels;

            if (labelDefinitions != null && labelDefinitions.Length > 0)
            {
                // Find anchor points in the spawned model
                LabelPoint[] labelsWithAnchors = FindAnchorsInModel(labelDefinitions, currentOrgan);
                labelManager.SetupLabels(labelsWithAnchors);
            }
        }
    }

    private LabelPoint[] FindAnchorsInModel(LabelPoint[] labelDefinitions, GameObject model)
    {
        LabelPoint[] result = new LabelPoint[labelDefinitions.Length];

        for (int i = 0; i < labelDefinitions.Length; i++)
        {
            // Create a copy of the label definition
            result[i] = new LabelPoint
            {
                labelText = labelDefinitions[i].labelText,
                labelColor = labelDefinitions[i].labelColor,
                anchorName = labelDefinitions[i].anchorName
            };

            // Find the anchor transform in the model hierarchy
            string searchName = labelDefinitions[i].GetAnchorSearchName();
            Transform foundAnchor = FindChildRecursive(model.transform, searchName);

            if (foundAnchor != null)
            {
                result[i].anchorPoint = foundAnchor;
                Debug.Log($"Found anchor '{searchName}' for label '{labelDefinitions[i].labelText}'");
            }
            else
            {
                Debug.LogWarning($"Could not find anchor GameObject named '{searchName}' for label '{labelDefinitions[i].labelText}' in model '{model.name}'");
            }
        }

        return result;
    }

    private Transform FindChildRecursive(Transform parent, string childName)
    {
        // Check direct children first
        foreach (Transform child in parent)
        {
            if (child.name.Equals(childName, System.StringComparison.OrdinalIgnoreCase))
            {
                return child;
            }
        }

        // Recursively search in children
        foreach (Transform child in parent)
        {
            Transform found = FindChildRecursive(child, childName);
            if (found != null)
                return found;
        }

        return null;
    }

    public void ToggleOrgan()
    {
        showingDetailed = !showingDetailed;
        StartCoroutine(AnimateModelSwap(showingDetailed));
    }

    private IEnumerator AnimateModelSwap(bool newDetailState)
    {
        GameObject old = currentOrgan;
        if (old != null)
        {
            yield return StartCoroutine(FadeOutModel(old));
        }

        if (old != null)
            Destroy(old);

        SpawnOrgan(newDetailState);
    }

    private IEnumerator FadeInModel(GameObject model)
    {
        Renderer[] renderers = model.GetComponentsInChildren<Renderer>(true);
        Vector3 originalScale = model.transform.localScale;
        model.transform.localScale = originalScale * 0.3f;

        foreach (var r in renderers)
            foreach (var mat in r.materials)
                mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 0f);

        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = t / fadeDuration;
            float scale = Mathf.Lerp(0.3f, 1f, alpha);

            foreach (var r in renderers)
                foreach (var mat in r.materials)
                    mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, alpha);

            model.transform.localScale = originalScale * scale;
            yield return null;
        }

        model.transform.localScale = originalScale;
    }

    private IEnumerator FadeOutModel(GameObject model)
    {
        Renderer[] renderers = model.GetComponentsInChildren<Renderer>(true);
        Vector3 originalScale = model.transform.localScale;

        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = 1f - (t / fadeDuration);
            float scale = Mathf.Lerp(1f, 0.3f, t / fadeDuration);

            foreach (var r in renderers)
                foreach (var mat in r.materials)
                    mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, alpha);

            model.transform.localScale = originalScale * scale;
            yield return null;
        }
    }

    private void RemoveOrganImmediate()
    {
        if (currentOrgan != null)
            Destroy(currentOrgan);

        if (labelManager != null)
            labelManager.ClearLabels();
    }
}