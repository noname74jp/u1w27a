using System.Collections.Generic;
using UnityEngine;

namespace Game.Logic
{
    /// <summary>
    /// キャラのロジックのベース。
    /// </summary>
    public abstract class CharLogicBase
    {
        #region methods

        /// <summary>
        /// ヒットしている対象を見つける。
        /// </summary>
        /// <param name="logics">判定対象のロジック群。</param>
        /// <typeparam name="T">ロジックの型。</typeparam>
        /// <returns>ヒットしているならそのロジック。していないならnull。</returns>
        public T FindHitTarget<T>(IEnumerable<T> logics) where T : CharLogicBase
        {
            // 距離の2乗がサイズの和の2乗以下ならヒット
            foreach (var logic in logics)
            {
                var totalSize = (_size + logic.Size) * 0.5f; // _sizeは直径で距離判定は半径のため0.5倍
                var sqrMagnitude = (_location - logic.Location).sqrMagnitude;
                if (sqrMagnitude <= totalSize * totalSize)
                {
                    return logic;
                }
            }

            // 何もヒットしていなければnull
            return null;
        }

        #endregion

        #region methods

        /// <summary>
        /// 生成する。
        /// </summary>
        /// <param name="location">初期座標。</param>
        /// <param name="velocity">初期速度。</param>
        /// <param name="size">初期サイズ。</param>
        protected void Create(Vector2 location, Vector2 velocity, float size)
        {
            _location = location;
            _velocity = velocity;
            _size = size;
            _alive = true;
        }

        /// <summary>
        /// 破棄する。
        /// </summary>
        protected void Destroy()
        {
            _alive = false;
        }

        #endregion

        #region variables

        /// <summary>
        /// 位置。
        /// </summary>
        protected Vector2 _location;

        /// <summary>
        /// 速度。
        /// </summary>
        protected Vector2 _velocity;

        /// <summary>
        /// サイズ(直径)[pixel]。
        /// </summary>
        private float _size;

        /// <summary>
        /// 生存しているか。
        /// </summary>
        private bool _alive;

        #endregion

        #region properties

        /// <summary>
        /// 位置。
        /// </summary>
        public Vector2 Location => _location;

        /// <summary>
        /// 速度。
        /// </summary>
        public Vector2 Velocity => _velocity;

        /// <summary>
        /// サイズ(直径)[pixel]。
        /// </summary>
        public float Size => _size;

        /// <summary>
        /// 速度。
        /// </summary>
        public Rect Rect => new(_location.x - _size * 0.5f, _location.y - _size * 0.5f, _size, _size);

        /// <summary>
        /// 生存しているか。
        /// </summary>
        public bool Alive => _alive;

        #endregion
    }
}
