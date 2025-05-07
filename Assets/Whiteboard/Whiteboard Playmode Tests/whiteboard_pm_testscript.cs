using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

public class whiteboard_pm_testscript
{

    [UnityTest]
    public IEnumerator penDraws()
    {
        //GameObject whiteboard_temp = new GameObject();
        //Whiteboard mywhiteboard = whiteboard_temp.AddComponent<Whiteboard>();
        Whiteboard mywhiteboard = AssetDatabase.LoadAssetAtPath<Whiteboard>("Assets/Whiteboard/Whiteboard.prefab");
        // Colors mypen = AssetDatabase.LoadAssetAtPath<Colors>("Assets/Whiteboard/PenManager.prefab");
        // mywhiteboard.Start();
        Debug.Log(mywhiteboard.textures[0]);
        //Debug.Log(mypen);
        Assert.NotNull(mywhiteboard);
        yield return null;
    }

    [UnityTest]
    public IEnumerator undoRedoTextureMatch()
    {
        Whiteboard mywhiteboard = AssetDatabase.LoadAssetAtPath<Whiteboard>("Assets/Whiteboard/Whiteboard.prefab");
        Debug.Log(mywhiteboard.textures.Length);
        Debug.Log(mywhiteboard.rangeUndoRedo);
        Assert.AreEqual(10, mywhiteboard.rangeUndoRedo);
        yield return null;
    }

    [UnityTest]
    public IEnumerator spriteTextureFill()
    {

        Whiteboard mywhiteboard = AssetDatabase.LoadAssetAtPath<Whiteboard>("Assets/Whiteboard/Whiteboard.prefab");

        Assert.NotNull(mywhiteboard.textures);
        for(int i = 0; i < mywhiteboard.textures.Length; i++)
        {
            Assert.IsNull(mywhiteboard.textures[i]);
        }
        yield return null;
    }

    [UnityTest]
    public IEnumerator unclickedByDefault()
    {
        Whiteboard mywhiteboard = AssetDatabase.LoadAssetAtPath<Whiteboard>("Assets/Whiteboard/Whiteboard.prefab");

        Assert.AreEqual(mywhiteboard.lineMode, false);
        Assert.AreEqual(mywhiteboard.mouseLeftClick, false);
        Assert.AreEqual(mywhiteboard.whiteboardHover, false);
        yield return null;
    }

    [UnityTest]
    public IEnumerator penSet()
    {
        Whiteboard mywhiteboard = AssetDatabase.LoadAssetAtPath<Whiteboard>("Assets/Whiteboard/Whiteboard.prefab");
        Colors mypen = AssetDatabase.LoadAssetAtPath<Colors>("Assets/Whiteboard/PenManager.prefab");

        Assert.AreEqual(mywhiteboard.penSize, 0);
        Assert.AreEqual(mywhiteboard.rangeUndoRedo, 10);
        Assert.AreEqual(mypen.myColor, new Color(1.000f, 0.000f, 0.000f, 1.000f)); // default black

        yield return null;
    }

}
