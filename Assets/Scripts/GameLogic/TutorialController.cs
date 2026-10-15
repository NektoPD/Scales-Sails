using System.Collections;
using DG.Tweening;
using EnemyLogic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameLogic
{
    public class TutorialController : MonoBehaviour
    {
        [Header("Fade")]
        [SerializeField] private CanvasGroup _blackFade;
        [SerializeField] private float _fadeDuration = 1f;

        [Header("Hint sprites (player children)")]
        [SerializeField] private SpriteRenderer _wHint;
        [SerializeField] private SpriteRenderer _aHint;
        [SerializeField] private SpriteRenderer _dHint;
        [SerializeField] private SpriteRenderer _mouseHint;
        [SerializeField] private float _hintFadeDuration = 0.4f;

        [Header("Refs")]
        [SerializeField] private EnemySpawner _spawner;
        [SerializeField] private Transform _player;

        [Header("Timing")]
        [SerializeField] private float _delayBeforeWHint = 1f;
        [SerializeField] private float _delayBeforeEnemy = 1f;
        [SerializeField] private float _delayBeforeAdHint = 1f;
        [SerializeField] private float _delayBeforeWaves = 3f;
        [SerializeField] private float _mouseHintRange = 6f;

        private EnemyShip _tutorialEnemy;
        private Quaternion _wHintRotation;
        private Quaternion _aHintRotation;
        private Quaternion _dHintRotation;
        private Quaternion _mouseHintRotation;

        private void Awake()
        {
            _wHintRotation = GetHintRotation(_wHint);
            _aHintRotation = GetHintRotation(_aHint);
            _dHintRotation = GetHintRotation(_dHint);
            _mouseHintRotation = GetHintRotation(_mouseHint);
        }

        private void Start()
        {
            HideHint(_wHint);
            HideHint(_aHint);
            HideHint(_dHint);
            HideHint(_mouseHint);
            StartCoroutine(RunTutorial());
        }

        private void LateUpdate()
        {
            KeepHintRotation(_wHint, _wHintRotation);
            KeepHintRotation(_aHint, _aHintRotation);
            KeepHintRotation(_dHint, _dHintRotation);
            KeepHintRotation(_mouseHint, _mouseHintRotation);
        }

        private IEnumerator RunTutorial()
        {
            yield return FadeInScene();

            yield return new WaitForSeconds(_delayBeforeWHint);

            ShowHint(_wHint);
            yield return WaitForKeys(Key.W);
            HideHint(_wHint);

            yield return new WaitForSeconds(_delayBeforeEnemy);
            SpawnTutorialEnemy();

            yield return new WaitForSeconds(_delayBeforeAdHint);
            ShowHint(_aHint);
            ShowHint(_dHint);
            yield return WaitForKeys(Key.A, Key.D);
            HideHint(_aHint);
            HideHint(_dHint);

            yield return WaitForPlayerNearEnemy();
            ShowHint(_mouseHint);

            yield return WaitForEnemyDead();
            HideHint(_mouseHint);

            yield return new WaitForSeconds(_delayBeforeWaves);

            if (_spawner != null)
                _spawner.StartWaves();
        }

        private IEnumerator FadeInScene()
        {
            if (_blackFade == null)
                yield break;

            _blackFade.alpha = 1f;
            _blackFade.blocksRaycasts = false;
            yield return _blackFade.DOFade(0f, _fadeDuration).WaitForCompletion();
            _blackFade.gameObject.SetActive(false);
        }

        private IEnumerator WaitForKeys(params Key[] keys)
        {
            while (true)
            {
                if (Keyboard.current != null)
                {
                    foreach (Key key in keys)
                    {
                        if (Keyboard.current[key].wasPressedThisFrame)
                            yield break;
                    }
                }

                yield return null;
            }
        }

        private void SpawnTutorialEnemy()
        {
            if (_spawner == null)
                return;

            _tutorialEnemy = _spawner.SpawnSingle(_spawner.Sloop, true);
        }

        private IEnumerator WaitForPlayerNearEnemy()
        {
            while (_tutorialEnemy != null && !_tutorialEnemy.IsDead)
            {
                if (_player != null &&
                    Vector2.Distance(_player.position, _tutorialEnemy.transform.position) <= _mouseHintRange)
                    yield break;

                yield return null;
            }
        }

        private IEnumerator WaitForEnemyDead()
        {
            while (_tutorialEnemy != null && !_tutorialEnemy.IsDead)
                yield return null;
        }

        private void ShowHint(SpriteRenderer hint)
        {
            if (hint == null)
                return;

            hint.gameObject.SetActive(true);
            hint.DOKill();
            SetAlpha(hint, 0f);
            hint.DOFade(1f, _hintFadeDuration);

            if (hint == _mouseHint)
                PulseHold(hint);
        }

        private void PulseHold(SpriteRenderer hint)
        {
            Vector3 baseScale = hint.transform.localScale;
            hint.transform.DOScale(baseScale * 0.85f, 0.5f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetId(hint);
        }

        private void HideHint(SpriteRenderer hint)
        {
            if (hint == null)
                return;

            hint.DOKill();
            DOTween.Kill(hint);
            hint.transform.DOKill();
            hint.DOFade(0f, _hintFadeDuration).OnComplete(() => hint.gameObject.SetActive(false));
        }

        private void SetAlpha(SpriteRenderer hint, float alpha)
        {
            Color color = hint.color;
            color.a = alpha;
            hint.color = color;
        }

        private static Quaternion GetHintRotation(SpriteRenderer hint)
        {
            return hint != null ? hint.transform.rotation : Quaternion.identity;
        }

        private static void KeepHintRotation(SpriteRenderer hint, Quaternion rotation)
        {
            if (hint != null)
                hint.transform.rotation = rotation;
        }
    }
}
