public static class CutsceneState
{
    public static CutsceneData SelectedCutscene;
    public static string PendingLevel; //level the Overworld is sending us to (set it by PlayLevel)
    public static string LastLevel;    //level that just ended, so we can find its win cutscene

    public static CutsceneData Consume() //returns SelectedCutscene and clears it for next run
    {
        var selected = SelectedCutscene;
        SelectedCutscene = null;
        return selected;
    }

    public static void Clear()
    {
        SelectedCutscene = null;
    }
}