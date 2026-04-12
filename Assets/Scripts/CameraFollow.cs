using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new Vector3(0f, 0f, -10f);
    [SerializeField] float smoothTime = 0.15f;

    Vector3 _vel;
    float _fixedY;
    float _fixedZ;

    void Start()
    {
        _fixedY = transform.position.y + offset.y;
        _fixedZ = offset.z;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 goal = new Vector3(target.position.x + offset.x, _fixedY, target.position.z + _fixedZ);
        transform.position = Vector3.SmoothDamp(transform.position, goal, ref _vel, smoothTime);
    }

    public void SetTarget(Transform t) => target = t;
}
