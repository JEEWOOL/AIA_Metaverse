using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    // The target we are following
    [SerializeField]
    public Transform target;
    // The distance in the x-z plane to the target
    [SerializeField]
    public float distance = 3;
    // the height we want the camera to be above the target
    [SerializeField]
    public float height = 2;

    [SerializeField]
    public float rotationDamping = 0.5f;
    [SerializeField]
    public float heightDamping = 0.5f;

    [SerializeField]
    public float tarX = 2;
    [SerializeField]
    public float tarY = 2;
    [SerializeField]
    public float tarZ = 2;



    // Use this for initialization
    void Start() { }

    // Update is called once per frame
    void LateUpdate()
    {
        // Early out if we don't have a target
        if (!target)
            return;

        // Calculate the current rotation angles
        var wantedRotationAngle = target.eulerAngles.y;
        var wantedHeight = target.position.y + height;

        var currentRotationAngle = transform.eulerAngles.y;
        var currentHeight = transform.position.y;

        // Damp the rotation around the y-axis
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        // Convert the angle into a rotation
        var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        transform.position = target.position;
        transform.position -= currentRotation * Vector3.forward * distance;

        // Set the height of the camera
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
        // Always look at the target
        transform.LookAt(new Vector3(target.position.x - tarX, target.position.y + tarY, target.position.z + tarZ));
    }
}
