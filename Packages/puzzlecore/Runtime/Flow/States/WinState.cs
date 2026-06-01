using System;

namespace trungnhd.puzzlecore.Flow.States
{
    /// <summary>Thắng màn. Cho phép quay về <see cref="MainMenuState"/> (màn tiếp theo / thoát).</summary>
    public class WinState : GameStateBase
    {
        public override bool CanTransitionTo(Type nextStateType)
            => nextStateType == typeof(MainMenuState);
    }
}
