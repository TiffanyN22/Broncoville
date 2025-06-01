using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;
using Color = UnityEngine.Color;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

// add todo whenever things can be synced
// sync everything to whiteboard data file
// rework whiteboard_script to allow for transparent editing system

public class Whiteboard : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    [NonSerialized] public int rangeUndoRedo = 10;
    public Vector2 textureSize;
    public int currentIndex;
    public Vector2 lineStart;
    public Vector2 lineEnd;

    [NonSerialized] public bool whiteboardHover = false;
    [SerializeField] public int penSize = 10; // default penSize
    [SerializeField] private Colors pen_script;
    [SerializeField] private linemaker_script mylinemaker_script;
    private Image whiteboard;
    [NonSerialized] public bool mouseLeftClick;
    [NonSerialized] public bool inWhiteboardBounds;
    [NonSerialized] public bool lineMode = false;
    [NonSerialized] public bool penMode = true;

    [SerializeField] public GameObject whiteboardLayer;
    [NonSerialized] public GameObject[] whiteboardLayerArr;
    [NonSerialized] public Texture2D[] textures;

    public Vector2 mousePositionOffset;
    [NonSerialized] public int nullValue = -123; // vectors can't be null, using this as replacement for null
    [NonSerialized] public Vector2 lastTouch; // where the whiteboard was last drawn on

    // Start is called before the first frame update
    public void Start()
    {
        textures = new Texture2D[rangeUndoRedo];
        whiteboardLayerArr = new GameObject[rangeUndoRedo];

        lastTouch = new Vector2(nullValue, nullValue);
        whiteboard = GetComponent<Image>();
        textureSize = new Vector2(x: whiteboard.rectTransform.rect.width, y: whiteboard.rectTransform.rect.height);
        lineStart = new Vector2(0, 0);
        lineEnd = new Vector2(0, 0);

        for (currentIndex = 0; currentIndex < rangeUndoRedo; currentIndex++)
        {
            whiteboardLayerArr[currentIndex] = Instantiate(whiteboardLayer, this.transform); // create a new whiteboard layer as a child of WhiteboardManager
            whiteboardLayerArr[currentIndex].name = "blank layer " + (currentIndex);
            textures[currentIndex] = new Texture2D(width: (int)textureSize.x, height: (int)textureSize.y);
            whiteboardLayerArr[currentIndex].GetComponent<Image>().sprite = Sprite.Create(textures[currentIndex], new Rect(0, 0, (int)textureSize.x, (int)textureSize.y), Vector2.zero, 100, 0, SpriteMeshType.FullRect, Vector4.zero, false, null); // set texture
            CleanCanvas(ref textures[currentIndex]);

            // drawABlock(currentIndex);
        }

        currentIndex = 0;

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

    public void drawABlock(int index) // for testing. draws a block at texture index index
    {
        for(int x = 20; x < 100; x++)
        {
            for (int y = 20; y < 300; y++)
            {
                textures[index].SetPixel(x, y, Color.black);
            }
        }
        textures[currentIndex].Apply();
    }

    public void ClearCanvas() // classified as a move
    {
        prepNewTexture();
        var saveColor = pen_script.myColor;

        for (int i = 0; i <= (int)textureSize.x; i++)
        {
            for(int j = 0; j <= (int)textureSize.y; j++)
            {
                textures[currentIndex].SetPixel(i, j, Color.white);
            }
        }
        textures[currentIndex].Apply();

        pen_script.myColor = saveColor;
    }

    public void CleanCanvas(ref Texture2D layerTexture) // not classified as a move
    {
        var saveColor = pen_script.myColor;

        for (int i = 0; i <= (int)textureSize.x; i++)
        {
            for (int j = 0; j <= (int)textureSize.y; j++)
            {
                layerTexture.SetPixel(i, j, Color.clear);
            }
        }
        layerTexture.Apply();

        pen_script.myColor = saveColor;
    }

    public void prepNewTexture() // "save" texture in current slot and move one slot forward
    {
        Debug.Log("prepping new texture...");
        if (currentIndex < textures.Length - 1) // there is an empty slot in front of currentTexture, move forward to the empty spot
        {
            whiteboardLayerArr[currentIndex].name = "edited layer " + (currentIndex);

            //textures[currentIndex + 1] = new Texture2D(width: (int)textureSize.x, height: (int)textureSize.y);
            CleanCanvas(ref textures[currentIndex + 1]);
            whiteboardLayerArr[currentIndex + 1].name = "current layer " + (currentIndex + 1) + " (you're looking at this one!)";

            if (currentIndex < textures.Length - 2)
            {
                // textures[currentIndex + 2] = new Texture2D(width: (int)textureSize.x, height: (int)textureSize.y);
                CleanCanvas(ref textures[currentIndex + 2]);
                whiteboardLayerArr[currentIndex + 2].name = "blank layer " + (currentIndex + 2);
            }

            // whiteboard.sprite = Sprite.Create(textures[currentIndex + 1], new Rect(0, 0, (int)textureSize.x, (int)textureSize.y), Vector2.zero, 100, 0, SpriteMeshType.FullRect, Vector4.zero, false, null);
            currentIndex++;

            // Debug.Log("moved one texture forward. current texture: " + currentIndex);
        }
        else // there are no empty slots ahead of currentTexture (we're on the last texture), move everything one slot backward to free up the last texture 
        {
            for(int i = 0; i < textures.Length - 1; i++)
            {
                whiteboardLayerArr[i].GetComponent<Image>().sprite = Sprite.Create(Instantiate(textures[i + 1]), new Rect(0, 0, (int)textureSize.x, (int)textureSize.y), Vector2.zero, 100, 0, SpriteMeshType.FullRect, Vector4.zero, false, null);
                // textures[i] = Instantiate(textures[i + 1]);
                // textures[i].Apply();
                // Debug.Log("texture " + i + " becomes texture " + (i + 1));
            }
            CleanCanvas(ref textures[currentIndex]);
            // whiteboard.sprite = Sprite.Create(textures[currentIndex], new Rect(0, 0, (int)textureSize.x, (int)textureSize.y), Vector2.zero, 100, 0, SpriteMeshType.FullRect, Vector4.zero, false, null);
            Debug.Log("shifted all textures a step back. current texture: " + currentIndex);
        }
    }

    public void undoTexture()
    {
        // rename old texture
        whiteboardLayerArr[currentIndex].name = "edited layer " + (currentIndex);
        if (currentIndex >= 1)
        {
            currentIndex--;
        }

        // rename new texture
        whiteboardLayerArr[currentIndex].name = "current layer " + (currentIndex) + " (you're looking at this one!)";

        // whiteboard.sprite = Sprite.Create(textures[currentIndex], new Rect(0, 0, (int)textureSize.x, (int)textureSize.y), Vector2.zero, 100, 0, SpriteMeshType.FullRect, Vector4.zero, false, null);
            Debug.Log("undo. current texture: " + currentIndex);
    }

    public void combineTextures(Texture2D topTexture, Texture2D bottomTexture)
    {

    }

    public void redoTexture()
    {
        if (currentIndex < textures.Length - 1 && (!(whiteboardLayerArr[currentIndex + 1].name).Contains("blank")))
        {   
            // rename textures
            whiteboardLayerArr[currentIndex].name = "edited texture " + (currentIndex);

            whiteboardLayerArr[currentIndex + 1].name = "current layer " + (currentIndex + 1) + " (you're looking at this one!)";

            currentIndex++;
            // whiteboard.sprite = Sprite.Create(textures[currentIndex], new Rect(0, 0, (int)textureSize.x, (int)textureSize.y), Vector2.zero, 100, 0, SpriteMeshType.FullRect, Vector4.zero, false, null);
        }
        Debug.Log("undo. current texture: " + currentIndex);
    }

    public int getCurrentIndex()
    {
        return currentIndex; 
    }

    public void onWhiteboardClickDown()
    {
        mouseLeftClick = true;
        Debug.Log("onpointerdown");
        if (lineMode)
        {
            lineModeClickDown();
        }
    }

    public void onWhiteboardClickUp()
    {
        mouseLeftClick = false;
        lastTouch = new Vector2(nullValue, nullValue); // indicates that there were no previous pixels placed in current brushstroke
        Debug.Log("onpointerup");
        if (lineMode)
        {
            lineModeClickUp();
        }
        prepNewTexture();
    }

    public void onWhiteboardDrag()
    {
        mouseLeftClick = true;
        if (lineMode)
        {
            lineModeDrag();
        }
    }

    // LINE MODE FUNCTIONS
    private void lineModeDrag()
    {
        lineEnd = GetMouseWorldPosition();
        mylinemaker_script.rotateLine();
    }

    private void lineModeClickUp()
    {
        lineEnd = GetMouseWorldPosition();
        mylinemaker_script.rotateLine();
        mylinemaker_script.hideLine();

        int pixel_freq = 5; // in a given distance, how far apart should the pixels be
        float density = (float)(1 / (Mathf.Sqrt(Mathf.Pow(lineEnd.x - lineStart.x, 2) + Mathf.Pow(lineEnd.y - lineStart.y, 2)) / pixel_freq)); // line density

        for (float f = 0.01f; f < 1.00f; f += density) // last value determines how many points in between now and lastTouch (brush smoothness)
        {
            var lerpX = (int)Mathf.Lerp(a: lineStart.x, b: lineEnd.x, t: f);
            var lerpY = (int)Mathf.Lerp(a: lineStart.y, b: lineEnd.y, t: f);
            textures[currentIndex].SetPixels(lerpX - (penSize / 2), lerpY - (penSize / 2), blockWidth: penSize, blockHeight: penSize, pen_script.myColorArray);
            Debug.Log("x: " + lerpX + " y: " + lerpY);
        }
        textures[currentIndex].Apply();
    }

    private void lineModeClickDown()
    {
        lineStart = GetMouseWorldPosition();
        mylinemaker_script.rotateLine();
        mylinemaker_script.showLine();
        mylinemaker_script.setLineStart();
    }

}
