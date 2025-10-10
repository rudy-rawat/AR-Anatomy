using UnityEngine;

public class PinchToZoom : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float zoomSpeed = 0.01f;
    public float minScale = 5f;
    public float maxScale = 50f;
    
    [Header("Rotation Settings")]
    public float rotationSpeed = 100f;
    public bool enableRotation = true;
    
    private bool isTwoFingerGesture = false;
    private float previousTouchDistance;
    private Vector3 currentScale; // Store current scale

    void Start()
    {
        currentScale = transform.localScale;
    }

    void Update()
    {
        HandleTouchInput();
        
#if UNITY_EDITOR
        HandleMouseInput();
#endif
    }
    
    void HandleTouchInput()
    {
        int touchCount = Input.touchCount;

        if (touchCount == 0)
        {
            if (isTwoFingerGesture)
            {
                // Touch ended, maintain current scale
                isTwoFingerGesture = false;
                currentScale = transform.localScale;
            }
            return;
        }

        if (touchCount == 1)
        {
            HandleSingleTouch();
        }
        else if (touchCount == 2)
        {
            HandleTwoFingerGestures();
        }
    }

    void HandleSingleTouch()
    {
        if (isTwoFingerGesture) return;

        Touch touch = Input.GetTouch(0);

        if (enableRotation && touch.phase == TouchPhase.Moved)
        {
            // Rotate model only around Y-axis (vertical/middle axis)
            Vector2 touchDelta = touch.deltaPosition;

            float rotationY = -touchDelta.x * rotationSpeed * Time.deltaTime;

            // Only rotate around Y-axis (up vector)
            transform.Rotate(Vector3.up, rotationY, Space.World);
        }
    }

    void HandleTwoFingerGestures()
    {
        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);

        if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
        {
            isTwoFingerGesture = true;
            previousTouchDistance = Vector2.Distance(touch0.position, touch1.position);
            return;
        }

        if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
        {
            float currentDistance = Vector2.Distance(touch0.position, touch1.position);
            float deltaDistance = currentDistance - previousTouchDistance;

            if (Mathf.Abs(deltaDistance) > 1f) // Threshold to avoid jitter
            {
                float scaleFactor = 1 + deltaDistance * zoomSpeed * 0.01f;
                Vector3 newScale = currentScale * scaleFactor;
                newScale = Vector3.Max(Vector3.one * minScale, 
                                      Vector3.Min(newScale, Vector3.one * maxScale));
                transform.localScale = newScale;
                currentScale = newScale; // Update stored scale
            }

            previousTouchDistance = currentDistance;
        }
    }

#if UNITY_EDITOR
    void HandleMouseInput()
    {
        // Mouse scroll for zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            float scaleFactor = 1 + scroll * zoomSpeed;
            Vector3 newScale = currentScale * scaleFactor;
            newScale = Vector3.Max(Vector3.one * minScale, 
                                  Vector3.Min(newScale, Vector3.one * maxScale));
            transform.localScale = newScale;
            currentScale = newScale; // Update stored scale
        }

        // Mouse drag for rotation - only Y-axis
        if (Input.GetMouseButton(0) && enableRotation)
        {
            float mouseX = Input.GetAxis("Mouse X");

            // Only rotate around Y-axis (up vector)
            transform.Rotate(Vector3.up, -mouseX * rotationSpeed * Time.deltaTime, Space.World);
        }
    }
#endif
}
