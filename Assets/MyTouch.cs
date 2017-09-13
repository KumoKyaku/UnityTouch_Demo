using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyTouch : MonoBehaviour {
    public Camera cam;
    public Text label;
    public Transform target;
    public Toggle t;

    public void Clear()
    {
        if (label)
        {
            label.text = "";
        }

        
    }

    void Log(object obj)
    {
        Debug.LogError(obj);
        if (label)
        {
            label.text += ("\n" + obj.ToString());
        }

    }
	// Use this for initialization
	void Start () {
        if (!cam)
        {
            cam = Camera.main;
        }
        Log(Input.simulateMouseWithTouches);
        Log(1);

        if (!target)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            target = go.transform;
            target.position = Vector3.zero;
        }

        allowF = t.isOn;
        cam.transform.LookAt(target);
    }
	
	// Update is called once per frame
	void Update () {

        
        ControlCam();
        TouchZ();
    }

    void ControlCam()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 deltaPos = touch.deltaPosition;
            deltaPos *= zScale;
            cam.transform.RotateAround(target.position, Vector3.up, deltaPos.x);

            float yoff = -deltaPos.y;

            var nextx = cam.transform.eulerAngles.x + yoff;

            if (!allowF)
            {
                Log(nextx);

                if (nextx >= 89.5f && nextx <= 180)
                {
                    yoff = 89.5f - cam.transform.eulerAngles.x;
                }

                if (nextx <= 270.5f && nextx > 180)
                {
                    yoff = 270.5f - cam.transform.eulerAngles.x;
                }

            }

            cam.transform.RotateAround(target.position, cam.transform.right, yoff);
            
        }

        //if (Input.GetMouseButton(1))
        //{
        //    cam.transform.RotateAround(target.position, Vector3.up, Input.GetAxis("Mouse X"));

        //    float yoff = -Input.GetAxis("Mouse Y");
        //    yoff *= zScale;
        //    var nextx = cam.transform.eulerAngles.x + yoff;

        //    if (!allowF)
        //    {
        //        Log(nextx);

        //        if (nextx >= 89.5f && nextx <= 180)
        //        {
        //            yoff = 89.5f - cam.transform.eulerAngles.x;
        //        }

        //        if (nextx <= 270.5f && nextx > 180)
        //        {
        //            yoff = 270.5f - cam.transform.eulerAngles.x;
        //        }

        //    }

        //    cam.transform.RotateAround(target.position, cam.transform.right, yoff);


        //    //transform.rotation.x = Mathf.Clamp (transform.rotation.x,0,90);

        //    //cam.transform.eulerAngles = Vector3.Scale(cam.transform.eulerAngles, new Vector3(1, 1, 0));

            
        //}

        if (allowF)
        {

        }
        else
        {
            cam.transform.LookAt(target);
        }

        var z = Input.GetAxis("Mouse ScrollWheel");
        ScaleZ(z);
    }

    float dis = 0;
    void TouchZ()
    {
        if (Input.touchCount >= 2)
        {
            //多点触摸, 放大缩小  
            Touch newTouch1 = Input.GetTouch(0);
            Touch newTouch2 = Input.GetTouch(1);

            var newdis = Vector2.Distance(newTouch1.position, newTouch2.position);
            if (newTouch2.phase == TouchPhase.Began)
            {
                dis = newdis;
                return;
            }

            var z = newdis - dis;
            z *= zScale;
            ScaleZ(z);
            dis = newdis;
        }
    }

    float zScale = 0.1f;
    bool allowF = false;
    public void ToogleChange(bool value)
    {
        allowF = t.isOn;
        if (!allowF)
        {
            cam.transform.LookAt(target);
        }
    }

    void ScaleZ(float z)
    {

        cam.transform.Translate(new Vector3(0, 0, z));
        cam.transform.LookAt(target);
    }

}
