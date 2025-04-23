using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;
using Color = UnityEngine.Color;

public class Whiteboard : MonoBehaviour
{
    public Texture2D texture;
    public Vector2 textureSize;
    public bool whiteboardHover = false;
    [SerializeField] private int penSize;
    [SerializeField] private Colors pen_script;
    private SpriteRenderer whiteboard;
    private bool pressingMouse;

    Vector3 mousePositionOffset;
    public float defaultX;
    public float defaultY;

    // Start is called before the first frame update
    void Start()
    {

        whiteboard = GetComponent<SpriteRenderer>();
        textureSize = new Vector2(x: whiteboard.bounds.size.x, y: whiteboard.bounds.size.y);
        texture = new Texture2D(width: (int)textureSize.x, height: (int)textureSize.y);

        var my_sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 100, 0, SpriteMeshType.FullRect, Vector4.zero, false, null);
        whiteboard.sprite = my_sprite;
        //whiteboard.material.mainTexture = texture;
        penSize = 10;
    }

    // Update is called once per frame
    void Update()
    {
        // default values
        whiteboardHover = false;
        pressingMouse = false;
    }

    // draw
    public Vector3 GetMouseWorldPosition()
    {
        //capture mouse position & return world point
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    private void OnMouseDown()
    {
    }

    private void OnMouseDrag()
    {
        Debug.Log("pressing mouse");
        pressingMouse = true;
    }

    private void OnMouseOver()
    {
        whiteboardHover = true;
        // Debug.Log("hovering over whiteboard");
        if (pressingMouse)
        {
            Draw();
        }
    }

    private void Draw()
    {
        texture.SetPixels((int) GetMouseWorldPosition().x, (int) GetMouseWorldPosition().y, blockWidth: penSize, blockHeight: penSize, pen_script.myColorArray);
        texture.Apply();
        Debug.Log("Drawing");
    }
}
