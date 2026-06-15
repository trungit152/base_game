using UnityEngine;

namespace CatchHim.Gameplay.Grid
{
    public class GridViewManager : MonoBehaviour
    {
        [SerializeField] private CellVIew cellViewPrefab;
        [SerializeField] private CellVisualConfig cellVisualConfig;
        [SerializeField] private RectTransform gridRoot;
        private CellVIew[,] _views;

        public void Build(GridManager gridManager)
        {
            _views = new CellVIew[gridManager.X, gridManager.Y];
            for (int x = 0; x < gridManager.X; x++)
            {
                for (int y = 0; y < gridManager.Y; y++)
                {
                    GridData data = gridManager.Cells[x, y];

                    var view = Instantiate(cellViewPrefab, gridRoot);
                    var rect = (RectTransform)view.transform;
                    rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
                    rect.pivot = new Vector2(0.5f, 0.5f);
                    rect.localScale = Vector3.one;              
                    rect.sizeDelta = new Vector2(gridManager.CellSize, gridManager.CellSize);
                    rect.anchoredPosition = gridManager.GetCellLocalPosition(new Vector2Int(x, y));

                    _views[x, y] = view;
                    RefreshCell(x, y, data.State);
                }
            }
        }

        public void RefreshCell(int x, int y, ICellState state)
        {
            _views[x, y].SetStateImage(cellVisualConfig.Resolve(state.Visual));
        }
    }
}
