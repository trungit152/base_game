using System.Collections.Generic;
using CatchHim.Gameplay.Grid;
using UnityEngine;

namespace CatchHim.Gameplay.Actors
{
    public class ActorViewManager : MonoBehaviour
    {
        [SerializeField] private ActorView actorViewPrefab;
        [SerializeField] private ActorVisualConfig visualConfig;
        [SerializeField] private RectTransform actorRoot;

        private GridManager _grid;
        private ActorManager _actors;
        private readonly Dictionary<Actor, ActorView> _views = new();

        public void Bind(GridManager grid, ActorManager actors)
        {
            _grid = grid;
            _actors = actors;

            foreach (var actor in _actors.Actors)
                CreateView(actor);

            _actors.Spawned += OnSpawned;
            _actors.Moved += OnMoved;
        }

        private void OnDestroy()
        {
            if (_actors == null)
                return;

            _actors.Spawned -= OnSpawned;
            _actors.Moved -= OnMoved;
        }

        private void OnSpawned(Actor actor) => CreateView(actor);

        private void OnMoved(Actor actor)
        {
            if (_views.TryGetValue(actor, out var view))
                view.MoveTo(_grid.GetCellLocalPosition(actor.Coordinate));
        }

        private void CreateView(Actor actor)
        {
            if (_views.ContainsKey(actor))
                return;

            var view = Instantiate(actorViewPrefab, actorRoot);
            var rect = (RectTransform)view.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.localScale = Vector3.one;
            rect.sizeDelta = new Vector2(_grid.CellSize, _grid.CellSize);

            view.SetSprite(visualConfig.Resolve(actor.Type));
            view.SetAnchoredPosition(_grid.GetCellLocalPosition(actor.Coordinate));

            _views[actor] = view;
        }
    }
}
