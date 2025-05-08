using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class Colors : MonoBehaviour
{
    [SerializeField] public Color myColor;
    [SerializeField] private Whiteboard whiteboard_script;
    [SerializeField] private penbrush_script mypenbrush_script;
    [SerializeField] private linemaker_script mylinemaker_script;
    public Vector2 pen_dims;
    public Color[] myColorArray;

    // Start is called before the first frame update
    void Start()
    {
        myColor = Color.black;
        whiteboard_script.penSize = 10; // penSize is 0 for some reason. set here as well
        pen_dims = new Vector2(whiteboard_script.penSize, whiteboard_script.penSize);
        set_pen();
    }

    // Update is called once per frame
    void Update()
    {
        if (myColor != myColorArray[0] || whiteboard_script.penSize != pen_dims.x || whiteboard_script.penSize != pen_dims.y)
        {
            set_pen();
        }
    }

    public void set_pen()
    {
        pen_dims = new Vector2(whiteboard_script.penSize, whiteboard_script.penSize);
        myColorArray = new Color[(int)pen_dims.x * (int)pen_dims.y];
        for (int i = 0; i < myColorArray.Length; ++i)
        {
            myColorArray[i] = myColor;
        }
        whiteboard_script.mousePositionOffset = new Vector2(whiteboard_script.penSize / 2, whiteboard_script.penSize / 2);
        mylinemaker_script.setLineMakerColor(myColor);
    }

    // there's definitely a better way to do this
    public void set_black()
    {
        myColor = Color.black;
    }
    public void set_white()
    {
        myColor = Color.white;
    }
    public void set_red()
    {
        myColor = Color.red;
    }
    public void set_orange()
    {
        myColor = new Color(1, 0.64f, 0, 1);
    }
    public void set_yellow()
    {
        myColor = Color.yellow;
    }
    public void set_green()
    {
        myColor = Color.green;
    }
    public void set_blue()
    {
        myColor = Color.blue;
    }
    public void set_purple()
    {
        myColor = Color.magenta;
    }
    public void set_eraser()
    {
        // change adaptive to bg later
        myColor = Color.white;
    }
    public void set_small()
    {
        whiteboard_script.penSize = 5;
        mypenbrush_script.penBrushSizeAdjust();
    }
    public void set_med()
    {
        whiteboard_script.penSize = 10;
        mypenbrush_script.penBrushSizeAdjust();
    }
    public void set_lg()
    {
        whiteboard_script.penSize = 15;
        mypenbrush_script.penBrushSizeAdjust();
    }

    public void set_lineMode()
    {
        whiteboard_script.lineMode = true;
        whiteboard_script.penMode = false;
    }

    public void set_penMode()
    {
        whiteboard_script.lineMode = false;
        whiteboard_script.penMode = true;
    }
}
