using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class OrganLabelManager : MonoBehaviour
{
    [Header("Label Prefab")]
    public GameObject labelPrefab;  // Assign the UI label prefab in inspector

    [Header("Label Settings")]
    public Canvas labelCanvas;      // Canvas to hold all labels (should be World Space)
    public Material lineMaterial;   // Material for the connecting lines
    public float labelDistance = 0.05f; // Distance from anchor point to label (in world units)
    public bool showLabels = true;  // Toggle to show/hide all labels

    private List<LabelUI> activeLabelUIs = new List<LabelUI>();

    public void SetupLabels(LabelPoint[] labelPoints)
    {
        ClearLabels();

        if (!showLabels || labelPoints == null || labelPoints.Length == 0)
        {
            Debug.Log("No labels to setup or labels disabled");
            return;
        }

        Debug.Log($"Setting up {labelPoints.Length} labels");

        foreach (var point in labelPoints)
        {
            if (point.anchorPoint == null)
            {
                Debug.LogWarning($"Label '{point.labelText}' anchor point is null, skipping...");
                continue;
            }

            Debug.Log($"Creating label '{point.labelText}' at anchor position: {point.anchorPoint.position}");
            CreateLabel(point);
        }

        Debug.Log($"Total active labels: {activeLabelUIs.Count}");
    }

    private void CreateLabel(LabelPoint labelPoint)
    {
        if (labelPrefab == null || labelCanvas == null)
        {
            Debug.LogError("Label prefab or canvas not assigned!");
            return;
        }

        GameObject labelObj = Instantiate(labelPrefab, labelCanvas.transform);
        LabelUI labelUI = labelObj.GetComponent<LabelUI>();

        if (labelUI == null)
        {
            labelUI = labelObj.AddComponent<LabelUI>();
        }

        labelUI.Initialize(labelPoint, labelDistance, lineMaterial);
        activeLabelUIs.Add(labelUI);
    }

    public void ClearLabels()
    {
        foreach (var label in activeLabelUIs)
        {
            if (label != null)
                Destroy(label.gameObject);
        }
        activeLabelUIs.Clear();
    }

    public void ToggleLabels(bool visible)
    {
        showLabels = visible;
        foreach (var label in activeLabelUIs)
        {
            if (label != null)
                label.gameObject.SetActive(visible);
        }
    }

    void LateUpdate()
    {
        // Update all label positions every frame
        foreach (var label in activeLabelUIs)
        {
            if (label != null)
                label.UpdatePosition();
        }
    }

    void OnDestroy()
    {
        ClearLabels();
    }
}