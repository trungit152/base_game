using System;

namespace trungnhd.puzzlecore.Flow.States
{
    /// <summary>Thua màn. Cho phép quay về <see cref="MainMenuState"/> (chơi lại / thoát).</summary>
    public class LoseState : GameStateBase
    {
        public override bool CanTransitionTo(Type nextStateType)
            => nextStateType == typeof(MainMenuState);
    }
}
