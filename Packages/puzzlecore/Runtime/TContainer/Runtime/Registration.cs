using System;

namespace trungnhd.puzzlecore.TContainer
{
    public sealed class Registration
    {
        public Type ServiceType;
        public Type ImplementationType;
        public Func<IObjectResolver, object> Factory;
        public object Instance;
        public Lifetime Lifetime;
        public bool IsEntryPoint;
    }
}