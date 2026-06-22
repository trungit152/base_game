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
        
        public void MoveAll(SwipeDirection direction)
        {
            Vector2Int delta = ToDelta(direction);
            if (delta == Vector2Int.zero)
                return;

            foreach (var actor in _actors)
                Slide(actor, delta);
        }

        private void Slide(Actor actor, Vector2Int delta)
        {
            Vector2Int destination = actor.Coordinate;
            while (IsWalkable(destination + delta))
                destination += delta;

            if (destination == actor.Coordinate)
                return;

            actor.MoveTo(destination);
            Moved?.Invoke(actor);
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
