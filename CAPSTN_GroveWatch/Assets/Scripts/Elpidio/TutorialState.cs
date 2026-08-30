public static class TutorialState
{
    public static bool ShouldShowTutorial;
    public static bool Consume()
    {
        bool requested = ShouldShowTutorial;
        ShouldShowTutorial = false;
        return requested;
    }

    public static void Clear()
    {
        ShouldShowTutorial = false;
    }
}
