using System.Collections.Generic;
using UnityEngine;

namespace Game.Logic
{
    /// <summary>
    /// 敵管理のロジック。
    /// </summary>
    public class EnemySpawnerLogic
    {
        #region properties

        /// <summary>
        /// 敵のロジックのリスト。
        /// </summary>
        public List<EnemyLogic> Enemies => _enemies;

        /// <summary>
        /// アクティブな敵のロジックのリスト。
        /// </summary>
        public LinkedList<EnemyLogic> ActiveEnemies => _activeEnemies;

        #endregion

        #region variables

        /// <summary>
        /// カウンター。
        /// </summary>
        private int _frameCounter;

        /// <summary>
        /// 敵のロジックのリスト。
        /// </summary>
        private readonly List<EnemyLogic> _enemies;

        /// <summary>
        /// アクティブな敵のロジックのリスト。
        /// </summary>
        private readonly LinkedList<EnemyLogic> _activeEnemies;

        /// <summary>
        /// 非アクティブな敵のロジックのリスト。
        /// </summary>
        private readonly LinkedList<EnemyLogic> _inactiveEnemies;

        #endregion

        #region methods

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="enemyCount">敵の数。</param>
        public EnemySpawnerLogic(int enemyCount)
        {
            _frameCounter = 0;
            _enemies = new List<EnemyLogic>();
            for (var i = 0; i < enemyCount; i++)
            {
                _enemies.Add(new EnemyLogic());
            }

            _activeEnemies = new LinkedList<EnemyLogic>();
            _inactiveEnemies = new LinkedList<EnemyLogic>(_enemies);
        }

        /// <summary>
        /// 更新する。
        /// </summary>
        /// <param name="playerLogic">プレイヤーのロジック。</param>
        public void UpdateStatus(PlayerLogic playerLogic)
        {
            // フレームカウンターを進める
            _frameCounter++;

            // 状態を更新
            _enemies.ForEach(enemyLogic => enemyLogic.UpdateStatus(playerLogic));

            // 敵の生成を試みる
            TrySpawnClub(playerLogic, _frameCounter);
            TrySpawnDiamond(playerLogic, _frameCounter);
            TrySpawnHeart(playerLogic, _frameCounter);
            TrySpawnSpade(playerLogic, _frameCounter);
            TrySpawnJoker(playerLogic, _frameCounter);
        }

        /// <summary>
        /// <see cref="EnemyLogic.EnemyCategory.Club" />を生成する。
        /// </summary>
        /// <param name="playerLogic">プレイヤーのロジック。</param>
        /// <param name="frameCounter">フレーム数のカウンター。</param>
        private void TrySpawnClub(PlayerLogic playerLogic, int frameCounter)
        {
            // 1秒に1回生成
            // TODO: nn74: 仮設定
            if (frameCounter % 240 != 0)
            {
                return;
            }

            // 敵を生成
            var enemy = ActivateEnemy();
            var location = GetRandomSpawnLocation(false, true);
            enemy.Create(EnemyLogic.EnemyType.Club01, location, playerLogic);
        }

        /// <summary>
        /// <see cref="EnemyLogic.EnemyCategory.Diamond" />を生成する。
        /// </summary>
        /// <param name="playerLogic">プレイヤーのロジック。</param>
        /// <param name="frameCounter">フレーム数のカウンター。</param>
        private void TrySpawnDiamond(PlayerLogic playerLogic, int frameCounter)
        {
            // 1秒に1回生成
            // TODO: nn74: 仮設定
            if (frameCounter % 240 != 0)
            {
                return;
            }

            // 敵を生成
            var enemy = ActivateEnemy();
            var location = GetRandomSpawnLocation(true, false);
            enemy.Create(EnemyLogic.EnemyType.Diamond01, location, playerLogic);
        }

        /// <summary>
        /// <see cref="EnemyLogic.EnemyCategory.Heart" />を生成する。
        /// </summary>
        /// <param name="playerLogic">プレイヤーのロジック。</param>
        /// <param name="frameCounter">フレーム数のカウンター。</param>
        private void TrySpawnHeart(PlayerLogic playerLogic, int frameCounter)
        {
            // 1秒に1回生成
            // TODO: nn74: 仮設定
            if (frameCounter % 240 != 0)
            {
                return;
            }

            // 敵を生成
            var enemy = ActivateEnemy();
            var location = GetRandomSpawnLocation(true, true);
            enemy.Create(EnemyLogic.EnemyType.Heart01, location, playerLogic);
        }

        /// <summary>
        /// <see cref="EnemyLogic.EnemyCategory.Spade" />を生成する。
        /// </summary>
        /// <param name="playerLogic">プレイヤーのロジック。</param>
        /// <param name="frameCounter">フレーム数のカウンター。</param>
        private void TrySpawnSpade(PlayerLogic playerLogic, int frameCounter)
        {
            // 1秒に1回生成
            // TODO: nn74: 仮設定
            if (frameCounter % 240 != 0)
            {
                return;
            }

            // 敵を生成
            var enemy = ActivateEnemy();
            var location = GetRandomSpawnLocation(true, true);
            enemy.Create(EnemyLogic.EnemyType.Spade01, location, playerLogic);
        }

        /// <summary>
        /// <see cref="EnemyLogic.EnemyCategory.Joker" />を生成する。
        /// </summary>
        /// <param name="playerLogic">プレイヤーのロジック。</param>
        /// <param name="frameCounter">フレーム数のカウンター。</param>
        private void TrySpawnJoker(PlayerLogic playerLogic, int frameCounter)
        {
            // 1秒に1回生成
            // TODO: nn74: 仮設定
            if (frameCounter % 240 != 0)
            {
                return;
            }

            // 敵を生成
            var enemy = ActivateEnemy();
            var location = GetRandomSpawnLocation(true, false);
            enemy.Create(EnemyLogic.EnemyType.Joker, location, playerLogic);
        }

        /// <summary>
        /// ランダムな生成位置を取得。
        /// </summary>
        /// <param name="enableTopBottom">上下に出現可能か。</param>
        /// <param name="enableLeftRight">左右に出現可能か。</param>
        /// <returns>生成位置。</returns>
        private Vector2 GetRandomSpawnLocation(bool enableTopBottom, bool enableLeftRight)
        {
            var random03 = Random.Range(0, 4);

            // 上下か左右かを抽選。
            var isTopBottom = (random03 & 0x02) == 0;
            isTopBottom &= enableTopBottom;
            isTopBottom |= !enableLeftRight;

            // 上下のどちらかに生成
            if (isTopBottom)
            {
                var x = Random.Range(-440.0f, 440.0f);
                var y = (random03 & 0x01) == 0 ? -270.0f : 270.0f;
                return new Vector2(x, y);
            }
            // 左右のどちらかに生成
            else
            {
                var x = Random.Range(0, 2) == 0 ? -480.0f : 480.0f;
                var y = Random.Range(-222.0f, 222.0f);
                return new Vector2(x, y);
            }
        }

        private EnemyLogic ActivateEnemy()
        {
            // 非アクティブリストの先頭を取得し、nullならnullを返す
            var enemy = _inactiveEnemies.First.Value;
            if (enemy == null)
            {
                return null;
            }

            // 非アクティブリストの先頭をアクティブリストに移動
            _inactiveEnemies.RemoveFirst();
            _activeEnemies.AddLast(enemy);

            // 取得したEnemyLogicを返す
            return enemy;
        }

        #endregion
    }
}
