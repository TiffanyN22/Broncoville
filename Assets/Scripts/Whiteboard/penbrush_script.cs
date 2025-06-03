using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class penbrush_script : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RawImage penBrush;
    [SerializeField] private Whiteboard whiteboard_script;
    private RectTransform penBrushRT;
    [SerializeField] public int xoffset;
    [SerializeField] public int yoffset;

    // Start is called before the first frame update
    void Start()
    {
        whiteboard_script.penSize = 10; // penSize is 0 for some reason. set here as well
        penBrush = GetComponent<RawImage>();
        penBrushRT = GetComponent<RectTransform>();
        penBrushRT.sizeDelta = new Vector2(whiteboard_script.penSize, whiteboard_script.penSize);
        penBrushRT.anchoredPosition = new Vector2(0, 0);
    }

    public void penBrushSizeAdjust()
    {
        penBrushRT.sizeDelta = new Vector2(whiteboard_script.penSize, whiteboard_script.penSize);
    }

    // Update is called once per frame
    void Update()
    {
        float x_offset = xoffset + (whiteboard_script.textureSize.x / 2);
        float y_offset = yoffset + (whiteboard_script.textureSize.y / 2);
        penBrushRT.anchoredPosition = new Vector2((int)(whiteboard_script.GetMouseWorldPosition().x - (Screen.width / 2)), (int)(whiteboard_script.GetMouseWorldPosition().y - (Screen.height / 2)));
        //penBrushRT.anchoredPosition = new Vector2((int)(whiteboard_script.GetMouseWorldPosition().x - x_offset), (int)(whiteboard_script.GetMouseWorldPosition().y - y_offset));
        // penBrushRT.anchoredPosition = new Vector2(xoffset, yoffset);
    }

    // added extra drawing events in case mouse clicks on brush and not whiteboard (it's either this or raycasting so i choose this)
    public void OnPointerDown(PointerEventData eventData)
    {
        whiteboard_script.onWhiteboardClickDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        whiteboard_script.onWhiteboardClickUp();
    }

    public void OnDrag(PointerEventData eventData)
    {
        whiteboard_script.onWhiteboardDrag();
    }
}
