using UnityEngine;
using UnityEngine.UI;

namespace CatchHim.Gameplay.Actors
{
    public class ActorView : MonoBehaviour
    {
        [SerializeField] private Image image;

        public void SetSprite(Sprite sprite)
        {
            image.sprite = sprite;
            image.enabled = sprite != null;
        }

        public void SetAnchoredPosition(Vector2 anchoredPosition)
        {
            ((RectTransform)transform).anchoredPosition = anchoredPosition;
        }
    }
}
