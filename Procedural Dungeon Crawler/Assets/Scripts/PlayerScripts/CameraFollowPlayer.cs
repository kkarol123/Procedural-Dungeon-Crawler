using UnityEngine;

namespace PlayerScripts
{
    public class CameraFollowPlayer : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;
        private float smoothSpeed = 5f;
        private Vector3 offset = new Vector3(0f, 0f, -10f);

        void Update()
        {
            Vector3 targetPosition = playerTransform.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
    }
}