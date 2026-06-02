namespace trungnhd.puzzlecore.input
{
    /// <summary>Tham số nhận diện swipe. Khoảng cách tính theo pixel screen-space.</summary>
    public readonly struct SwipeConfig
    {
        /// <summary>
        /// Quãng đường tối thiểu (pixel) để tính là một cú vuốt; dưới ngưỡng coi như tap và bỏ qua.
        /// </summary>
        public readonly float MinSwipeDistance;

        public SwipeConfig(float minSwipeDistance)
        {
            MinSwipeDistance = minSwipeDistance;
        }

        public static SwipeConfig Default => new SwipeConfig(50f);
    }
}
