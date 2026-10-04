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

        if(_currentScene.name == "TutorialLevel")
            _sH._aM.PlayMusic(Music.Gameplay);

        if(_currentScene.name == "Overworld")
            _sH._aM.PlayMusic(Music.Overworld);
        
        if(_currentScene.name == "Cebu")
            _sH._aM.PlayMusic(Music.Gameplay);
        
        if(_currentScene.name == "Bagiuo")
            _sH._aM.PlayMusic(Music.Gameplay);

        if(_currentScene.name == "Palawan")
            _sH._aM.PlayMusic(Music.Gameplay);    
    }
}
