using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizePony : MonoBehaviour
{
    // Color Change Variables
    public Slider colorSlider;
    public Slider saturationSlider;
    public Image targetSprite;
    public Image hueBackgroundSprite;

    // Hair Change Variables
    public Image displayedHairImage;
    public Sprite hair1Sprite;
    public Sprite hair2Sprite;

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
        displayedHairImage.sprite = hair1Sprite;
    }

    public void SelectHair2(){
        if (displayedHairImage == null || hair2Sprite == null) return;
        displayedHairImage.sprite = hair2Sprite;
    }

    public void ClosePopup(){
        pony.color = targetSprite.color;
        hair.sprite = displayedHairImage.sprite;
        gameObject.SetActive(false);
    }
}
