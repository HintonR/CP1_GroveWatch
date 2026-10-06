using System.Collections.Generic;
public static class TutorialState
{
    public static bool ShouldShowTutorial;
    static readonly HashSet<CutsceneData> shown = new HashSet<CutsceneData>(); //for tutorials that have been already shown
    public static bool FirstTime(CutsceneData tutorial) { return shown.Add(tutorial); } //true only once per run

    //public static bool Consume()
    //{
    //    bool requested = ShouldShowTutorial;
    //    ShouldShowTutorial = false;
    //    return requested;
    //}

    public static void Clear()
    {
        ShouldShowTutorial = false;
        shown.Clear();
    }
}
