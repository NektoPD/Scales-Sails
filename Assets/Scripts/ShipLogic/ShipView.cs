using UnityEngine;

namespace ShipLogic
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ShipView : MonoBehaviour
    {
        [SerializeField] private Sprite _menuSprite;
        [SerializeField] private Sprite _gameSprite;

        private SpriteRenderer _renderer;
        private bool _inGame;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            ShowMenuSprite();
        }

        public void ShowMenuSprite()
        {
            _inGame = false;
            _renderer.sprite = _menuSprite;
        }

        public void ShowGameSprite()
        {
            _inGame = true;
            _renderer.sprite = _gameSprite;
        }

        public void Toggle()
        {
            if (_inGame)
                ShowMenuSprite();
            else
                ShowGameSprite();
        }
    }
}
