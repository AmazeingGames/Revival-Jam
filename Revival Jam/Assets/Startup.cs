using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Startup : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] SceneLoader sceneLoader;
    [SerializeField] string sceneName;
    private void OnEnable()
    {
        sceneLoader.LoadScene(sceneName, false);
    }

    private void Awake()
    {
        sceneLoader.LoadScene(sceneName, false);
    }
}
