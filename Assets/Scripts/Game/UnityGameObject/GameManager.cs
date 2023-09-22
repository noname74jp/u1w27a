using Game.Logic;
using Game.UnityGameObject.Char;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.UnityGameObject
{
    /// <summary>
    /// ゲーム管理。
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region variables

        /// <summary>
        /// 入力アクション。
        /// </summary>
        [SerializeField] private InputAction inputAction;

        /// <summary>
        /// プレイヤーのオブジェクト。
        /// </summary>
        [SerializeField] private Player player;

        /// <summary>
        /// プレイヤーのロジック。
        /// </summary>
        private PlayerLogic _playerLogic;

        /// <summary>
        /// タイマー。
        /// </summary>
        private double _timer;

        #endregion

        #region methods

        /// <summary>
        /// Unityイベント関数Awake。
        /// </summary>
        private void Awake()
        {
            inputAction?.Enable();
            Initialize();
        }

        /// <summary>
        /// Unityイベント関数OnEnable。
        /// </summary>
        private void OnEnable()
        {
            inputAction?.Enable();
        }

        /// <summary>
        /// Unityイベント関数OnDisable。
        /// </summary>
        private void OnDisable()
        {
            inputAction?.Disable();
        }

        /// <summary>
        /// Unityイベント関数Update。
        /// </summary>
        public void Update()
        {
            // 入力管理
            var isKeyPressed = inputAction != null && inputAction.ReadValue<float>() >= 0.5f;

            // ロジックの更新
            _timer += Time.deltaTime;
            while (_timer >= Defines.SecondsPerFrame)
            {
                _timer -= Defines.SecondsPerFrame;
                _playerLogic.UpdateStatus(isKeyPressed);
            }

            // Unity上の更新
            player.UpdateStatus(_playerLogic);
        }

        /// <summary>
        /// 初期化する。
        /// </summary>
        private void Initialize()
        {
            _timer = 0.0;
            _playerLogic = new PlayerLogic();
            _playerLogic.Initialize();
            player.UpdateStatus(_playerLogic);
        }

        #endregion
    }
}