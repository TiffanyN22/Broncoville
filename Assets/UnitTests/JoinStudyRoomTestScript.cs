using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
public class JoinStudyRoomTestScript
{
    // A Test behaves as an ordinary method
    [Test]
    public void JoinStudyRoomTestScriptSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    [Test]
    public void TestSetRoomCreationTypePublicTrue()
    {
        GameObject selectionObject = new GameObject();
        StudyRoomSelection selection = selectionObject.AddComponent<StudyRoomSelection>();
        selection.setRoomCreationTypePublic(true); 
        Assert.AreEqual(selection.getCreateRoomPrivate(), true);
    }

    [Test]
    public void TestSetRoomCreationTypePublicFalse()
    {
        GameObject selectionObject = new GameObject();
        StudyRoomSelection selection = selectionObject.AddComponent<StudyRoomSelection>();
        selection.setRoomCreationTypePublic(false); 
        Assert.AreEqual(selection.getCreateRoomPrivate(), false);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator JoinStudyRoomTestScriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
