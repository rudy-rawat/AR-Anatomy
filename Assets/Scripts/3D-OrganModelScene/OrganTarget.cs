using UnityEngine;
using Vuforia;
using System.Collections;

public class OrganTarget : MonoBehaviour
{
    [Header("Organ Info")]
    public string organType;
    public bool haveDetailVersion;

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
            
            // Add PinchToZoom component instead of ARTouchControls
            if (currentOrgan.GetComponent<PinchToZoom>() == null)
            {
                currentOrgan.AddComponent<PinchToZoom>();
            }
        }

        showingDetailed = detailed;
    }

    //private void RemoveOrgan()
    //{
    //    if (currentOrgan != null)
    //    {
    //        Destroy(currentOrgan);
    //    }
    //}

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

        model.transform.localScale = originalScale; // Ensure it's exact at end
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
    }

}
