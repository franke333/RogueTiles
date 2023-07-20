using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : SingletonClass<CameraManager>
{
    Camera cam;
    Vector3 target;

    const float lerpspeed = 5;
    [SerializeField]
    float cameraZoffset = -10;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        //smoothly move camera to target
        cam.transform.position = Vector3.Lerp(cam.transform.position, target, Time.deltaTime * lerpspeed);

    }

    /// <summary>
    /// Move camera so the object is in the center of the screen
    /// </summary>
    /// <param name="obj">Object that the camera centers at</param>
    public void SetFocusAt(GameObject obj)
    {
        target = new Vector3(obj.transform.position.x, obj.transform.position.y, cameraZoffset);
    }
}
