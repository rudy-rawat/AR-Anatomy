using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LabelUI : MonoBehaviour
{
    [Header("UI References")]
    public Image backgroundImage;
    public TextMeshProUGUI labelText;

    [Header("Line Settings")]
    public Material lineMaterial;
    public float lineWidth = 0.002f;

    private LabelPoint labelPoint;
    private Camera mainCamera;
    private Canvas canvas;
    private float labelDistance;
    private LineRenderer lineRenderer;

    public void Initialize(LabelPoint point, float distance, Material lineMat)
    {
        labelPoint = point;
        labelDistance = distance;
        lineMaterial = lineMat;
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();

        // Get text from anchor GameObject name
        if (labelPoint.anchorPoint != null)
        {
            string anchorName = labelPoint.anchorPoint.name;
            if (labelText != null)
            {
                labelText.text = anchorName;
                labelText.color = labelPoint.labelColor;
            }
        }

        // Setup background
        if (backgroundImage != null)
        {
            backgroundImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        }

        // Add FaceCamera to this root so it faces camera
        if (GetComponent<FaceCamera>() == null)
        {
            gameObject.AddComponent<FaceCamera>();
        }

        // Create line renderer
        CreateLine();

        Debug.Log($"Label initialized: {labelText.text}");
    }

    private void CreateLine()
    {
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.SetParent(transform);
        lineObj.transform.localPosition = Vector3.zero;

        lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = labelPoint.labelColor;
        lineRenderer.endColor = labelPoint.labelColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
    }

    public void UpdatePosition()
    {
        if (labelPoint.anchorPoint == null || mainCamera == null)
            return;

        Vector3 anchorWorldPos = labelPoint.anchorPoint.position;

        // Check if behind camera
        Vector3 viewPos = mainCamera.WorldToViewportPoint(anchorWorldPos);
        if (viewPos.z < 0)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        // Calculate label position offset to the right of anchor
        Vector3 cameraRight = mainCamera.transform.right;
        Vector3 labelWorldPos = anchorWorldPos + cameraRight * labelDistance;

        // Position label (FaceCamera will handle rotation)
        transform.position = labelWorldPos;

        // Update line positions
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, anchorWorldPos);
            lineRenderer.SetPosition(1, labelWorldPos);
        }
    }
}