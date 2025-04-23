using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PublicRoomSelectButton : MonoBehaviour
{

    [SerializeField] private TMP_Text roomName; 
    [SerializeField] private int roomId;

    public void displayRoomName(string name){
      roomName.text = name;
    }

    public void setRoomId(int newId){
      roomId = newId;
    }

    public void clickedButton(){
      // TODO: change scene
      Debug.Log($"Going to {roomId}");
    }
}
