using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PenHand : MonoBehaviour
{
    [SerializeField] private Whiteboard whiteboard_script;
    Vector3 mousePositionOffset;

    // Start is called before the first frame update
    void Start()
    {
        var this_penhand = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (whiteboard_script.whiteboardHover) 
        {
            // if cursor is hovering over whiteboard, show hand
            GetComponent<Renderer>().enabled = true;
            transform.position = new Vector3(whiteboard_script.GetMouseWorldPosition().x, whiteboard_script.GetMouseWorldPosition().y, -1);
        }
        else
        {
            GetComponent<Renderer>().enabled = false;
        }
    }
}
