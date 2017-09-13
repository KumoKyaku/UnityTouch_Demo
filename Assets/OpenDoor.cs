using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour {
    public Animator animator;
    Dictionary<string, Door> doors = new Dictionary<string, Door>();
	// Use this for initialization
	void Start () {
        doors.Add("D1", new Door() { name = "D1" });
        doors.Add("D2", new Door() { name = "D2" });
        doors.Add("D3", new Door() { name = "D3" });
        doors.Add("D4", new Door() { name = "D4" });
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButton(0))
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            OpenDoor2(r);
        }

        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                Ray r = Camera.main.ScreenPointToRay(t.position);
                OpenDoor2(r);
            }
        }

        foreach (var item in doors)
        {
            item.Value.Update(Time.deltaTime);
        }
	}

    private void OpenDoor2(Ray r)
    {
        //Gizmos.color = Color.white;
        //Gizmos.DrawRay(r);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit))
        {
            Transform door = hit.transform;
            Debug.Log(door.tag);
            var animaName = doors[door.tag].Check();
            if (animaName != "IDEL")
            {
                switch (door.tag)
                {
                    default:
                        break;
                }
                animator.Play(animaName);
            }
        }
    }

    class Door
    {
        public string name = "";
        public bool isOpen = true;
        float cooldown = 2f;
        public void Update(float time)
        {
            if (cooldown > 0)
            {
                cooldown -= time;
            }
        }

        public string Check()
        {
            if (cooldown > 0)
            {
                return "IDEL";
            }
            else
            {
                cooldown += 2f;
                isOpen = !isOpen;
                if (isOpen)
                {
                    return "O"+name+" 0";
                }
                else
                {
                    return "O" + name;
                }
            }
        }
    }
}
