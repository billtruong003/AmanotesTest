using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackBtn : MonoBehaviour
{
    [SerializeField] private string sceneBack;
    public void LoadSceneBack()
    {
        SceneManager.LoadScene(sceneBack);
    }
}
