// COMMENTED OUT BECAUSE REMOVED STUDY ROOM SELECTION ASSEMBLY FILE
/* 
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
public class JoinStudyRoomTestScript
{
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
}
*/