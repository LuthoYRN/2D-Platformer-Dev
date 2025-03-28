using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float currentPosX;
    private Vector3 velocity = Vector3.zero;
    public void MoveToCheckPoint(Transform _checkpoint)
    {
        print("here");
        currentPosX = _checkpoint.position.x;
    }
}