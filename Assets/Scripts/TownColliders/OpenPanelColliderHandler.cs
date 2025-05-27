using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OpenPanelColliderHandler : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    private bool userInCollider = false;
    void Start()
    {
        panel.SetActive(true);
        Debug.Log("starting");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("user pressed e!");
            if (!panel.activeSelf)
                panel.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Testing");
        // if (other.CompareTag("Player"))
        // {
        // if (!panel.activeSelf)
        //     panel.SetActive(true);
        // }
        // TODO: change sprite
        userInCollider = true;
    }

    private void OnTriggerExit(Collider other)
    {
        userInCollider = false;
    }
}
