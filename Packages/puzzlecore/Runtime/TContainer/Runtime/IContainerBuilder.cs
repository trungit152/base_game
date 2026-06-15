using System;
using System.Collections.Generic;

namespace trungnhd.puzzlecore.TContainer
{
    public interface IContainerBuilder
    {
        void Add(Registration r);
        IObjectResolver Build();
    }
}