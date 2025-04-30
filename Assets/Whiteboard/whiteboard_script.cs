using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;
using Color = UnityEngine.Color;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Whiteboard : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    public Texture2D texture;
    public Vector2 textureSize;
    public bool whiteboardHover = false;
    [SerializeField] public int penSize = 10; // default penSize
    [SerializeField] private Colors pen_script;
    private Image whiteboard;
    public bool mouseLeftClick;
    public bool inWhiteboardBounds;

    public Vector2 mousePositionOffset;
    public int nullValue = -123; // vectors can't be null, using this as replacement for null
    public Vector2 lastTouch; // where the whiteboard was last drawn on

    // Start is called before the first frame update
    void Start()
    {
        lastTouch = new Vector2(nullValue, nullValue);
        whiteboard = GetComponent<Image>();
        textureSize = new Vector2(x: whiteboard.rectTransform.rect.width, y: whiteboard.rectTransform.rect.height);
        texture = new Texture2D(width: (int)textureSize.x, height: (int)textureSize.y);

        var my_sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 100, 0, SpriteMeshType.FullRect, Vector4.zero, false, null);
        whiteboard.sprite = my_sprite;
        //whiteboard.material.mainTexture = texture;
        penSize = 10;
        ClearCanvas();
    }

    // Update is called once per frame
    void Update()
    {
        // default values
        
        // if the pen is in whiteboard bounds
        inWhiteboardBounds = GetMouseWorldPosition().x - mousePositionOffset.x > 0 && GetMouseWorldPosition().x + mousePositionOffset.x < textureSize.x && GetMouseWorldPosition().y - mousePositionOffset.y > 0 && GetMouseWorldPosition().y + mousePositionOffset.y < textureSize.y;
        // mouseLeftClick = Input.GetMouseButtonDown(0);

        if (mouseLeftClick && inWhiteboardBounds)
        {
            Draw();
        }
        else
        {
        }
    }

    // draw
    public Vector3 GetMouseWorldPosition()
    {
        //capture mouse position & return world point
        //return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return Input.mousePosition;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        // mousePositionOffset = gameObject.transform.position - GetMouseWorldPosition();
        mouseLeftClick = true;
        Debug.Log(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mouseLeftClick = false;
        lastTouch = new Vector2(nullValue, nullValue); // indicates that there were no previous pixels placed in current brushstroke
        Debug.Log("onpointerup");
    }

    public void OnDrag(PointerEventData eventData)
    {
        mouseLeftClick = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        whiteboardHover = true;
        // Debug.Log("hovering over whiteboard");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        whiteboardHover = false;
    }

    private void Draw()
    {
        texture.SetPixels((int) (GetMouseWorldPosition().x - mousePositionOffset.x), (int) (GetMouseWorldPosition().y - mousePositionOffset.y), blockWidth: penSize, blockHeight: penSize, pen_script.myColorArray);

        // interpolate
        if (lastTouch.x != nullValue && lastTouch.y != nullValue)
        {
            for(float f = 0.01f; f < 1.00f; f+= 0.03f) // last value determines how many points in between now and lastTouch (brush smoothness)
            {
                var lerpX = (int)Mathf.Lerp(a: lastTouch.x, b: (int)(GetMouseWorldPosition().x - mousePositionOffset.x), t: f);
                var lerpY = (int)Mathf.Lerp(a: lastTouch.y, b: (int)(GetMouseWorldPosition().y - mousePositionOffset.y), t: f);
                texture.SetPixels(lerpX, lerpY, blockWidth: penSize, blockHeight: penSize, pen_script.myColorArray);
            }
        }
        texture.Apply();

        lastTouch = new Vector2(GetMouseWorldPosition().x - mousePositionOffset.x, (int)GetMouseWorldPosition().y - mousePositionOffset.y);
    }

    public void ClearCanvas()
    {
        for (int i = 0; i <= whiteboard.rectTransform.rect.width; i++)
        {
            for(int j = 0; j <= whiteboard.rectTransform.rect.height; j++)
            {
                texture.SetPixel(i, j, Color.white);
            }
        }
        texture.Apply();
    }
}
