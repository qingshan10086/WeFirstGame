using UnityEngine;

namespace SceneScript
{
    public class CameraFollow2D : MonoBehaviour
    {
        [Header("Target to follow (e.g., the player)")]
        public Transform target;

        [Header("Camera offset (optional)")]
        public Vector3 offset = new Vector3(0, 0, -10);
        // Keeps the camera positioned correctly along the Z-axis, especially in 2D

        [Header("Follow smoothness")]
        public float smoothSpeed = 5f;
        // Higher values result in snappier movement; lower values make it smoother and more gradual

        void LateUpdate()
        {
            // Make sure a target has been assigned
            if (target == null) return;

            // Calculate the desired camera position based on the target’s position and offset
            Vector3 desiredPosition = target.position + offset;

            // Smoothly interpolate from the current position to the desired position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            // Update the camera's position
            transform.position = smoothedPosition;
        }
    }
}