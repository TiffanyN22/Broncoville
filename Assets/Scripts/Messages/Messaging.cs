using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    GameObject newText = Instantiate(textItemPrefab, messagesScrollViewContent);
    newText.GetComponentInChildren<TMP_Text>().text = $"[{username}]: {message}";
    inputField.textComponent.color = Color.white;
    inputField.text = "";
  }

  public void UpdateMessageDisplay(bool display)
  {
    messageDisplayGroup.SetActive(display);
    noMessageDisplayGroup.SetActive(!display);
  }
  
  void Update()
  {
  }
}
