using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] private Transform CTarget;
    [SerializeField] private Vector3 Offset;

    private void LateUpdate()
    {
        FollowTargetCamera();
    }

    private void FollowTargetCamera()
    {
        Camera Camera1 = this;
        Camera1.transform.position = CTarget.position + Offset;
    }
}