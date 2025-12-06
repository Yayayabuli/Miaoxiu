using UnityEngine;

public class CameraZoomButton : MonoBehaviour
{
    public Camera cam;  
    public float zoomStep = 0.5f;   
    public float minZoom = 2f;     
    public float maxZoom = 8f;      
    public float smoothTime = 0.2f;

    private float targetZoom;
    private float velocity = 0f;

    void Start()
    {
        if (cam == null)
            cam = Camera.main;

        targetZoom = cam.orthographicSize;
    }

    void Update()
    {
        cam.orthographicSize = Mathf.SmoothDamp(
            cam.orthographicSize,
            targetZoom,
            ref velocity,
            smoothTime
        );
    }

 
    public void ZoomIn()
    {
        Debug.Log("ZoomIn Clicked");  
        targetZoom -= zoomStep;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
    }


    public void ZoomOut()
    {
        targetZoom += zoomStep;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
    }
    
    
}