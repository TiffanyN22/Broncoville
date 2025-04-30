using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;
using Color = UnityEngine.Color;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static System.Net.Mime.MediaTypeNames;
using Image = UnityEngine.UI.Image;

public class Whiteboard : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    int rangeUndoRedo = 5;
    public Texture2D[] textures;
    public Sprite[] sprites;
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
    void Start()
    {
        textures = new Texture2D[rangeUndoRedo];
        sprites = new Sprite[rangeUndoRedo];
        currentIndex = 0; // current texture and sprite index
        lastTouch = new Vector2(nullValue, nullValue);
        whiteboard = GetComponent<Image>();
        textureSize = new Vector2(x: whiteboard.rectTransform.rect.width, y: whiteboard.rectTransform.rect.height);

        for(int i = 0; i < rangeUndoRedo; i++)
        {
            textures[i] = new Texture2D(width: (int)textureSize.x, height: (int)textureSize.y);
            sprites[i] = Sprite.Create(textures[i], new Rect(0, 0, textureSize.x, textureSize.y), Vector2.zero, 100, 0, SpriteMeshType.FullRect, Vector4.zero, false, null);
            ClearCanvas();
        }

        currentIndex = 0;

        whiteboard.sprite = sprites[0]; // set texture
        penSize = 10;
    }

    // Update is called once per frame
    void Update()
    {
        // default values

        // if the pen is in whiteboard bounds
        inWhiteboardBounds = GetMouseWorldPosition().x - mousePositionOffset.x > 0 && GetMouseWorldPosition().x + mousePositionOffset.x < textureSize.x && GetMouseWorldPosition().y - mousePositionOffset.y > 0 && GetMouseWorldPosition().y + mousePositionOffset.y < textureSize.y;

        if (mouseLeftClick && inWhiteboardBounds && penMode) // normal drawing mode
        {
            PlacePixelCluster();
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
        Debug.Log("onpointerdown");
        if (lineMode)
        {
            
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mouseLeftClick = false;
        lastTouch = new Vector2(nullValue, nullValue); // indicates that there were no previous pixels placed in current brushstroke
        prepNewTexture();
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

    private void PlacePixelCluster()
    {
        textures[currentIndex].SetPixels((int) (GetMouseWorldPosition().x - mousePositionOffset.x), (int) (GetMouseWorldPosition().y - mousePositionOffset.y), blockWidth: penSize, blockHeight: penSize, pen_script.myColorArray);

        // interpolate
        if (lastTouch.x != nullValue && lastTouch.y != nullValue)
        {
            for(float f = 0.01f; f < 1.00f; f+= 0.03f) // last value determines how many points in between now and lastTouch (brush smoothness)
            {
                var lerpX = (int)Mathf.Lerp(a: lastTouch.x, b: (int)(GetMouseWorldPosition().x - mousePositionOffset.x), t: f);
                var lerpY = (int)Mathf.Lerp(a: lastTouch.y, b: (int)(GetMouseWorldPosition().y - mousePositionOffset.y), t: f);
                textures[currentIndex].SetPixels(lerpX, lerpY, blockWidth: penSize, blockHeight: penSize, pen_script.myColorArray);
            }
        }
        textures[currentIndex].Apply();

        lastTouch = new Vector2(GetMouseWorldPosition().x - mousePositionOffset.x, (int)GetMouseWorldPosition().y - mousePositionOffset.y);
    }

    public void ClearCanvas()
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

        pen_script.myColor = saveColor;
    }

    public void prepNewTexture() // "save" texture in current slot and move one slot forward
    {
        if (currentIndex < textures.Length - 1) // there is an empty slot in front of currentTexture, move forward to the empty spot
        {
            Texture2D textureCopy = new Texture2D(width: (int)textureSize.x, height: (int)textureSize.y);
            textureCopy.LoadRawTextureData(textures[currentIndex].GetRawTextureData());
            textures[currentIndex + 1] = textureCopy;
            sprites[currentIndex + 1] = sprites[currentIndex];
            currentIndex++;
            whiteboard.sprite = sprites[currentIndex]; // set texture
            Debug.Log("moved one texture forward. current texture: " + currentIndex);
        }
        else // there are no empty slots ahead of currentTexture (we're on the last texture), move everything one slot backward to free up the last texture 
        {
            for(int i = 1; i  < textures.Length; i++)
            {
                textures[i - 1] = textures[i];
            }
            Debug.Log("shifted all textures a step back. current texture: " + currentIndex);
        }
    }

    public void undoTexture()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            whiteboard.sprite = sprites[currentIndex];
        }
        Debug.Log("current texture: " + currentIndex);
    }

    public void redoTexture()
    {
        if (currentIndex < textures.Length - 1)
        {
            currentIndex++;
            whiteboard.sprite = sprites[currentIndex];
        }
        Debug.Log("current texture: " + currentIndex);
    }
}
