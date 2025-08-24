using UnityEngine;

[System.Serializable]
public class ParallaxElement
{
    public Transform target;
    public float depth = 1f;

    [HideInInspector]
    public Vector3 initialPosition;
}

public class Parallax : MonoBehaviour
{
    public ParallaxElement[] layers;


    public Transform cameraTransform;

    private Vector3 lastCameraPosition;

    void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (cameraTransform == null)
        {
            Debug.LogError("카메라를 못 찾았습니다.");
            enabled = false;
            return;
        }

        lastCameraPosition = cameraTransform.position;

        foreach (var layer in layers)
        {
            if (layer.target != null)
                layer.initialPosition = layer.target.position;
        }
    }

    void LateUpdate()
    {
        Vector3 delta = cameraTransform.position - lastCameraPosition;

        foreach (var layer in layers)
        {
            if (layer.target == null) continue;

            float factor = 1f / Mathf.Max(layer.depth, 0.01f);
            Vector3 newPos = layer.initialPosition + delta * factor;
            layer.target.position = new Vector3(newPos.x, newPos.y, layer.target.position.z);
        }

        lastCameraPosition = cameraTransform.position;
    }
}
