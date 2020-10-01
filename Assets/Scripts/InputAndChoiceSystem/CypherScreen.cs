using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CypherScreen : MonoBehaviour
{
    private bool showingKey = false;
    public bool isShowingKey { get { return showingKey; } }
    public static CypherScreen instance;
    public string keyImage = "key0";
    public bool blockCypherScreen = false;
    public void Awake()
    {
        instance = this;
        blockCypherScreen = false;
    }
    public void ShowKey()
    {
        if (blockCypherScreen) return;
        AudioClip key_look_sound = Resources.Load("Audio/SFX/effect_key_look") as AudioClip;
        AudioManager.instance.PlaySFX(key_look_sound);
        showingKey ^= true;
        if (showingKey)
        {
            NovelController.instance.blockNext = true;
            AudioManager.instance.PlaySFX(key_look_sound,0.25f);
            NovelController.instance.Command_SetLayerImage(keyImage, BCFC.instance.cypherframe);
        }
        else
        {
            NovelController.instance.blockNext = false;
            NovelController.instance.Command_SetLayerImage("null", BCFC.instance.cypherframe);
        }

    }
}
