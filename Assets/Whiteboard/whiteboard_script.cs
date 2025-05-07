using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;
using Color = UnityEngine.Color;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Whiteboard : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    [NonSerialized] public int rangeUndoRedo = 10;
    public Texture2D[] textures;
    public Sprite sprite;
    public Vector2 textureSize;
    private int currentIndex;

    public bool whiteboardHover = false;
    [SerializeField] public int penSize = 10; // default penSize
    [SerializeField] private Colors pen_script;
    private Image whiteboard;
    public bool mouseLeftClick;
    public bool inWhiteboardBounds;
    [SerializeField] public bool lineMode = false;
    [SerializeField] public bool penMode = true;

    public Vector2 mousePositionOffset;
    public int nullValue = -123; // vectors can't be null, using this as replacement for null
    public Vector2 lastTouch; // where the whiteboard was last drawn on

    // Start is called before the first frame update
    public void Start()
    {
        textures = new Texture2D[rangeUndoRedo];
        lastTouch = new Vector2(nullValue, nullValue);
        whiteboard = GetComponent<Image>();
        textureSize = new Vector2(x: whiteboard.rectTransform.rect.width, y: whiteboard.rectTransform.rect.height);

        for (currentIndex = 0; currentIndex < textures.Length; currentIndex++)
        {
            textures[currentIndex] = new Texture2D(width: (int)textureSize.x, height: (int)textureSize.y);
            textures[currentIndex].name = "blank texture " + (currentIndex);
            CleanCanvas();
        }

        currentIndex = 0;

        whiteboard.sprite = Sprite.Create(textures[currentIndex], new Rect(0, 0, (int)textureSize.x, (int)textureSize.y), Vector2.zero, 100, 0, SpriteMeshType.FullRect, Vector4.zero, false, null); // set texture
        penSize = 10;
    }

    // Update is called once per frame
    void Update()
    {
        var my_draw_x = (int)(GetMouseWorldPosition().x - mousePositionOffset.x);
        var my_draw_y = (int)(GetMouseWorldPosition().y - mousePositionOffset.y);
        // default values

        // if the pen is in whiteboard bounds
        inWhiteboardBounds = GetMouseWorldPosition().x - mousePositionOffset.x > 0 && GetMouseWorldPosition().x + mousePositionOffset.x < textureSize.x && GetMouseWorldPosition().y - mousePositionOffset.y > 0 && GetMouseWorldPosition().y + mousePositionOffset.y < textureSize.y;

        if (mouseLeftClick && inWhiteboardBounds && penMode) // normal drawing mode
        {
            PlacePixelCluster(my_draw_x, my_draw_y);
        }
        else
        {
        }
    }

    // draw
    public Vector3 GetMouseWorldPosition()
    {
        return Input.mousePosition;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        onWhiteboardClickDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onWhiteboardClickUp();
    }

    public void OnDrag(PointerEventData eventData)
    {
        onWhiteboardDrag();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        whiteboardHover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        whiteboardHover = false;
    }

    public void PlacePixelCluster(int draw_x, int draw_y)
    {
        textures[currentIndex].SetPixels(draw_x, draw_y, blockWidth: penSize, blockHeight: penSize, pen_script.myColorArray);

        // interpolate
        if (lastTouch.x != nullValue && lastTouch.y != nullValue)
        {
            for(float f = 0.01f; f < 1.00f; f+= 0.03f) // last value determines how many points in between now and lastTouch (brush smoothness)
            {
                var lerpX = (int)Mathf.Lerp(a: lastTouch.x, b: draw_x, t: f);
                var lerpY = (int)Mathf.Lerp(a: lastTouch.y, b: draw_y, t: f);
                textures[currentIndex].SetPixels(lerpX, lerpY, blockWidth: penSize, blockHeight: penSize, pen_script.myColorArray);
            }
        }
        textures[currentIndex].Apply();

        lastTouch = new Vector2(draw_x, draw_y);
    }

    public void ClearCanvas() // classified as a move
    {
        var saveColor = pen_script.myColor;

        for (int i = 0; i <= whiteboard.rectTransform.rect.width; i++)
        {
            for(int j = 0; j <= whiteboard.rectTransform.rect.height; j++)
            {
                textures[currentIndex].SetPixel(i, j, Color.white);
            }
        }
        textures[currentIndex].Apply();
        prepNewTexture();

        pen_script.myColor = saveColor;
    }

    public void CleanCanvas() // not classified as a move
    {
        var saveColor = pen_script.myColor;

        for (int i = 0; i <= whiteboard.rectTransform.rect.width; i++)
        {
            for (int j = 0; j <= whiteboard.rectTransform.rect.height; j++)
            {
                textures[currentIndex].SetPixel(i, j, Color.white);
            }
        }
        textures[currentIndex].Apply();

        pen_script.myColor = saveColor;
    }

    public void prepNewTexture() // "save" texture in current slot and move one slot forward
    {
        Debug.Log("prepping new texture...");
        if (currentIndex < textures.Length - 1) // there is an empty slot in front of currentTexture, move forward to the empty spot
        {
            textures[currentIndex] = Instantiate(textures[currentIndex]); // apparently you have to instantiate stuff to rename it (boo)
            textures[currentIndex].name = "edited texture " + (currentIndex); 

            textures[currentIndex + 1] = Instantiate(textures[currentIndex]); // copy texture
            textures[currentIndex + 1].name = "current texture " + (currentIndex + 1) + " (you're looking at this one!)";

            if(currentIndex < textures.Length - 2)
            {
                textures[currentIndex + 2] = Instantiate(textures[currentIndex + 2]); // copy texture
                textures[currentIndex + 2].name = "blank texture " + (currentIndex + 2);
            }

            //textures[currentIndex + 1].LoadRawTextureData(textures[currentIndex].GetRawTextureData());
            whiteboard.sprite = Sprite.Create(textures[currentIndex + 1], new Rect(0, 0, (int)textureSize.x, (int)textureSize.y), Vector2.zero, 100, 0, SpriteMeshType.FullRect, Vector4.zero, false, null);
            
            currentIndex++;

            Debug.Log("moved one texture forward. current texture: " + currentIndex);
        }
        else // there are no empty slots ahead of currentTexture (we're on the last texture), move everything one slot backward to free up the last texture 
        {
            for(int i = 1; i  < textures.Length; i++)
            {
                textures[i - 1] = Instantiate(textures[i]);
                textures[i - 1].name = "edited texture " + (i - 1); // so everything isn't marked with (Clone)
                // Debug.Log("copied texture " + i + " into texture " + (i - 1));
            }
            whiteboard.sprite = Sprite.Create(textures[currentIndex], new Rect(0, 0, (int)textureSize.x, (int)textureSize.y), Vector2.zero, 100, 0, SpriteMeshType.FullRect, Vector4.zero, false, null);
            // currentIndex--;
            Debug.Log("shifted all textures a step back. current texture: " + currentIndex);
        }
    }

    public void undoTexture()
    {
        // rename old texture
        textures[currentIndex] = Instantiate(textures[currentIndex]);
        textures[currentIndex].name = "edited texture " + (currentIndex);
        if(currentIndex >= 1)
        {
            currentIndex--;
        }

        // rename new texture
        textures[currentIndex] = Instantiate(textures[currentIndex]); 
        textures[currentIndex].name = "current texture " + (currentIndex) + " (you're looking at this one!)";

        whiteboard.sprite = Sprite.Create(textures[currentIndex], new Rect(0, 0, (int)textureSize.x, (int)textureSize.y), Vector2.zero, 100, 0, SpriteMeshType.FullRect, Vector4.zero, false, null);
            Debug.Log("undo. current texture: " + currentIndex);
    }

    public void redoTexture()
    {
        if (currentIndex < textures.Length - 1 && (!(textures[currentIndex + 1].name).Contains("blank")))
        {   
            // rename textures
            textures[currentIndex] = Instantiate(textures[currentIndex]); // apparently you have to instantiate stuff to rename it (boo)
            textures[currentIndex].name = "edited texture " + (currentIndex);
            textures[currentIndex + 1] = Instantiate(textures[currentIndex + 1]); // copy texture
            textures[currentIndex + 1].name = "current texture " + (currentIndex + 1) + " (you're looking at this one!)";

            currentIndex++;
            whiteboard.sprite = Sprite.Create(textures[currentIndex], new Rect(0, 0, (int)textureSize.x, (int)textureSize.y), Vector2.zero, 100, 0, SpriteMeshType.FullRect, Vector4.zero, false, null);
        }
        Debug.Log("undo. current texture: " + currentIndex);
    }

    public int getCurrentIndex()
    {
        return currentIndex; 
    }

    public void onWhiteboardClickDown()
    {
        prepNewTexture();
        mouseLeftClick = true;
        Debug.Log("onpointerdown");
        if (lineMode)
        {
            
        }
    }

    public void onWhiteboardClickUp()
    {
        mouseLeftClick = false;
        lastTouch = new Vector2(nullValue, nullValue); // indicates that there were no previous pixels placed in current brushstroke
        Debug.Log("onpointerup");
    }

    public void onWhiteboardDrag()
    {
        mouseLeftClick = true;
    }

}
