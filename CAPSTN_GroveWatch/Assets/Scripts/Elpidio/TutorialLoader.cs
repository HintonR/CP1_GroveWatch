using UnityEngine;

public static class TutorialLoader
{
    public const string PrefabPath = "Tutorial/TutorialOverlay";

    static GameObject instance;

    public static bool IsOpen => instance != null;

    public static void Show(CutsceneData data)
    {
        var player = IsOpen ? instance.GetComponentInChildren<TutorialPlayer>(true) : Spawn();
        
        player.onFinished.RemoveListener(Close);
        player.onFinished.AddListener(Close);
        player.Show(data);
    }

    static TutorialPlayer Spawn()
    {
        var prefab = Resources.Load<GameObject>(PrefabPath);


        if (prefab == null)
        {
            Debug.Log("Prefab is null award");
            return null;
        }

        instance = Object.Instantiate(prefab);
        instance.name = prefab.name;

        var player = instance.GetComponentInChildren<TutorialPlayer>(true);
        return player;
    }

    public static void Close()
    {
        Object.Destroy(instance);
        instance = null;
    }
}
