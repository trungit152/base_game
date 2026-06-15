using trungnhd.puzzlecore.Common;

namespace trungnhd.puzzlecore.Boosters
{
    public interface IBoosterEffect
    {
        Result Apply(BoosterActivationContext context);
    }
}
