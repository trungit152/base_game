namespace trungnhd.puzzlecore.input
{
    /// <summary>Pha của một mẫu con trỏ trong dòng pointer (backend-agnostic).</summary>
    public enum PointerPhase
    {
        Began,
        Moved,
        Ended,
        Canceled
    }
}
