using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class linemaker_script : MonoBehaviour
{

    [SerializeField] private Whiteboard whiteboard_script;
    private RectTransform lineMakerRT;

    // Start is called before the first frame update
    void Start()
    {
        lineMakerRT = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        // calculate angle between cursor and object
        /*
        Vector2 Point_1 = new Vector2(whiteboard_script.GetMouseWorldPosition().x, whiteboard_script.GetMouseWorldPosition().y);
        Vector2 Point_2 = new Vector2(0, 0);
        float rotation = Mathf.Atan2(Point_2.y - Point_1.y, Point_2.x - Point_1.x) * 180 / Mathf.PI;
        lineMakerRT.transform.Rotate(0, 0, rotation);
        */
    }
}
