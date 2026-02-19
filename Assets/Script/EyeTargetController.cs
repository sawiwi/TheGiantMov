using UnityEngine;

public class EyeTargetController : MonoBehaviour
{   

    public Transform eyeTarget;
    public FaceReceiver receiver;

    public float rangeX = 1.2f;
    public float rangeY = 0.8f;
    public float depth = 2f;
    public float smooth = 4f;

    void Update()
    {
        if (receiver == null) return;

        float x = (receiver.faceX - 0.5f) * rangeX;
        float y = (0.5f - receiver.faceY) * rangeY;

        Vector3 targetPos = new Vector3(x, y, depth);

        eyeTarget.localPosition = Vector3.Lerp(
            eyeTarget.localPosition,
            targetPos,
            Time.deltaTime * smooth
        );
    }
}

