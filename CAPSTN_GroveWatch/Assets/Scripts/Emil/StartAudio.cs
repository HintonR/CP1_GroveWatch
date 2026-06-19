using UnityEngine;
using UnityEngine.SceneManagement;

public class StartAudio : MonoBehaviour
{
    ServiceHub _sH;

    void Awake()
    {
        _sH = ServiceHub.Instance;
    }
    void Start()
    {
        Scene _currentScene = SceneManager.GetActiveScene();           
        if(_currentScene.name == "TitleScreen")
            _sH._aM.PlayMusic(Music.Title);

        if(_currentScene.name == "Main")
            _sH._aM.PlayMusic(Music.Gameplay);
    }
}
