using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResetButton : MonoBehaviour
{
    public Button restartButton;
    private void OnEnable()
    {
        restartButton.onClick.AddListener(() => ButtonPress(restartButton));
        
    }
    private void ButtonPress(Button buttonPressed)
    {
        if (buttonPressed == restartButton)
        {

            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }
    }
}
