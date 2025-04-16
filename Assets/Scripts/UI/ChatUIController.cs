using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ChatUIController : NetworkBehaviour
{
	[Tooltip("The prefab for chat messages.")]
	[SerializeField] private TextMeshProUGUI chatMessagePrefab = null;

	[Tooltip("The maximum amount of chat messages that can be shown before older ones get deleted.")]
	[SerializeField] private int maxChatHistory = 100;

	[Tooltip("Chat messages will be added as children to this object.")]
	[SerializeField] private VerticalLayoutGroup chat = null;

	[Tooltip("The text field where chat messages are sent from.")]
	[SerializeField] private TMP_InputField messageInput = null;

	public void Start()
	{
		this.messageInput.onSubmit.AddListener(this.SubmitMessage);
	}

	public void SubmitMessage(string message)
	{
		this.SendMessageRpc(message);
		this.messageInput.text = "";
		this.messageInput.ActivateInputField();
	}

	[Rpc(SendTo.ClientsAndHost)]
	public void SendMessageRpc(string message)
	{
		TextMeshProUGUI chatMessage = Instantiate(this.chatMessagePrefab, this.chat.transform);
		chatMessage.text = message;

		while(chat.transform.childCount > maxChatHistory)
		{
			Destroy(this.chat.transform.GetChild(0).gameObject);
			break;
		}
	}
}
