using System;
using System.Collections.Generic;
using CatchHim.Gameplay.Grid;
using trungnhd.puzzlecore.input;
using UnityEngine;

namespace CatchHim.Gameplay.Actors
{
    public class ActorManager
    {
        private readonly GridManager _grid;
        private readonly List<Actor> _actors = new();

        public IReadOnlyList<Actor> Actors => _actors;

        public event Action<Actor> Spawned;
        public event Action<Actor> Moved;

        public ActorManager(GridManager grid)
        {
            _grid = grid;
        }

        public Actor Spawn(ActorType type, Vector2Int coordinate)
        {
            var actor = new Actor(type, coordinate);
            _actors.Add(actor);
            Spawned?.Invoke(actor);
            return actor;
        }

        public Actor GetActorAt(Vector2Int coordinate)
        {
            foreach (var actor in _actors)
            {
                if (actor.Coordinate == coordinate)
                    return actor;
            }

            return null;
        }

        public Actor GetActor(ActorType type)
        {
            foreach (var actor in _actors)
            {
                if (actor.Type == type)
                    return actor;
            }

            return null;
        }

        public bool TryMove(Actor actor, SwipeDirection direction)
        {
            return TryMoveTo(actor, actor.Coordinate + ToDelta(direction));
        }

        public bool TryMoveTo(Actor actor, Vector2Int target)
        {
            if (!IsWalkable(target))
                return false;

            actor.MoveTo(target);
            Moved?.Invoke(actor);
            return true;
        }

        private bool IsWalkable(Vector2Int coordinate)
        {
            if (coordinate.x < 0 || coordinate.x >= _grid.X ||
                coordinate.y < 0 || coordinate.y >= _grid.Y)
                return false;

            return _grid.Cells[coordinate.x, coordinate.y].State.CanGoThrough();
        }

        private static Vector2Int ToDelta(SwipeDirection direction)
        {
            return direction switch
            {
                SwipeDirection.Up => Vector2Int.up,
                SwipeDirection.Down => Vector2Int.down,
                SwipeDirection.Left => Vector2Int.left,
                SwipeDirection.Right => Vector2Int.right,
                _ => Vector2Int.zero,
            };
        }
    }
}
