using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Offset from target (your isometric setup)")]
    public Vector3 offset = new Vector3(-14f, 17.5f, -15.5f);

    [Header("Fixed rotation (your isometric setup)")]
    public Vector3 fixedRotationEuler = new Vector3(36f, 41f, 0f);

    [Header("Follow smoothing")]
    public float smoothSpeed = 10f;

    void Start()
    {
        // lock rotation immediately
        transform.rotation = Quaternion.Euler(fixedRotationEuler);
    }

    void LateUpdate()
    {
        if (target == null) return;

        // keep camera at a fixed offset relative to player
        Vector3 desiredPos = target.position + offset;

        // direct follow (no smoothing)
        transform.position = desiredPos;

        // keep rotation locked (prevents drift)
        transform.rotation = Quaternion.Euler(fixedRotationEuler);
    }
}
