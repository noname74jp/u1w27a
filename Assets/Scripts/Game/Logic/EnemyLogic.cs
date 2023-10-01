using System;
using UnityEngine;

namespace Game.Logic
{
    /// <summary>
    /// 敵のロジック。
    /// </summary>
    public class EnemyLogic : CharLogicBase
    {
        #region Properties

        /// <summary>
        /// 敵のカテゴリー。
        /// </summary>
        public EnemyCategory Category { get; private set; }

        /// <summary>
        /// 敵の種類。
        /// </summary>
        private EnemyType Type { get; set; }

        /// <summary>
        /// 敵のライフ。
        /// </summary>
        private int Life { get; set; }

        /// <summary>
        /// 敵の角度。
        /// </summary>
        public float Angle { get; private set; }

        /// <summary>
        /// 敵の角度。
        /// </summary>
        public int Score { get; private set; }

        #endregion

        #region enums

        /// <summary>
        /// 敵のカテゴリー。
        /// </summary>
        public enum EnemyCategory
        {
            /// <summary>
            /// クラブ。横方向に等速直線移動。
            /// </summary>
            Club,

            /// <summary>
            /// ダイヤ。縦方向に等速直線移動。
            /// </summary>
            Diamond,

            /// <summary>
            /// ハート。プレイヤーにじりじり寄ってくる。
            /// </summary>
            Heart,

            /// <summary>
            /// スペード。プレイヤーとの角度を見て角度を変えて寄ってくる。
            /// </summary>
            Spade,

            /// <summary>
            /// ジョーカー。任意角度で侵入し、壁て反射。
            /// </summary>
            Joker
        }

        /// <summary>
        /// 敵の種類。
        /// </summary>
        public enum EnemyType
        {
            /// <summary>
            /// 横方向に直線運動。
            /// </summary>
            Club00,

            /// <summary>
            /// 横方向に直線運動。大きい。
            /// </summary>
            Club01,

            /// <summary>
            /// 縦方向に直線運動。
            /// </summary>
            Diamond00,

            /// <summary>
            /// 縦方向に直線運動。大きい。
            /// </summary>
            Diamond01,

            /// <summary>
            /// プレイヤーにじりじり寄ってくる。
            /// </summary>
            Heart00,

            /// <summary>
            /// プレイヤーにじりじり寄ってくる。大きい。
            /// </summary>
            Heart01,

            /// <summary>
            /// スペード。プレイヤーとの角度を見て角度を変えて寄ってくる。
            /// </summary>
            Spade00,

            /// <summary>
            /// スペード。プレイヤーとの角度を見て角度を変えて寄ってくる。
            /// </summary>
            Spade01,

            /// <summary>
            /// ジョーカー。任意角度で侵入し、壁て反射。
            /// </summary>
            Joker
        }

        #endregion

        #region constants

        /// <summary>
        /// 小さいサイズ。
        /// </summary>
        private const float SizeSmall = 20.0f;

        /// <summary>
        /// 大きいサイズ。
        /// </summary>
        private const float SizeLarge = 30.0f;

        /// <summary>
        /// 遅い速度。
        /// </summary>
        private const float SpeedSlow = 40.0f;

        /// <summary>
        /// 普通の速度。
        /// </summary>
        private const float SpeedNormal = 80.0f;

        /// <summary>
        /// 速い速度。
        /// </summary>
        private const float SpeedFast = 120.0f;

        /// <summary>
        /// とても速い速度。
        /// </summary>
        private const float SpeedVeryFast = 160.0f;

        #endregion

        #region methods

        /// <summary>
        /// 生成する。
        /// </summary>
        /// <param name="enemyType">敵の種類。</param>
        /// <param name="location">初期座標。</param>
        /// <param name="player">プレイヤー。</param>
        public void Create(EnemyType enemyType, Vector2 location, PlayerLogic player)
        {
            Type = enemyType;
            switch (enemyType)
            {
                case EnemyType.Club00:
                case EnemyType.Club01:
                {
                    var velocity = new Vector2(location.x > 0.0f ? -SpeedNormal : SpeedNormal, 0.0f);
                    var size = enemyType == EnemyType.Club00 ? SizeSmall : SizeLarge;
                    base.Create(location, velocity, size);
                    Category = EnemyCategory.Club;
                    Life = enemyType == EnemyType.Club00 ? 1 : 2;
                    Score = 1000;
                    Angle = GetAngle(velocity);
                    break;
                }
                case EnemyType.Diamond00:
                case EnemyType.Diamond01:
                {
                    var velocity = new Vector2(0.0f, location.y > 0.0f ? -SpeedNormal : SpeedNormal);
                    var size = enemyType == EnemyType.Diamond00 ? SizeSmall : SizeLarge;
                    base.Create(location, velocity, size);
                    Category = EnemyCategory.Diamond;
                    Life = enemyType == EnemyType.Diamond00 ? 1 : 2;
                    Score = 1500;
                    Angle = GetAngle(velocity);
                    break;
                }
                case EnemyType.Heart00:
                case EnemyType.Heart01:
                {
                    var size = enemyType == EnemyType.Heart00 ? SizeSmall : SizeLarge;
                    base.Create(location, Vector2.zero, size);
                    Category = EnemyCategory.Heart;
                    Life = enemyType == EnemyType.Heart00 ? 2 : 3;
                    Score = 2000;
                    Angle = 0.0f;
                    break;
                }
                case EnemyType.Spade00:
                case EnemyType.Spade01:
                {
                    var speed = enemyType == EnemyType.Spade00 ? SpeedNormal : SpeedFast;
                    var velocity = (player.Location - location).normalized * speed;
                    var size = enemyType == EnemyType.Spade00 ? SizeSmall : SizeLarge;
                    base.Create(location, velocity, size);
                    Category = EnemyCategory.Spade;
                    Life = enemyType == EnemyType.Spade00 ? 2 : 3;
                    Score = 3000;
                    Angle = GetAngle(velocity);
                    break;
                }
                case EnemyType.Joker:
                {
                    var velocity = (player.Location - location).normalized * SpeedVeryFast;
                    base.Create(location, velocity, SizeLarge);
                    Category = EnemyCategory.Joker;
                    Life = 4;
                    Score = 5000;
                    Angle = 0.0f;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(enemyType), enemyType, null);
            }
        }

        /// <summary>
        /// 更新する。
        /// </summary>
        /// <param name="player">プレイヤー。</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void UpdateStatus(PlayerLogic player)
        {
            // 生存していなければ何もしない
            if (!Alive)
            {
                return;
            }

            // カテゴリー別処理
            switch (Category)
            {
                case EnemyCategory.Club:
                case EnemyCategory.Diamond:
                {
                    // 等速直線運動で移動
                    _location += _velocity * (float)Defines.SecondsPerFrame;

                    // 領域外に出たら破棄
                    if (!Rect.Overlaps(Defines.EnemyValidArea))
                    {
                        Destroy();
                    }

                    break;
                }
                case EnemyCategory.Heart:
                {
                    // プレイヤーの方向に移動
                    _velocity = GetVelocityToPlayer(_location, player, SpeedSlow);
                    _location += _velocity * (float)Defines.SecondsPerFrame;
                    break;
                }
                case EnemyCategory.Spade:
                {
                    // 速度を変更して移動
                    var speed = Type == EnemyType.Spade00 ? SpeedNormal : SpeedFast;
                    _velocity += (player.Location - _location).normalized * 0.5f;
                    _velocity = _velocity.normalized * speed;
                    _location += _velocity * (float)Defines.SecondsPerFrame;

                    // 移動方向に回転
                    Angle = GetAngle(_velocity);
                    break;
                }
                case EnemyCategory.Joker:
                {
                    // 等速直線運動で移動
                    _location += _velocity * (float)Defines.SecondsPerFrame;

                    // プレイヤーの領域外に出たらプレイヤーの方向に移動
                    if (!Rect.Overlaps(Defines.EnemyValidArea))
                    {
                        _velocity = GetVelocityToPlayer(_location, player, SpeedVeryFast);
                    }

                    // 回転
                    Angle = Mathf.Repeat(Angle + 3.0f, 360.0f);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(Category), Category, null);
            }
        }

        /// <summary>
        /// ダメージを与える。
        /// </summary>
        /// <returns>ダメージを与えた結果、ライフが0以下になったらtrue。</returns>
        public bool AddDamage()
        {
            Life--;
            return Life <= 0;
        }

        /// <summary>
        /// プレイヤーの方向の速度を取得する。
        /// </summary>
        /// <param name="location">位置。</param>
        /// <param name="player">プレイヤー。</param>
        /// <param name="speed">スピード[pixel/frame]</param>
        /// <returns>速度。</returns>
        private static Vector2 GetVelocityToPlayer(Vector2 location, PlayerLogic player, float speed)
        {
            return (player.Location - location).normalized * speed;
        }

        /// <summary>
        /// 指定方向の角度を取得する。
        /// </summary>
        /// <param name="direction">角度を取得する方向。</param>
        /// <returns>角度。</returns>
        private static float GetAngle(Vector2 direction)
        {
            return Vector2.Angle(direction, Vector2.up) * (direction.x >= 0.0f ? -1.0f : 1.0f);
        }

        #endregion
    }
}
