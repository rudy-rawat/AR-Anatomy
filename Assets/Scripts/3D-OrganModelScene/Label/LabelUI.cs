using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LabelUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI labelText;
    public Image lineImage;
    public RectTransform lineRect;

    private LabelPoint labelPoint;
    private Camera mainCamera;
    private RectTransform rectTransform;
    private Canvas canvas;
    private float lineLength;
    private float labelDistance;

    public void Initialize(LabelPoint point, float length, float distance)
    {
        labelPoint = point;
        lineLength = length;
        labelDistance = distance;
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        // Setup text
        if (labelText != null)
        {
            labelText.text = labelPoint.labelText;
            labelText.color = labelPoint.labelColor;
        }

        // Setup line color
        if (lineImage != null)
        {
            lineImage.color = labelPoint.labelColor;
        }
    }

    public void UpdatePosition()
    {
        if (labelPoint.anchorPoint == null || mainCamera == null)
            return;

        Vector3 anchorWorldPos = labelPoint.anchorPoint.position;

        // Check if point is behind camera
        Vector3 viewPos = mainCamera.WorldToViewportPoint(anchorWorldPos);
        if (viewPos.z < 0)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        // For World Space Canvas - position directly in world space
        if (canvas.renderMode == RenderMode.WorldSpace)
        {
            UpdateWorldSpacePosition(anchorWorldPos);
        }
        else
        {
            // Fallback for Screen Space (less ideal for AR)
            UpdateScreenSpacePosition(anchorWorldPos);
        }
    }

    private void UpdateWorldSpacePosition(Vector3 anchorWorldPos)
    {
        // Calculate direction from anchor to camera
        Vector3 toCamera = (mainCamera.transform.position - anchorWorldPos).normalized;

        // Calculate offset direction (up and to the right relative to camera)
        Vector3 cameraRight = mainCamera.transform.right;
        Vector3 cameraUp = mainCamera.transform.up;
        Vector3 offsetDirection = (cameraRight + cameraUp * 0.5f).normalized;

        // Position label at offset from anchor
        Vector3 labelWorldPos = anchorWorldPos + offsetDirection * labelDistance;

        // Set position
        transform.position = labelWorldPos;

        // Make label face camera
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                         mainCamera.transform.rotation * Vector3.up);

        // Update line
        if (lineRect != null)
        {
            UpdateWorldSpaceLine(anchorWorldPos, labelWorldPos);
        }
    }

    private void UpdateWorldSpaceLine(Vector3 startWorldPos, Vector3 endWorldPos)
    {
        // Position line at anchor point in world space
        lineRect.position = startWorldPos;

        // Calculate direction
        Vector3 direction = endWorldPos - startWorldPos;
        float distance = direction.magnitude;

        // Make line look at the label
        lineRect.LookAt(endWorldPos, mainCamera.transform.up);

        // Set line length (scale in X direction)
        float worldScale = canvas.transform.lossyScale.x;
        lineRect.sizeDelta = new Vector2(distance / worldScale, lineRect.sizeDelta.y);
    }

    private void UpdateScreenSpacePosition(Vector3 anchorWorldPos)
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(anchorWorldPos);

        // Convert to canvas position
        Vector2 canvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.worldCamera,
            out canvasPos
        );

        // Offset label to the side
        Vector2 labelOffset = new Vector2(lineLength, lineLength * 0.5f);
        rectTransform.anchoredPosition = canvasPos + labelOffset;

        // Update line
        if (lineRect != null)
        {
            UpdateScreenSpaceLine(canvasPos, rectTransform.anchoredPosition);
        }
    }

    private void UpdateScreenSpaceLine(Vector2 startPos, Vector2 endPos)
    {
        // Position line at anchor point
        lineRect.anchoredPosition = startPos;

        // Calculate direction and distance
        Vector2 direction = endPos - startPos;
        float distance = direction.magnitude;

        // Rotate line to point toward label
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineRect.rotation = Quaternion.Euler(0, 0, angle);

        // Set line length
        lineRect.sizeDelta = new Vector2(distance, lineRect.sizeDelta.y);
    }
}