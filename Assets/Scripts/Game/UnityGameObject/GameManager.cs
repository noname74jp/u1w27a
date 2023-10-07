using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Logic;
using Game.UnityGameObject.Char;
using Game.UnityGameObject.UI;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using unityroom.Api;
using Random = UnityEngine.Random;

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
            [SerializeField] private InputAction decide;

            /// <summary>
            /// リトライ。
            /// </summary>
            [SerializeField] private InputAction retry;

            /// <summary>
            /// 音量アップ。
            /// </summary>
            [SerializeField] private InputAction volumeUp;

            /// <summary>
            /// 音量ダウン。
            /// </summary>
            [SerializeField] private InputAction volumeDown;

            #endregion

            #region methods

            /// <summary>
            /// 有効にする。
            /// </summary>
            public void Enable()
            {
                decide.Enable();
                retry.Enable();
                volumeUp.Enable();
                volumeDown.Enable();
            }

            /// <summary>
            /// 無効にする。
            /// </summary>
            public void Disable()
            {
                decide.Disable();
                retry.Disable();
                volumeUp.Disable();
                volumeDown.Disable();
            }

            /// <summary>
            /// 決定ボタンが押されているか。
            /// </summary>
            /// <returns>ボタンが押されていればtrue。</returns>
            public bool IsDecideKeyPressed()
            {
                return decide.IsPressed();
            }

            /// <summary>
            /// リトライボタンが押されているか。
            /// </summary>
            /// <returns>ボタンが押されていればtrue。</returns>
            public bool IsRetryKeyPressed()
            {
                return retry.IsPressed();
            }

            /// <summary>
            /// 音量アップ/ダウンボタンが押されているか。
            /// </summary>
            /// <returns>音量ボタンが押されていれば1、音量ダウンボタンが押されていれば-1、どちらも押されていなければ0。</returns>
            public float GetVolumeUpDownValue()
            {
                if (volumeUp.triggered)
                {
                    return 1.0f;
                }

                if (volumeDown.triggered)
                {
                    return -1.0f;
                }

                return 0.0f;
            }

            #endregion
        }

        /// <summary>
        /// サウンド。
        /// </summary>
        [Serializable]
        private class Sound
        {
            /// <summary>
            /// 環境音。
            /// </summary>
            [SerializeField] public AudioSource ambience;

            /// <summary>
            /// オーディオミキサー。
            /// </summary>
            [SerializeField] public AudioMixer audioMixer;

            /// <summary>
            /// BGM。
            /// </summary>
            [SerializeField] public AudioSource bgm;

            /// <summary>
            /// ゲームオーバー。
            /// </summary>
            [SerializeField] public AudioSource gameOver;

            /// <summary>
            /// ヒット。
            /// </summary>
            [SerializeField] public AudioSource hit;

            /// <summary>
            /// ジャンプ。
            /// </summary>
            [SerializeField] public AudioSource jump;

            /// <summary>
            /// ショット。
            /// </summary>
            [SerializeField] public AudioSource shot;
        }

        #endregion

        #region constants

        /// <summary>
        /// プレイヤーの弾数。
        /// </summary>
        private const int PlayerBulletCount = 60;

        /// <summary>
        /// 敵数。
        /// </summary>
        private const int EnemyCount = 128;

        #endregion

        #region variables

        /// <summary>
        /// 入力アクションデータ。
        /// </summary>
        [SerializeField] private InputActionData inputActionData;

        /// <summary>
        /// サウンド。
        /// </summary>
        [SerializeField] private Sound sound;

        /// <summary>
        /// スコアボード。
        /// </summary>
        [SerializeField] private ScoreBoard scoreBoard;

        /// <summary>
        /// タイトルロゴ。
        /// </summary>
        [SerializeField] private TitleLogo titleLogo;

        /// <summary>
        /// ライセンスウィンドウ。
        /// </summary>
        [SerializeField] private LicensesWindow licencesWindow;

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
            UpdateVolumeController(_cts.Token).Forget();
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

            titleLogo.gameObject.SetActive(false);
            licencesWindow.gameObject.SetActive(false);
        }

        /// <summary>
        /// 音量制御を更新する。
        /// </summary>
        /// <param name="token">キャンセルトークン。</param>
        private async UniTaskVoid UpdateVolumeController(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await UniTask.NextFrame(token);

            const float maxVolume = 10.0f;
            var volume = 7.0f;
            sound.audioMixer.SetFloat("MasterVolume", ConvertVolumeToDB(volume / maxVolume));

            // メインループ
            var previousDeltaVolume = 0.0f;
            while (true)
            {
                // 音量を設定
                var deltaVolume = inputActionData.GetVolumeUpDownValue();
                if (Math.Abs(previousDeltaVolume - deltaVolume) > 0.0f)
                {
                    var previousVolume = volume;
                    volume = Mathf.Clamp(volume + deltaVolume, 0.0f, maxVolume);
                    if (Math.Abs(previousVolume - volume) > 0.0f)
                    {
                        sound.audioMixer.SetFloat("MasterVolume", ConvertVolumeToDB(volume / maxVolume));
                    }
                }

                previousDeltaVolume = deltaVolume;

                // 次のフレームへ
                await UniTask.NextFrame(token);
            }
        }

        /// <summary>
        /// ボリュームをdbに変換する。
        /// </summary>
        /// <param name="volume">ボリューム(0.0〜1.0)。</param>
        /// <returns>dB(-80〜0)。</returns>
        private static float ConvertVolumeToDB(float volume)
        {
            return Mathf.Clamp(20f * Mathf.Log10(Mathf.Clamp01(volume)), -80f, 0f);
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

            // リトライキーが押されていたら待つ
            await UniTask.WaitWhile(() => inputActionData.IsRetryKeyPressed(), cancellationToken: token);

            // メインループ
            while (true)
            {
                // ゲーム開始
                if (inputActionData.IsDecideKeyPressed())
                {
                    break;
                }

                // ライセンス
                if (inputActionData.IsRetryKeyPressed())
                {
                    await UpdateLicenseWindow(token);
                }

                // 次のフレームへ
                await UniTask.NextFrame(token);
            }

            // タイトル終了処理
            titleLogo.gameObject.SetActive(false);
            UpdateGame(token).Forget();
        }

        /// <summary>
        /// ライセンスウィンドウを更新する。
        /// </summary>
        /// <param name="token">キャンセルトークン。</param>
        private async UniTask UpdateLicenseWindow(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            licencesWindow.gameObject.SetActive(true);
            await UniTask.WaitWhile(() => inputActionData.IsRetryKeyPressed(), cancellationToken: token);
            await UniTask.WaitWhile(() => !inputActionData.IsRetryKeyPressed(), cancellationToken: token);
            await UniTask.WaitWhile(() => inputActionData.IsRetryKeyPressed(), cancellationToken: token);
            licencesWindow.gameObject.SetActive(false);
        }

        /// <summary>
        /// ゲームを更新する。
        /// </summary>
        /// <param name="token">キャンセルトークン。</param>
        private async UniTaskVoid UpdateGame(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            _worldRootLogic.SetTargetScale(WorldRootLogic.MinScale);
            sound.bgm.Play();

            // メインループ
            var frameCount = 0;
            while (true)
            {
                // 入力管理
                var isDecideKeyPressed = inputActionData.IsDecideKeyPressed();
                var isRetryKeyPressed = inputActionData.IsRetryKeyPressed();

                // リトライ
                if (isRetryKeyPressed)
                {
                    UnityroomApiClient.Instance.SendScore(1, _score, ScoreboardWriteMode.HighScoreDesc);
                    UpdateTitle(token).Forget();
                    break;
                }

                // ロジックの更新
                var needPlayJumpSe = false;
                var needPlayShootSe = false;
                var needPlayHitSe = false;
                var isGameOver = false;
                _timer += Time.deltaTime;
                while (_timer >= Defines.SecondsPerFrame)
                {
                    _timer -= Defines.SecondsPerFrame;
                    frameCount++;

                    // 状態を更新
                    _playerLogic.UpdateStatus(isDecideKeyPressed, _playerBulletLogics, out var jumped, out var shot);
                    needPlayJumpSe |= jumped;
                    needPlayShootSe |= shot;
                    _playerBulletLogics.ForEach(logic => logic.UpdateStatus());
                    _enemySpawnerLogic.UpdateStatus(_playerLogic);
                    _worldRootLogic.UpdateStatus();

                    // プレイヤーと敵とのヒット判定
                    var hitEnemyLogic = _playerLogic.FindHitTarget(_enemySpawnerLogic.ActiveEnemies);
                    if (hitEnemyLogic != null)
                    {
                        isGameOver = true;
                        UnityroomApiClient.Instance.SendScore(1, _score, ScoreboardWriteMode.HighScoreDesc);
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
                        if (enemyLogic.AddDamage())
                        {
                            _score += enemyLogic.Score;
                            _enemySpawnerLogic.InactivateEnemy(enemyLogic);
                        }
                        else
                        {
                            _score += enemyLogic.Score / 5;
                        }

                        needPlayHitSe = true;
                    }

                    // 生存ボーナス
                    _score++; // TODO: nn74: スコア計算

                    // 定期的にハイスコア送信
                    if (frameCount % Mathf.RoundToInt((float)(Defines.FramePerSec * 15.0)) == 0)
                    {
                        UnityroomApiClient.Instance.SendScore(1, _score, ScoreboardWriteMode.HighScoreDesc);
                    }
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
                    UnityroomApiClient.Instance.SendScore(1, _score, ScoreboardWriteMode.HighScoreDesc);
                    UpdateGameOver(token).Forget();
                    break;
                }

                // SE再生
                if (needPlayJumpSe)
                {
                    sound.jump.Play();
                    sound.jump.pitch = Random.Range(1.0f, 1.2f);
                }

                if (needPlayShootSe)
                {
                    sound.shot.Play();
                    sound.shot.pitch = Random.Range(1.0f, 1.2f);
                }

                if (needPlayHitSe)
                {
                    sound.hit.Play();
                    sound.hit.pitch = Random.Range(1.0f, 1.2f);
                }

                // 次のフレームへ
                await UniTask.NextFrame(token);
            }

            sound.bgm.Stop();
        }

        /// <summary>
        /// ゲームオーバーを更新する。
        /// </summary>
        /// <param name="token">キャンセルトークン。</param>
        private async UniTaskVoid UpdateGameOver(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            _worldRootLogic.SetTargetScale(WorldRootLogic.MaxScale);
            sound.gameOver.Play();

            // メインループ
            var totalTime = 0.0f;
            while (true)
            {
                // 入力管理
                var isDecideKeyPressed = inputActionData.IsDecideKeyPressed();
                var isRetryKeyPressed = inputActionData.IsRetryKeyPressed();

                // リトライ
                totalTime += Time.deltaTime;
                if (isRetryKeyPressed || (totalTime >= 1.5f && isDecideKeyPressed))
                {
                    await UniTask.WaitWhile(() => inputActionData.IsDecideKeyPressed(), cancellationToken: token);
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
