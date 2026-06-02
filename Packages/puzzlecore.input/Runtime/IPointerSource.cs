using System;

namespace trungnhd.puzzlecore.input
{
    public interface IPointerSource
    {
        event Action<PointerSample> Pointer;
    }
}
