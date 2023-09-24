using UnityEngine;

namespace Game.Logic
{
    public class LogicBase
    {
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
        public void Destroy()
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
        /// サイズ。
        /// </summary>
        protected float _size;

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
        /// サイズ。
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