using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using System.Linq;

public class JoinStudyRoomTestScript
{
    [UnityTest]
    public IEnumerator TestCreateRoomNoRoomName()
    {
        // load scene and wait for it to finish
        SceneManager.LoadScene("StudyRoomSelectScene");
        yield return new WaitForSeconds(0.1f);

        // find StudyRoomSelection in the scene
        var selection = GameObject.FindObjectOfType<StudyRoomSelection>();
        Assert.IsNotNull(selection, "StudyRoomSelection not found in scene.");

        // test createRoom doesn't change the list
        List<OpenRoom> originalRooms = new List<OpenRoom>(selection.getAllRooms());
        selection.createRoom();
        Assert.AreEqual(originalRooms, selection.getAllRooms(), "Room list changed unexpectedly.");
    }

    [UnityTest]
    public IEnumerator TestCreateRoomHasRoomName()
    {
        // load scene and wait for it to finish
        SceneManager.LoadScene("StudyRoomSelectScene");
        yield return new WaitForSeconds(0.1f); 

        // find StudyRoomSelection in the scene
        var selection = GameObject.FindObjectOfType<StudyRoomSelection>();
        Assert.IsNotNull(selection, "StudyRoomSelection not found in scene.");

        // add new room to input field
        string newRoomName = "TestRoom";
        selection.setCreateRoomNameInput(newRoomName);

        // test new element as added createRoom doesn't change the list
        List<OpenRoom> originalRooms = new List<OpenRoom>(selection.getAllRooms());
        selection.createRoom();
        Assert.AreNotEqual(originalRooms, selection.getAllRooms(), "Room list didn't changed.");
        Assert.AreEqual(originalRooms.Count + 1, selection.getAllRooms().Count, "There is not exactly 1 more room");
        OpenRoom newRoom = selection.getAllRooms().Last();
        Assert.AreEqual(newRoomName, newRoom.roomName, "The added room has the wrong room nate");
    }

    [UnityTest]
    public IEnumerator TestCreateRoomFirstRoom()
    {
        // load scene and wait for it to finish
        SceneManager.LoadScene("StudyRoomSelectScene");
        yield return new WaitForSeconds(0.1f); 

        // find StudyRoomSelection in the scene
        var selection = GameObject.FindObjectOfType<StudyRoomSelection>();
        Assert.IsNotNull(selection, "StudyRoomSelection not found in scene.");

        // delete existing rooms
        selection.clearAllRoom();
        Assert.AreEqual(0, selection.getAllRooms().Count, "Did not clear study rooms correctly");

        // add new room to input field
        string newRoomName = "TestRoom";
        selection.setCreateRoomNameInput(newRoomName);

        // test new element as added createRoom doesn't change the list
        List<OpenRoom> originalRooms = new List<OpenRoom>(selection.getAllRooms());
        selection.createRoom();
        Assert.AreNotEqual(originalRooms, selection.getAllRooms(), "Room list didn't changed.");
        Assert.AreEqual(originalRooms.Count + 1, selection.getAllRooms().Count, "There is not exactly 1 more room");
        OpenRoom newRoom = selection.getAllRooms().Last();
        Assert.AreEqual(newRoomName, newRoom.roomName, "The added room has the wrong room name");
        Assert.AreEqual(0, newRoom.roomID, "The added room's room ID is not 0");
    }
}
