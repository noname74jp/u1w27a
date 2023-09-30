using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Logic;
using Game.UnityGameObject.Char;
using Game.UnityGameObject.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.UnityGameObject
{
    /// <summary>
    /// ゲーム管理。
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region classes

        /// <summary>
        /// 入力アクションデータ。
        /// </summary>
        [Serializable]
        private class InputActionData
        {
            #region variables

            /// <summary>
            /// 決定。
            /// </summary>
            [SerializeField] public InputAction decide;

            /// <summary>
            /// リトライ。
            /// </summary>
            [SerializeField] public InputAction retry;

            #endregion

            #region methods

            /// <summary>
            /// 有効にする。
            /// </summary>
            public void Enable()
            {
                decide.Enable();
                retry.Enable();
            }

            /// <summary>
            /// 無効にする。
            /// </summary>
            public void Disable()
            {
                decide.Disable();
                retry.Disable();
            }

            /// <summary>
            /// 決定ボタンが押されているか。
            /// </summary>
            /// <returns></returns>
            public bool IsDecideKeyPressed()
            {
                return decide.ReadValue<float>() >= 0.5f;
            }

            /// <summary>
            /// リトライボタンが押されているか。
            /// </summary>
            /// <returns></returns>
            public bool IsRetryKeyPressed()
            {
                return retry.ReadValue<float>() >= 0.5f;
            }

            #endregion
        }

        #endregion

        #region constants

        /// <summary>
        /// プレイヤーの弾数。
        /// </summary>
        private const int PlayerBulletCount = 32;

        /// <summary>
        /// 敵数。
        /// </summary>
        private const int EnemyCount = 512;

        #endregion

        #region variables

        /// <summary>
        /// 入力アクションデータ。
        /// </summary>
        [SerializeField] private InputActionData inputActionData;

        /// <summary>
        /// スコアボード。
        /// </summary>
        [SerializeField] private ScoreBoard scoreBoard;

        /// <summary>
        /// タイトルロゴ。
        /// </summary>
        [SerializeField] private TitleLogo titleLogo;

        /// <summary>
        /// ワールドのルートのオブジェクト。
        /// </summary>
        [SerializeField] private WorldRoot worldRoot;

        /// <summary>
        /// プレイヤーのオブジェクト。
        /// </summary>
        [SerializeField] private Player player;

        /// <summary>
        /// プレイヤーの弾丸管理。
        /// </summary>
        [SerializeField] private BulletCoordinator playerBulletCoordinator;

        /// <summary>
        /// 敵管理。
        /// </summary>
        [SerializeField] private EnemyCoordinator enemyCoordinator;

        /// <summary>
        /// プレイヤーのロジック。
        /// </summary>
        private PlayerLogic _playerLogic;

        /// <summary>
        /// プレイヤー弾のロジック。
        /// </summary>
        private List<BulletLogic> _playerBulletLogics;

        /// <summary>
        /// 敵生成のロジック。
        /// </summary>
        private EnemySpawnerLogic _enemySpawnerLogic;

        /// <summary>
        /// ワールドのルートのロジック。
        /// </summary>
        private WorldRootLogic _worldRootLogic;

        /// <summary>
        /// タイマー。
        /// </summary>
        private double _timer;

        /// <summary>
        /// スコア。
        /// </summary>
        private int _score;

        /// <summary>
        /// キャンセルトークン。
        /// </summary>
        private CancellationTokenSource _cts;

        #endregion

        #region methods

        /// <summary>
        /// Unityイベント関数Awake。
        /// </summary>
        private void Awake()
        {
            playerBulletCoordinator.Initialize(PlayerBulletCount);
            enemyCoordinator.Initialize(EnemyCount);
            StartGameLoop();
        }

        /// <summary>
        /// Unityイベント関数OnEnable。
        /// </summary>
        private void OnEnable()
        {
            inputActionData.Enable();
        }

        /// <summary>
        /// Unityイベント関数OnDisable。
        /// </summary>
        private void OnDisable()
        {
            inputActionData.Disable();
        }

        /// <summary>
        /// Unityイベント関数OnDestroy。
        /// </summary>
        private void OnDestroy()
        {
            StopGameLoop();
        }

        /// <summary>
        /// Unityイベント関数OnApplicationQuit。
        /// </summary>
        private void OnApplicationQuit()
        {
            StopGameLoop();
        }

        /// <summary>
        /// ゲームループを開始する。
        /// </summary>
        private void StartGameLoop()
        {
            _cts = new CancellationTokenSource();
            UpdateTitle(_cts.Token).Forget();
        }

        /// <summary>
        /// ゲームループを停止する。
        /// </summary>
        private void StopGameLoop()
        {
            _cts?.Cancel();
            _cts = null;
        }

        /// <summary>
        /// 初期化する。
        /// </summary>
        private void Initialize()
        {
            _timer = 0.0;
            _score = 0;

            _playerLogic = new PlayerLogic();
            _playerLogic.Create();
            player.Initialize(_playerLogic);

            _playerBulletLogics = new List<BulletLogic>();
            foreach (var playerBullet in playerBulletCoordinator.Bullets)
            {
                BulletLogic playerBulletLogic = new();
                _playerBulletLogics.Add(playerBulletLogic);
                playerBullet.Initialize(playerBulletLogic);
            }

            _enemySpawnerLogic = new EnemySpawnerLogic(EnemyCount);
            var enemyIndex = 0;
            foreach (var enemy in _enemySpawnerLogic.Enemies)
            {
                enemyCoordinator.Enemies[enemyIndex].Initialize(enemy);
                enemyIndex++;
            }

            _worldRootLogic = new WorldRootLogic(_playerLogic);
            worldRoot.Initialize(_worldRootLogic);
            scoreBoard.SetScore(_score);
        }

        /// <summary>
        /// タイトルを更新する。
        /// </summary>
        /// <param name="token">キャンセルトークン。</param>
        private async UniTaskVoid UpdateTitle(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            // 初期化
            Initialize();
            titleLogo.gameObject.SetActive(true);

            // メインループ
            while (true)
            {
                // ゲーム開始
                if (inputActionData.IsDecideKeyPressed())
                {
                    break;
                }

                // 次のフレームへ
                await UniTask.NextFrame(token);
            }

            // タイトル終了処理
            titleLogo.gameObject.SetActive(false);
            UpdateGame(token).Forget();
        }

        /// <summary>
        /// ゲームを更新する。
        /// </summary>
        /// <param name="token">キャンセルトークン。</param>
        private async UniTaskVoid UpdateGame(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            _worldRootLogic.SetTargetScale(WorldRootLogic.MinScale);

            // メインループ
            while (true)
            {
                // 入力管理
                var isDecideKeyPressed = inputActionData.IsDecideKeyPressed();
                var isRetryKeyPressed = inputActionData.IsRetryKeyPressed();

                // リトライ
                if (isRetryKeyPressed)
                {
                    UpdateTitle(token).Forget();
                    break;
                }

                // ロジックの更新
                var isGameOver = false;
                _timer += Time.deltaTime;
                while (_timer >= Defines.SecondsPerFrame)
                {
                    _timer -= Defines.SecondsPerFrame;

                    // 状態を更新
                    _playerLogic.UpdateStatus(isDecideKeyPressed, _playerBulletLogics);
                    _playerBulletLogics.ForEach(logic => logic.UpdateStatus());
                    _enemySpawnerLogic.UpdateStatus(_playerLogic);
                    _worldRootLogic.UpdateStatus();

                    // プレイヤーと敵とのヒット判定
                    var hitEnemyLogic = _playerLogic.FindHitTarget(_enemySpawnerLogic.ActiveEnemies);
                    if (hitEnemyLogic != null)
                    {
                        isGameOver = true;
                        break;
                    }

                    // 敵とプレイヤー弾とのヒット判定
                    for (var enemyLogicNode = _enemySpawnerLogic.ActiveEnemies.First; enemyLogicNode != null; enemyLogicNode = enemyLogicNode.Next)
                    {
                        var enemyLogic = enemyLogicNode.Value;
                        var hitBulletLogic = enemyLogic.FindHitTarget(_playerBulletLogics);
                        if (hitBulletLogic == null)
                        {
                            continue;
                        }

                        hitBulletLogic.Destroy();
                        enemyLogic.Destroy(); // TODO: nn74: HP計算 
                        _enemySpawnerLogic.ActiveEnemies.Remove(enemyLogicNode);
                        _score += 100; // TODO: nn74: スコア計算
                    }

                    // 生存ボーナス
                    _score++; // TODO: nn74: スコア計算
                }

                // Unity上の更新
                scoreBoard.SetScore(_score);
                worldRoot.UpdateStatus();
                player.UpdateStatus();
                foreach (var playerBullet in playerBulletCoordinator.Bullets)
                {
                    playerBullet.UpdateStatus();
                }

                foreach (var enemy in enemyCoordinator.Enemies)
                {
                    enemy.UpdateStatus();
                }

                // プレイヤーが死んでいたら抜ける
                if (isGameOver)
                {
                    UpdateGameOver(token).Forget();
                    return;
                }

                // 次のフレームへ
                await UniTask.NextFrame(token);
            }
        }

        /// <summary>
        /// ゲームオーバーを更新する。
        /// </summary>
        /// <param name="token">キャンセルトークン。</param>
        private async UniTaskVoid UpdateGameOver(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            _worldRootLogic.SetTargetScale(WorldRootLogic.MaxScale);

            // メインループ
            while (true)
            {
                // 入力管理
                var isRetryKeyPressed = inputActionData.IsRetryKeyPressed();

                // リトライ
                if (isRetryKeyPressed)
                {
                    UpdateTitle(token).Forget();
                    break;
                }

                // ロジックの更新
                _timer += Time.deltaTime;
                while (_timer >= Defines.SecondsPerFrame)
                {
                    _timer -= Defines.SecondsPerFrame;
                    _worldRootLogic.UpdateStatus();
                }

                // Unity上の更新
                worldRoot.UpdateStatus();
                player.UpdateStatus();

                // 次のフレームへ
                await UniTask.NextFrame(token);
            }
        }

        #endregion
    }
}
