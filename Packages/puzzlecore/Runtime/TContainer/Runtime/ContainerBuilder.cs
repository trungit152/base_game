using System;
using System.Collections.Generic;

namespace trungnhd.puzzlecore.TContainer
{
    public sealed class ContainerBuilder : IContainerBuilder
    {
        readonly List<Registration> _registrations = new List<Registration>();
        bool _built;
        public void Add(Registration r)
        {
            if(_built) throw new InvalidOperationException("Container already built");
            if (r == null) throw new ArgumentNullException(nameof(r));
            _registrations.Add(r);
        }

        public IObjectResolver Build()
        {
            if (_built) throw new InvalidOperationException("ContainerBuilder can be built 1 time");
            _built = true;
            return new Container(_registrations);
        }
    }
}
