using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class whiteboard_testscript
{

    // A Test behaves as an ordinary method
    [Test]
    public void whiteboard_testscriptSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator whiteboard_testscriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
    [UnityTest]
    public IEnumerator WhiteboardCreated()
    {
        GameObject whiteboard_temp = new GameObject();
        Whiteboard mywhiteboard = whiteboard_temp.AddComponent<Whiteboard>();
        Assert.IsNotNull(mywhiteboard.sprite);
        yield return null;
    }

    [Test]
    public void unclickedByDefault()
    {
        GameObject whiteboard_temp = new GameObject();
        Whiteboard mywhiteboard = whiteboard_temp.AddComponent<Whiteboard>();

        Assert.AreEqual(mywhiteboard.textures[0].imageContentsHash, mywhiteboard.textures[1].imageContentsHash);
    }

    /*
     * create assembly reference (the first one)
     * link to dll file (make easier by locking file)
     * HIT APPLY
     * make gameobject to reference
     */
}
