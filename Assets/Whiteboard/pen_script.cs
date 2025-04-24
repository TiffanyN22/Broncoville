using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class Colors : MonoBehaviour
{
    [SerializeField] public Color32 myColor;
    [SerializeField] private Whiteboard whiteboard_script;
    [SerializeField] public Vector2 pen_dims = new Vector2(10,  10);
    public Color[] myColorArray;

    // Start is called before the first frame update
    void Start()
    {
        myColor = Color.black;
        set_pen();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void set_pen()
    {
        myColorArray = new Color[(int)pen_dims.x * (int)pen_dims.y];
        for (int i = 0; i < myColorArray.Length; ++i)
        {
            myColorArray[i] = myColor;
        }
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
        myColor = new Color(1f, 0.5f, 0f, 0f);
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
        myColor = new Color(1f, 0f, 1f, 0f);
        set_pen();
    }
    public void set_eraser()
    {
        // change adaptive to bg later
        myColor = Color.white;
        set_pen();
    }
}
