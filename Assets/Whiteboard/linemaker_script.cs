using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class linemaker_script : MonoBehaviour
{

    [SerializeField] private Whiteboard whiteboard_script;
    private RectTransform lineMakerRT;
    private UnityEngine.UI.Image lineMakerIM;

    // Start is called before the first frame update
    void Start()
    {
        lineMakerRT = GetComponent<RectTransform>();
        lineMakerIM = GetComponent<UnityEngine.UI.Image>();
        lineMakerIM.gameObject.SetActive(false); // linemode is off by default
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void rotateLine()
    {
        // calculate angle between cursor and object
        Vector2 object_pos = new Vector2(lineMakerRT.position.x, lineMakerRT.position.y);
        float rel_x = (float)(whiteboard_script.GetMouseWorldPosition().x - object_pos.x);
        float rel_y = (float)(whiteboard_script.GetMouseWorldPosition().y - object_pos.y);
        float angle = Mathf.Atan2(rel_y, rel_x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); // rotate around parent object (parent object has axis at edge)
        float new_len = Mathf.Sqrt(Mathf.Pow(whiteboard_script.GetMouseWorldPosition().x - whiteboard_script.lineStart.x, 2) + Mathf.Pow(whiteboard_script.GetMouseWorldPosition().y - whiteboard_script.lineStart.y, 2));
        lineMakerRT.sizeDelta = new Vector2(new_len, whiteboard_script.penSize);
    }

    public void setLineStart()
    {
        transform.position = whiteboard_script.lineStart;
    }

    public void hideLine()
    {
        lineMakerIM.gameObject.SetActive(false);
    }

    public void showLine()
    {
        lineMakerIM.gameObject.SetActive(true);
    }
}
