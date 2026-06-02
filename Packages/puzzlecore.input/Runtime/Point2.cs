namespace trungnhd.puzzlecore.input
{
    /// <summary>Điểm 2D screen-space thuần (pixel). Payload cho các gesture (tap, drag, hold).</summary>
    public readonly struct Point2
    {
        public readonly float X;
        public readonly float Y;

        public Point2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => "(" + X + ", " + Y + ")";
    }
}
