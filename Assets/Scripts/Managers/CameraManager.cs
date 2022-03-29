using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : SingletonClass<CameraManager>
{
    Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    public void SetFocusAt(GameObject obj)
    {
        cam.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, cam.transform.position.z);
    }
}
