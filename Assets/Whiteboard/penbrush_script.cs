using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class penbrush_script : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Image penBrush;
    [SerializeField] private Whiteboard whiteboard_script;
    private RectTransform penBrushRT;

    // Start is called before the first frame update
    void Start()
    {
        whiteboard_script.penSize = 10; // penSize is 0 for some reason. set here as well
        penBrush = GetComponent<Image>();
        penBrushRT = GetComponent<RectTransform>();
        Debug.Log(whiteboard_script.penSize);
        penBrushRT.sizeDelta = new Vector2(whiteboard_script.penSize, whiteboard_script.penSize);
        penBrushRT.anchoredPosition = new Vector2(0, 0);
        Debug.Log(penBrushRT.anchoredPosition);
    }

    // Update is called once per frame
    void Update()
    {
        penBrushRT.anchoredPosition = new Vector2((int)(whiteboard_script.GetMouseWorldPosition().x - (whiteboard_script.textureSize.x/2)), (int)(whiteboard_script.GetMouseWorldPosition().y - (whiteboard_script.textureSize.y / 2)));
    }

    // added extra drawing events in case mouse clicks on brush and not whiteboard (it's either this or raycasting so i choose this)
    public void OnPointerDown(PointerEventData eventData)
    {
        // mousePositionOffset = gameObject.transform.position - GetMouseWorldPosition();
        whiteboard_script.mouseLeftClick = true;
        Debug.Log(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        whiteboard_script.mouseLeftClick = false;
        whiteboard_script.lastTouch = new Vector2(whiteboard_script.nullValue, whiteboard_script.nullValue); // indicates that there were no previous pixels placed in current brushstroke
        Debug.Log("onpointerup");
    }

    public void OnDrag(PointerEventData eventData)
    {
        whiteboard_script.mouseLeftClick = true;
    }
}
