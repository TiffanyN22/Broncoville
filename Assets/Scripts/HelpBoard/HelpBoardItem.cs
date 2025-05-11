using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HelpBoardItem : MonoBehaviour
{

    [SerializeField] private TMP_Text itemTopic; 
    [SerializeField] private Button learnMoreButton;
    [SerializeField] private GameObject helpDetails;
    [SerializeField] private GameObject helpList;
    public HelpDetailsInfo helpDetailsInfo;

    void Start(){
      learnMoreButton.onClick.AddListener(clickedMore);
    }

    public void SetTopic(string topic){
      itemTopic.text = topic;
    }

    public void SetHelpDetails(GameObject details){
      helpDetails = details;
    }
    public void SetHelpList (GameObject list){
      helpList = list;
    }

    public void clickedMore(){
      // GameObject helpDetails = helpDetailsTransform.gameObject;
      if (!helpDetails.activeSelf) helpDetails.SetActive(true);
      if (helpList.activeSelf) helpList.SetActive(false);
      helpDetails.GetComponent<HelpDetailsDisplay>().updateInfo(helpDetailsInfo.topic, helpDetailsInfo.requester, helpDetailsInfo.description);
    }
}

[System.Serializable] 
public class HelpDetailsInfo {
    public string topic;
    public string requester;
    public string description;

    public HelpDetailsInfo(string topic, string requester, string description)
    {
        this.topic = topic;
        this.requester = requester;
        this.description = description;
    }
}