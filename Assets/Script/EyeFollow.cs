using UnityEngine;

public class EyeFollow : MonoBehaviour
{
    public Transform Eye_L;
    public Transform Eye_R;
    public Transform eyeTarget;   
    public FaceReceiver faceReceiver; // script que recibe datos de Python

    [Range(0f, 30f)]
    public float maxRotation = 15f; // límite de rotación de los ojos
    public float smoothSpeed = 8f;  // suavizado
    public float maxHorizontal = 18f;
    public float maxVertical = 12f;
    

    private float smoothX;
    private float smoothY;
    public float inputSmooth = 5f; // suavizado de entrada

    void Update()
        {
            RotateEye(Eye_L);
            RotateEye(Eye_R);
        }

        void RotateEye(Transform eye)
        {
            Vector3 direction = eyeTarget.position - eye.position;

            Quaternion lookRotation = Quaternion.LookRotation(direction);

            Quaternion localRotation = Quaternion.Inverse(eye.parent.rotation) * lookRotation;

            Vector3 angles = localRotation.eulerAngles;

            // Convertimos a rango -180 a 180
            if (angles.x > 180) angles.x -= 360;
            if (angles.y > 180) angles.y -= 360;

            angles.x = Mathf.Clamp(angles.x, -maxVertical, maxVertical);
            angles.y = Mathf.Clamp(angles.y, -maxHorizontal, maxHorizontal);
            angles.z = 0;

            Quaternion finalRotation = Quaternion.Euler(angles);

            eye.localRotation = Quaternion.Slerp(
                eye.localRotation,
                finalRotation,
                Time.deltaTime * smoothSpeed
            );
        }
    // void Update()
    // {
    //     if (faceReceiver == null) return;

    //     // Convertimos de 0-1 a -1 a 1
    //     float targetX = (faceReceiver.faceX - 0.5f) * 2f;
    //     float targetY = (0.5f - faceReceiver.faceY) * 2f;

    //     smoothX = Mathf.Lerp(smoothX, targetX, Time.deltaTime * inputSmooth);
    //     smoothY = Mathf.Lerp(smoothY, targetY, Time.deltaTime * inputSmooth);

    //     // float xNorm = (faceReceiver.faceX - 0.5f) * 2f;
    //     // float yNorm = (0.5f - faceReceiver.faceY) * 2f;

    //     // Calculo rotación objetivo
    //     Quaternion targetRotation = Quaternion.Euler(smoothY * maxRotation, smoothX * maxRotation, 0);

    //     // Suavizamos
    //     Eye_L.localRotation = Quaternion.Slerp(Eye_L.localRotation, targetRotation, Time.deltaTime * smoothSpeed);
    //     Eye_R.localRotation = Quaternion.Slerp(Eye_R.localRotation, targetRotation, Time.deltaTime * smoothSpeed);

    //     // Eye_L.rotation = Quaternion.Slerp(Eye_L.rotation, targetRotation, Time.deltaTime * smoothSpeed);
    //     // Eye_R.rotation = Quaternion.Slerp(Eye_R.rotation, targetRotation, Time.deltaTime * smoothSpeed);
    // }
}

