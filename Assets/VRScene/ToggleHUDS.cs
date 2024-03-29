using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleHUDS : MonoBehaviour
{
    public GameObject OtherHud, HUD; 
    public void ToggleHUD()
    {
        OtherHud.SetActive(true);
        HUD.SetActive(false);
    }
}
