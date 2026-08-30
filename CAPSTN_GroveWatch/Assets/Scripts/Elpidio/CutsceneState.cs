public static class CutsceneState
{
    public static CutsceneData SelectedCutscene;

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