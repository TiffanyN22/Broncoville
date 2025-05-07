using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPanelColliderHandler : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    private void OnTriggerEnter2D(Collider2D other){
        if (other.CompareTag("Player")) // Make sure the player has the "Player" tag
        {
            if (!panel.activeSelf) panel.SetActive(true);
        }
    } 
}