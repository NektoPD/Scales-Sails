using UnityEngine;

namespace UILogic
{
    public class CursorController : MonoBehaviour
    {
        [SerializeField] private Texture2D _menuCursor;
        [SerializeField] private Texture2D _gameCursor;
        [SerializeField] private Vector2 _menuHotspot = Vector2.zero;
        [SerializeField] private Vector2 _gameHotspot = Vector2.zero;
        [SerializeField] private CursorMode _cursorMode = CursorMode.Auto;

        private bool _inGame;

        private void Start()
        {
            ShowMenuCursor();
        }

        public void ShowMenuCursor()
        {
            _inGame = false;
            Cursor.SetCursor(_menuCursor, _menuHotspot, _cursorMode);
        }

        public void ShowGameCursor()
        {
            _inGame = true;
            Cursor.SetCursor(_gameCursor, _gameHotspot, _cursorMode);
        }

        public void Toggle()
        {
            if (_inGame)
                ShowMenuCursor();
            else
                ShowGameCursor();
        }
    }
}
