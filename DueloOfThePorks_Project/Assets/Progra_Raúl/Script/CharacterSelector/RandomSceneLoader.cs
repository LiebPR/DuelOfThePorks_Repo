using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomSceneLoader : MonoBehaviour
{
    [SerializeField] private int[] sceneIndices;

    public void LoadRandomScene()
    {
        if (sceneIndices.Length == 0) return;

        int randomIndex = Random.Range(0, sceneIndices.Length);
        int sceneToLoad =  sceneIndices[randomIndex];

        SceneManager.LoadScene(sceneToLoad);
    }
}
