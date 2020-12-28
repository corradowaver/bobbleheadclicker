using UnityEngine;

namespace Utils
{
    public class InputUtils
    {
        public static Vector3 GetMouseWorldPosition() {
            Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
            vec.z = 0f;
            return vec;
        }
        public static Vector3 GetMouseWorldPositionWithZ() {
            return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        }
        public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera) {
            return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
        }
        public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera) {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }
        
        public static Vector3 GetTouchWorldPosition() {
            Vector3 vec = GetMouseWorldPositionWithZ(Input.GetTouch(0).position, Camera.main);
            vec.z = 0f;
            return vec;
        }
        public static Vector3 GetTouchWorldPositionWithZ() {
            return GetMouseWorldPositionWithZ(Input.GetTouch(0).position, Camera.main);
        }
        public static Vector3 GetTouchWorldPositionWithZ(Camera worldCamera) {
            return GetMouseWorldPositionWithZ(Input.GetTouch(0).position, worldCamera);
        }
        public static Vector3 GetTouchWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera) {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }
    }
}