using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CatchHim.Gameplay.Actors
{
    public class ActorView : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private float moveDuration = 0.25f;

        private RectTransform _rect;
        private Coroutine _moveRoutine;

        private RectTransform Rect => _rect != null ? _rect : _rect = (RectTransform)transform;

        public void SetSprite(Sprite sprite)
        {
            image.sprite = sprite;
            image.enabled = sprite != null;
        }

        public void SetAnchoredPosition(Vector2 anchoredPosition)
        {
            StopMove();
            Rect.anchoredPosition = anchoredPosition;
        }

        public void MoveTo(Vector2 anchoredPosition)
        {
            StopMove();

            if (moveDuration <= 0f || Rect.anchoredPosition == anchoredPosition)
            {
                Rect.anchoredPosition = anchoredPosition;
                return;
            }

            _moveRoutine = StartCoroutine(MoveRoutine(anchoredPosition));
        }

        private IEnumerator MoveRoutine(Vector2 target)
        {
            Vector2 start = Rect.anchoredPosition;
            float elapsed = 0f;

            while (elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / moveDuration));
                Rect.anchoredPosition = Vector2.Lerp(start, target, t);
                yield return null;
            }

            Rect.anchoredPosition = target;
            _moveRoutine = null;
        }

        private void StopMove()
        {
            if (_moveRoutine == null)
                return;

            StopCoroutine(_moveRoutine);
            _moveRoutine = null;
        }
    }
}
