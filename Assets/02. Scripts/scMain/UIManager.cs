using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void OnClickStartButton()
    {
        SceneManager.LoadScene("scGame_Stage");
        SceneManager.LoadScene("scGame_Play", LoadSceneMode.Additive);
    }
}
