using System.Collections;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using Unity.NetCode;
using Unity.Collections;

public class CustomizePony : MonoBehaviour
{
    // Color Change Variables
    public Slider colorSlider;
    public Slider saturationSlider;
    public RawImage targetSprite;
    public RawImage hueBackgroundSprite;

    // Hair Change Variables
    public RawImage displayedHairImage;
    public Texture hair1Sprite;
    public Texture hair2Sprite;

    // Actual Pony
    public SpriteRenderer pony;
    public SpriteRenderer hair;

    private void Start()
    {
        colorSlider.onValueChanged.AddListener((value) => UpdateSpriteHue());
        saturationSlider.onValueChanged.AddListener((value) => UpdateSprite());
    }

    private void UpdateSpriteHue()
    {
        if (targetSprite == null) return;

        // update hue slider color
        Color newColor = Color.HSVToRGB(colorSlider.value, 1, 1);
        hueBackgroundSprite.color = newColor;

        // update pony sprite
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (targetSprite == null) return;

        Color newSpriteColor = Color.HSVToRGB(colorSlider.value, saturationSlider.value, 1);
        targetSprite.color = newSpriteColor;
    }

    public void SelectHair1(){
        if (displayedHairImage == null || hair1Sprite == null) return;
        displayedHairImage.texture = hair1Sprite;
    }

    public void SelectHair2(){
        if (displayedHairImage == null || hair2Sprite == null) return;
        displayedHairImage.texture = hair2Sprite;
    }

    public void ClosePopup(){
         // Send new pony to server
        // EntityManager clientManager = FindFirstObjectByType<ClientManager>().GetEntityManager();
        // Entity sendMessageEntity = clientManager.CreateEntity(typeof(CustomizePonyRpc), typeof(SendRpcCommandRequest));
        // clientManager.SetComponentData(sendMessageEntity, new CustomizePonyRpc { bodyColor = targetSprite.color, hairStyle = displayedHairImage.texture == hair1Sprite ? HairStyle.WAVY : HairStyle.STRAIGHT });


        // pony.color = targetSprite.color;
        // hair.sprite = displayedHairImage.sprite;

        // SEND TO SERVER
        gameObject.SetActive(false);
    }
}
