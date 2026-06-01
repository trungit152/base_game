using System;

namespace trungnhd.puzzlecore.Flow.States
{
    /// <summary>
    /// Đang tạm dừng. Guard các transition: chỉ cho phép quay lại <see cref="PlayingState"/> hoặc
    /// thoát về <see cref="MainMenuState"/> (vd không thể nhảy thẳng sang Win/Lose).
    /// </summary>
    public class PausedState : GameStateBase
    {
        public override bool CanTransitionTo(Type nextStateType)
            => nextStateType == typeof(PlayingState) || nextStateType == typeof(MainMenuState);
    }
}
