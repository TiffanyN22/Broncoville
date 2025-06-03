using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
public class StudyRoomEntryList : MonoBehaviour
{
  [SerializeField] private List<OpenRoom> allRooms = new List<OpenRoom>();

  void Start()
  {
    for (int i = 0; i < allRooms.Count; ++i)
    {
      if (allRooms[i].roomGuid == Guid.Empty)
      {
        allRooms[i].roomGuid = Guid.NewGuid();
      }
    }
  }

  public List<OpenRoom> getAllRooms()
  {
    return allRooms;
  }

  public OpenRoom getRoomByGuid(Guid guid)
  {
    return allRooms.Find(curRoom => curRoom.roomGuid == guid);
  }

  public void addRoom(OpenRoom newRoom)
  {
    if (newRoom.roomGuid == Guid.Empty)
    {
      newRoom.roomGuid = Guid.NewGuid();
    }
    allRooms.Add(newRoom);
  }
  public void deleteRoom(Guid guid)
  {
    OpenRoom deletedRoom = allRooms.Find(curRoom => curRoom.roomGuid == guid);
    allRooms.Remove(deletedRoom);
  }
}