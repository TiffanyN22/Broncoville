using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using Unity.Entities;
using Unity.NetCode;
using Unity.Collections;
public class Messaging : MonoBehaviour
{
  string username = "Tiffany"; // TODO: get username
  [SerializeField] private TMP_InputField inputField;
  [SerializeField] private  GameObject textItemPrefab;
  [SerializeField] private  GameObject messageDisplayGroup;
  [SerializeField] private  GameObject noMessageDisplayGroup;
  [SerializeField] private  Transform messagesScrollViewContent;

  void Start()
  {
    inputField.onSubmit.AddListener(SendMessage);
  }

  void SendMessage(string message)
  {
    if (message.Length > 125)
    {
      inputField.textComponent.color = Color.red;
      return;
    }
    string fullMessage = $"[{username}]: {message}";

    // Send message to server
     EntityManager clientManager = FindFirstObjectByType<ClientManager>().GetEntityManager();
    Entity sendMessageEntity = clientManager.CreateEntity(typeof(MessagingSendMessageRpc), typeof(SendRpcCommandRequest));
    clientManager.SetComponentData(sendMessageEntity, new MessagingSendMessageRpc { message = fullMessage });


    inputField.textComponent.color = Color.white;
    inputField.text = "";
  }

  public void UpdateMessageDisplay(bool display)
  {
    messageDisplayGroup.SetActive(display);
    noMessageDisplayGroup.SetActive(!display);
  }

  public void AddMessage(string message)
  {
    // Debug.Log("Adding message " + message);
    GameObject newText = Instantiate(textItemPrefab, messagesScrollViewContent);
    newText.GetComponentInChildren<TMP_Text>().text = message;
  }
}
