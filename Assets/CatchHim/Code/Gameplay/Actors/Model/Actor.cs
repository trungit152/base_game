using UnityEngine;

namespace CatchHim.Gameplay.Actors
{
    public class Actor
    {
        public ActorType Type { get; }
        public Vector2Int Coordinate { get; private set; }

        public Actor(ActorType type, Vector2Int coordinate)
        {
            Type = type;
            Coordinate = coordinate;
        }

        public void MoveTo(Vector2Int coordinate)
        {
            Coordinate = coordinate;
        }
    }
}
