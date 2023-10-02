using System.Collections.Generic;
using UnityEngine;

namespace Game.Logic
{
    /// <summary>
    /// 敵管理のロジック。
    /// </summary>
    public class EnemySpawnerLogic
    {
        #region constants

        /// <summary>
        /// 敵の生成インターバル[frame]
        /// </summary>
        private const int EnemySpawnFrameInterval = (int)(Defines.FramePerSec * 1.25);

        #endregion

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
            _enemies.ForEach(enemyLogic =>
            {
                if (enemyLogic.UpdateStatus(playerLogic))
                {
                    InactivateEnemy(enemyLogic);
                }
            });

            // 敵の生成を試みる
            if (_frameCounter % EnemySpawnFrameInterval == 0)
            {
                TrySpawnClub(playerLogic, _frameCounter);
                TrySpawnDiamond(playerLogic, _frameCounter);
                TrySpawnHeart(playerLogic, _frameCounter);
                TrySpawnSpade(playerLogic, _frameCounter);
                TrySpawnJoker(playerLogic, _frameCounter);
            }
        }

        /// <summary>
        /// <see cref="EnemyLogic.EnemyCategory.Club" />を生成する。
        /// </summary>
        /// <param name="playerLogic">プレイヤーのロジック。</param>
        /// <param name="frameCounter">フレーム数のカウンター。</param>
        private void TrySpawnClub(PlayerLogic playerLogic, int frameCounter)
        {
            var count = frameCounter / EnemySpawnFrameInterval % 8;
            var type = (frameCounter / EnemySpawnFrameInterval / 8) switch
            {
                0 => EnemyLogic.EnemyType.Club00,
                1 => EnemyLogic.EnemyType.Club00,
                2 => (count & 0x01) == 0 ? EnemyLogic.EnemyType.Invalid : EnemyLogic.EnemyType.Club00,
                3 => (count & 0x03) == 0 ? EnemyLogic.EnemyType.Club01 : EnemyLogic.EnemyType.Club00,
                4 => (count & 0x03) == 0 ? EnemyLogic.EnemyType.Club01 : EnemyLogic.EnemyType.Club00,
                5 => (count & 0x03) == 0 ? EnemyLogic.EnemyType.Club01 : EnemyLogic.EnemyType.Club00,
                6 => (count & 0x03) == 0 ? EnemyLogic.EnemyType.Club01 : EnemyLogic.EnemyType.Club00,
                7 => (count & 0x01) == 0 ? EnemyLogic.EnemyType.Club01 : EnemyLogic.EnemyType.Club00,
                8 => (count & 0x01) == 0 ? EnemyLogic.EnemyType.Club01 : EnemyLogic.EnemyType.Club00,
                9 => (count & 0x01) == 0 ? EnemyLogic.EnemyType.Club01 : EnemyLogic.EnemyType.Club00,
                _ => EnemyLogic.EnemyType.Club01
            };

            if (type == EnemyLogic.EnemyType.Invalid)
            {
                return;
            }

            // 敵を生成
            var enemy = ActivateEnemy();
            if (enemy == null)
            {
                return;
            }

            var location = GetRandomSpawnLocation(false, true);
            enemy.Create(type, location, playerLogic);
        }

        /// <summary>
        /// <see cref="EnemyLogic.EnemyCategory.Diamond" />を生成する。
        /// </summary>
        /// <param name="playerLogic">プレイヤーのロジック。</param>
        /// <param name="frameCounter">フレーム数のカウンター。</param>
        private void TrySpawnDiamond(PlayerLogic playerLogic, int frameCounter)
        {
            var count = frameCounter / EnemySpawnFrameInterval % 8;
            var type = (frameCounter / EnemySpawnFrameInterval / 8) switch
            {
                0 => EnemyLogic.EnemyType.Invalid,
                1 => EnemyLogic.EnemyType.Diamond00,
                2 => (count & 0x01) == 0 ? EnemyLogic.EnemyType.Invalid : EnemyLogic.EnemyType.Diamond00,
                3 => (count & 0x01) == 0 ? EnemyLogic.EnemyType.Invalid : EnemyLogic.EnemyType.Diamond00,
                4 => (count & 0x03) == 0 ? EnemyLogic.EnemyType.Diamond01 : EnemyLogic.EnemyType.Diamond00,
                5 => (count & 0x03) == 0 ? EnemyLogic.EnemyType.Diamond01 : EnemyLogic.EnemyType.Diamond00,
                6 => (count & 0x03) == 0 ? EnemyLogic.EnemyType.Diamond01 : EnemyLogic.EnemyType.Diamond00,
                7 => (count & 0x03) == 0 ? EnemyLogic.EnemyType.Diamond01 : EnemyLogic.EnemyType.Diamond00,
                8 => (count & 0x03) == 0 ? EnemyLogic.EnemyType.Diamond01 : EnemyLogic.EnemyType.Diamond00,
                9 => (count & 0x01) == 0 ? EnemyLogic.EnemyType.Diamond01 : EnemyLogic.EnemyType.Diamond00,
                10 => (count & 0x01) == 0 ? EnemyLogic.EnemyType.Diamond01 : EnemyLogic.EnemyType.Diamond00,
                11 => (count & 0x01) == 0 ? EnemyLogic.EnemyType.Diamond01 : EnemyLogic.EnemyType.Diamond00,
                _ => EnemyLogic.EnemyType.Diamond01
            };

            if (type == EnemyLogic.EnemyType.Invalid)
            {
                return;
            }

            // 敵を生成
            var enemy = ActivateEnemy();
            if (enemy == null)
            {
                return;
            }

            var location = GetRandomSpawnLocation(true, false);
            enemy.Create(type, location, playerLogic);
        }

        /// <summary>
        /// <see cref="EnemyLogic.EnemyCategory.Heart" />を生成する。
        /// </summary>
        /// <param name="playerLogic">プレイヤーのロジック。</param>
        /// <param name="frameCounter">フレーム数のカウンター。</param>
        private void TrySpawnHeart(PlayerLogic playerLogic, int frameCounter)
        {
            var count = frameCounter / EnemySpawnFrameInterval % 8;
            var type = (frameCounter / EnemySpawnFrameInterval / 8) switch
            {
                0 => EnemyLogic.EnemyType.Invalid,
                1 => EnemyLogic.EnemyType.Invalid,
                2 => EnemyLogic.EnemyType.Heart00,
                3 => (count & 0x01) == 0 ? EnemyLogic.EnemyType.Invalid : EnemyLogic.EnemyType.Heart00,
                4 => (count & 0x01) == 0 ? EnemyLogic.EnemyType.Invalid : EnemyLogic.EnemyType.Heart00,
                5 => (count & 0x03) == 0 ? EnemyLogic.EnemyType.Heart01 : EnemyLogic.EnemyType.Heart00,
                6 => (count & 0x03) == 0 ? EnemyLogic.EnemyType.Heart01 : EnemyLogic.EnemyType.Heart00,
                7 => (count & 0x03) == 0 ? EnemyLogic.EnemyType.Heart01 : EnemyLogic.EnemyType.Heart00,
                8 => (count & 0x03) == 0 ? EnemyLogic.EnemyType.Heart01 : EnemyLogic.EnemyType.Heart00,
                9 => (count & 0x03) == 0 ? EnemyLogic.EnemyType.Heart01 : EnemyLogic.EnemyType.Heart00,
                10 => (count & 0x01) == 0 ? EnemyLogic.EnemyType.Heart01 : EnemyLogic.EnemyType.Heart00,
                11 => (count & 0x01) == 0 ? EnemyLogic.EnemyType.Heart01 : EnemyLogic.EnemyType.Heart00,
                12 => (count & 0x01) == 0 ? EnemyLogic.EnemyType.Heart01 : EnemyLogic.EnemyType.Heart00,
                _ => EnemyLogic.EnemyType.Heart01
            };

            if (type == EnemyLogic.EnemyType.Invalid)
            {
                return;
            }

            // 敵を生成
            var enemy = ActivateEnemy();
            if (enemy == null)
            {
                return;
            }

            var location = GetRandomSpawnLocation(true, true);
            enemy.Create(type, location, playerLogic);
        }

        /// <summary>
        /// <see cref="EnemyLogic.EnemyCategory.Spade" />を生成する。
        /// </summary>
        /// <param name="playerLogic">プレイヤーのロジック。</param>
        /// <param name="frameCounter">フレーム数のカウンター。</param>
        private void TrySpawnSpade(PlayerLogic playerLogic, int frameCounter)
        {
            var count = frameCounter / EnemySpawnFrameInterval % 8;
            var type = (frameCounter / EnemySpawnFrameInterval / 8) switch
            {
                0 => EnemyLogic.EnemyType.Invalid,
                1 => EnemyLogic.EnemyType.Invalid,
                2 => EnemyLogic.EnemyType.Invalid,
                3 => (count & 0x07) == 0 ? EnemyLogic.EnemyType.Spade00 : EnemyLogic.EnemyType.Invalid,
                4 => (count & 0x07) == 0 ? EnemyLogic.EnemyType.Spade00 : EnemyLogic.EnemyType.Invalid,
                5 => (count & 0x03) == 0 ? EnemyLogic.EnemyType.Spade00 : EnemyLogic.EnemyType.Invalid,
                6 => (count & 0x03) == 0 ? (count & 0x04) != 0 ? EnemyLogic.EnemyType.Spade00 : EnemyLogic.EnemyType.Spade01 : EnemyLogic.EnemyType.Invalid,
                7 => (count & 0x03) == 0 ? (count & 0x04) != 0 ? EnemyLogic.EnemyType.Spade00 : EnemyLogic.EnemyType.Spade01 : EnemyLogic.EnemyType.Invalid,
                8 => (count & 0x03) != 0 ? (count & 0x06) != 0 ? EnemyLogic.EnemyType.Spade00 : EnemyLogic.EnemyType.Spade01 : EnemyLogic.EnemyType.Invalid,
                9 => (count & 0x03) != 0 ? (count & 0x01) == 0 ? EnemyLogic.EnemyType.Spade00 : EnemyLogic.EnemyType.Spade01 : EnemyLogic.EnemyType.Invalid,
                10 => (count & 0x03) != 0 ? (count & 0x01) == 0 ? EnemyLogic.EnemyType.Spade00 : EnemyLogic.EnemyType.Spade01 : EnemyLogic.EnemyType.Invalid,
                11 => (count & 0x03) != 0 ? (count & 0x01) == 0 ? EnemyLogic.EnemyType.Spade00 : EnemyLogic.EnemyType.Spade01 : EnemyLogic.EnemyType.Invalid,
                12 => (count & 0x01) == 0 ? EnemyLogic.EnemyType.Spade01 : EnemyLogic.EnemyType.Spade00,
                13 => (count & 0x01) == 0 ? EnemyLogic.EnemyType.Spade01 : EnemyLogic.EnemyType.Spade00,
                14 => (count & 0x01) == 0 ? EnemyLogic.EnemyType.Spade01 : EnemyLogic.EnemyType.Spade00,
                _ => EnemyLogic.EnemyType.Spade01
            };

            if (type == EnemyLogic.EnemyType.Invalid)
            {
                return;
            }

            // 敵を生成
            var enemy = ActivateEnemy();
            if (enemy == null)
            {
                return;
            }

            var location = GetRandomSpawnLocation(true, true);
            enemy.Create(type, location, playerLogic);
        }

        /// <summary>
        /// <see cref="EnemyLogic.EnemyCategory.Joker" />を生成する。
        /// </summary>
        /// <param name="playerLogic">プレイヤーのロジック。</param>
        /// <param name="frameCounter">フレーム数のカウンター。</param>
        private void TrySpawnJoker(PlayerLogic playerLogic, int frameCounter)
        {
            var count = frameCounter / EnemySpawnFrameInterval % 8;
            var type = (frameCounter / EnemySpawnFrameInterval / 8) switch
            {
                0 => EnemyLogic.EnemyType.Invalid,
                1 => EnemyLogic.EnemyType.Invalid,
                2 => EnemyLogic.EnemyType.Invalid,
                3 => EnemyLogic.EnemyType.Invalid,
                4 => EnemyLogic.EnemyType.Invalid,
                5 => EnemyLogic.EnemyType.Invalid,
                6 => EnemyLogic.EnemyType.Invalid,
                7 => EnemyLogic.EnemyType.Invalid,
                8 => EnemyLogic.EnemyType.Invalid,
                9 => EnemyLogic.EnemyType.Invalid,
                10 => EnemyLogic.EnemyType.Invalid,
                11 => (count & 0x07) == 0 ? EnemyLogic.EnemyType.Joker : EnemyLogic.EnemyType.Invalid,
                12 => (count & 0x07) == 0 ? EnemyLogic.EnemyType.Joker : EnemyLogic.EnemyType.Invalid,
                13 => (count & 0x03) == 0 ? EnemyLogic.EnemyType.Joker : EnemyLogic.EnemyType.Invalid,
                14 => (count & 0x03) == 0 ? EnemyLogic.EnemyType.Joker : EnemyLogic.EnemyType.Invalid,
                15 => (count & 0x01) == 0 ? EnemyLogic.EnemyType.Joker : EnemyLogic.EnemyType.Invalid,
                16 => (count & 0x01) == 0 ? EnemyLogic.EnemyType.Joker : EnemyLogic.EnemyType.Invalid,
                _ => EnemyLogic.EnemyType.Joker
            };

            if (type == EnemyLogic.EnemyType.Invalid)
            {
                return;
            }

            // 敵を生成
            var enemy = ActivateEnemy();
            if (enemy == null)
            {
                return;
            }

            var location = GetRandomSpawnLocation(true, false);
            enemy.Create(type, location, playerLogic);
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
            var enemy = _inactiveEnemies.First?.Value;
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

        public void InactivateEnemy(EnemyLogic enemy)
        {
            _inactiveEnemies.AddLast(enemy);
            _activeEnemies.Remove(enemy);
            enemy.Destroy();
        }

        #endregion
    }
}
