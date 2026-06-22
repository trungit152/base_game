using CatchHim.Gameplay.Actors;
using UnityEngine;
using VContainer.Unity;

namespace CatchHim.Gameplay.Grid
{
    public sealed class GridController : IStartable
    {
        private readonly GridManager _grid;
        private readonly GridViewManager _view;
        private readonly ActorManager _actors;
        private readonly ActorViewManager _actorView;

        public GridController(GridManager grid, GridViewManager view, ActorManager actors, ActorViewManager actorView)
        {
            _grid = grid;
            _view = view;
            _actors = actors;
            _actorView = actorView;
        }

        public void Start()
        {
            _grid.CreateGridMap();
            _view.Build(_grid);

            _actorView.Bind(_grid, _actors);

            _actors.Spawn(ActorType.Thief, new Vector2Int(0, 0));
            _actors.Spawn(ActorType.Police, new Vector2Int(_grid.X - 1, _grid.Y - 1));
        }
    }
}