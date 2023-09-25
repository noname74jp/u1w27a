using System.Collections.Generic;
using UnityEngine;

namespace Game.UnityGameObject.Char
{
    /// <summary>
    /// 弾管理
    /// </summary>
    public class BulletCoordinator : MonoBehaviour
    {
        #region properties

        /// <summary>
        /// 弾丸のリスト。
        /// </summary>
        public IReadOnlyList<Bullet> Bullets => _bullets;

        #endregion

        #region methods

        /// <summary>
        /// 初期化する。
        /// </summary>
        /// <param name="bulletCount">弾数。</param>
        public void Initialize(int bulletCount)
        {
            _bullets = new List<Bullet>(bulletCount);
            for (var i = 0; i < bulletCount; i++)
            {
                var bullet = Instantiate(bulletPrefab, transform);
                _bullets.Add(bullet);
            }
        }

        #endregion

        #region variables

        /// <summary>
        /// 弾丸のプレハブ。
        /// </summary>
        [SerializeField] private Bullet bulletPrefab;

        /// <summary>
        /// 弾丸のリスト。
        /// </summary>
        private List<Bullet> _bullets = new();

        #endregion
    }
}
