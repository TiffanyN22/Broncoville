using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class Colors : MonoBehaviour
{
    [SerializeField] public Color myColor;
    [SerializeField] private Whiteboard whiteboard_script;
    public Vector2 pen_dims;
    public Color[] myColorArray;

    // Start is called before the first frame update
    void Start()
    {
        myColor = Color.black;
        whiteboard_script.penSize = 10; // penSize is 0 for some reason
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
    }

    // there's definitely a better way to do this
    public void set_black()
    {
        myColor = Color.black;
        set_pen();
    }
    public void set_white()
    {
        myColor = Color.white;
        set_pen();
    }
    public void set_red()
    {
        myColor = Color.red;
        set_pen();
    }
    public void set_orange()
    {
        myColor = new Color(1, 0.64f, 0, 1);
        set_pen();
    }
    public void set_yellow()
    {
        myColor = Color.yellow;
        set_pen();
    }
    public void set_green()
    {
        myColor = Color.green;
        set_pen();
    }
    public void set_blue()
    {
        myColor = Color.blue;
        set_pen();
    }
    public void set_purple()
    {
        myColor = Color.magenta;
        set_pen();
    }
    public void set_eraser()
    {
        // change adaptive to bg later
        myColor = Color.white;
        set_pen();
    }
}
