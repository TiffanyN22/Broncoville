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
    [SerializeField] private int penSize;
    [SerializeField] private Colors pen_script;
    private Image whiteboard;
    private bool pressingMouse;

    public Vector3 mousePositionOffset;

    // Start is called before the first frame update
    void Start()
    {
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

        if (pressingMouse && whiteboardHover)
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
        pressingMouse = true;
        Debug.Log(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressingMouse = false;
        Debug.Log("onpointerup");
    }

    public void OnDrag(PointerEventData eventData)
    {
        pressingMouse = true;
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
        texture.SetPixels((int) (GetMouseWorldPosition().x + mousePositionOffset.x), (int) (GetMouseWorldPosition().y + mousePositionOffset.y), blockWidth: penSize, blockHeight: penSize, pen_script.myColorArray);
        texture.Apply();
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
