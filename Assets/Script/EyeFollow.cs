using UnityEngine;

public class EyeFollow : MonoBehaviour
{
    public Transform Eye_L;
    public Transform Eye_R;
    public Transform eyeTarget;

    [Header("Limits (degrees)")]
    public float maxHorizontal = 18f;
    public float maxVertical = 12f;

    [Header("Smoothing")]
    public float smoothSpeed = 8f;

    [Header("Model axis correction (Inspector calibration)")]
    public Vector3 leftEyeOffsetEuler = Vector3.zero;
    public Vector3 rightEyeOffsetEuler = Vector3.zero;

    [Header("Center correction (use this if eyes rest slightly up/down/side)")]
    public Vector2 centerOffsetDegrees = Vector2.zero; 
    // X = pitch offset (up/down). Positive = look up.
    // Y = yaw offset (left/right). Positive = look right.

    [Header("Right eye mirror fix")]
    public bool invertYawOnRightEye = true;

    [Header("Optional: return to center when no face")]
    public bool returnToCenterWhenNoTarget = false;
    public float returnSpeedMultiplier = 0.6f;

    // Optional: if you have FaceReceiver expose a "hasFace" boolean
    public FaceReceiver faceReceiver;

    private Quaternion leftNeutral;
    private Quaternion rightNeutral;

    void Start()
    {
        if (Eye_L != null) leftNeutral = Eye_L.localRotation;
        if (Eye_R != null) rightNeutral = Eye_R.localRotation;
    }

    void Update()
    {
        if (Eye_L == null || Eye_R == null) return;

        bool noFace = false;
        // if (returnToCenterWhenNoTarget && faceReceiver != null)
        // {
        //     // Si tu FaceReceiver no tiene hasFace, pon returnToCenterWhenNoTarget=false
        //     noFace = !faceReceiver.hasFace;
        // }

        RotateEye(Eye_L, true, noFace);
        RotateEye(Eye_R, false, noFace);
    }

    void RotateEye(Transform eye, bool isLeft, bool noFace)
    {
        Quaternion neutral = isLeft ? leftNeutral : rightNeutral;

        // Si no hay cara y quieres volver al centro:
        if (noFace || eyeTarget == null)
        {
            float rs = smoothSpeed * returnSpeedMultiplier;
            eye.localRotation = Quaternion.Slerp(eye.localRotation, neutral, Time.deltaTime * rs);
            return;
        }

        Vector3 direction = eyeTarget.position - eye.position;
        if (direction.sqrMagnitude < 0.000001f) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        // Convertir a espacio local del padre del ojo
        Quaternion localRotation = Quaternion.Inverse(eye.parent.rotation) * lookRotation;

        Vector3 angles = localRotation.eulerAngles;
        if (angles.x > 180f) angles.x -= 360f;
        if (angles.y > 180f) angles.y -= 360f;

        if (eye == Eye_R) angles.y = -angles.y; //dont touch


        // Clamp a rangos humanos
        angles.x = Mathf.Clamp(angles.x, -maxVertical, maxVertical);
        angles.y = Mathf.Clamp(angles.y, -maxHorizontal, maxHorizontal);
        angles.z = 0f;

        // Fix espejo del ojo derecho (SOLO una vez)
        if (!isLeft && invertYawOnRightEye)
            angles.y = -angles.y;

        // Corrección del “centro” (si al inicio miran un poco arriba/abajo)
        angles.x -= centerOffsetDegrees.x; // restamos para corregir
        angles.y -= centerOffsetDegrees.y;

        Quaternion finalRotation = Quaternion.Euler(angles);

        // Offset por orientación del modelo
        Quaternion offset = Quaternion.Euler(isLeft ? leftEyeOffsetEuler : rightEyeOffsetEuler);
        finalRotation = finalRotation * offset;

        eye.localRotation = Quaternion.Slerp(
            eye.localRotation,
            neutral * finalRotation,
            Time.deltaTime * smoothSpeed
        );
    }

    // Opcional: presiona C para recalibrar el neutral en runtime (útil para pruebas)
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (Eye_L != null) leftNeutral = Eye_L.localRotation;
            if (Eye_R != null) rightNeutral = Eye_R.localRotation;
            Debug.Log("Recalibrated neutral (C).");
        }
    }
}
