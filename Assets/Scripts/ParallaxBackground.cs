using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] Transform cam;
    [SerializeField] [Range(0f, 1f)] float parallaxFactor = 0.5f;

    Vector3 _startCamPos;
    Vector3 _startPos;

    void Start()
    {
        if (cam == null)
            cam = Camera.main != null ? Camera.main.transform : null;
        if (cam == null)
            return;
        _startCamPos = cam.position;
        _startPos = transform.position;
    }

    void LateUpdate()
    {
        if (cam == null)
            return;
        Vector3 delta = cam.position - _startCamPos;
        transform.position = new Vector3(
            _startPos.x + delta.x * parallaxFactor,
            _startPos.y + delta.y * parallaxFactor * 0.2f,
            _startPos.z);
    }
}
