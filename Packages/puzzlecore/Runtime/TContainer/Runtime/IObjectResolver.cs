using System;

namespace trungnhd.puzzlecore.TContainer
{
    public interface IObjectResolver
    {
        T Resolve<T>();
        object Resolve(Type type);
    }
}