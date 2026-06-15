using UnityEngine;
using UnityEngine.UI;

namespace CatchHim.Gameplay.Grid
{
    public class CellVIew : MonoBehaviour
    {
        [SerializeField] private Image stateImage;

        public void SetStateImage(Sprite sprite)
        {
            stateImage.sprite = sprite;
            stateImage.enabled = sprite != null;
            stateImage.gameObject.SetActive(sprite != null);
        }
    }
}