using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera cinemachineCamera;
    public float zoomInFOV = 40f;
    public float zoomSpeed = 5f;
    private float defaultFOV;
    private bool isZoomedIn = false;
    private Coroutine currentZoomCoroutine = null;

    void Start()
    {
        if (cinemachineCamera != null)
        {
            defaultFOV = cinemachineCamera.m_Lens.FieldOfView;
        }
    }

    public void ZoomIn()
    {
        if (isZoomedIn) return;
        isZoomedIn = true;

        if (currentZoomCoroutine != null)
            StopCoroutine(currentZoomCoroutine);

        currentZoomCoroutine = StartCoroutine(ZoomCoroutine(zoomInFOV));
    }

    public void ZoomOut()
    {
        if (!isZoomedIn) return;
        isZoomedIn = false;

        if (currentZoomCoroutine != null)
            StopCoroutine(currentZoomCoroutine);

        currentZoomCoroutine = StartCoroutine(ZoomCoroutine(defaultFOV));
    }

    private IEnumerator ZoomCoroutine(float targetFOV)
    {
        while (!Mathf.Approximately(cinemachineCamera.m_Lens.FieldOfView, targetFOV))
        {
            cinemachineCamera.m_Lens.FieldOfView = Mathf.Lerp(
                cinemachineCamera.m_Lens.FieldOfView,
                targetFOV,
                zoomSpeed * Time.deltaTime
            );
            yield return null;
        }
        cinemachineCamera.m_Lens.FieldOfView = targetFOV;
    }
}
