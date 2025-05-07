using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;

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

        Vector2 object_pos = new Vector2(lineMakerRT.position.x, lineMakerRT.position.y);
        float rel_x = (float)(whiteboard_script.GetMouseWorldPosition().x - object_pos.x);
        float rel_y = (float)(whiteboard_script.GetMouseWorldPosition().y - object_pos.y);
        float angle = Mathf.Atan2(rel_y, rel_x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        
    }
}
