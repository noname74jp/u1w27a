using System.Collections.Generic;
using UnityEngine;

namespace Game.UnityGameObject.Char
{
    /// <summary>
    /// 敵管理。
    /// </summary>
    public class EnemyCoordinator : MonoBehaviour
    {
        #region properties

        /// <summary>
        /// 敵のリスト。
        /// </summary>
        public IReadOnlyList<Enemy> Enemies => _enemies;

        #endregion

        #region methods

        /// <summary>
        /// 初期化する。
        /// </summary>
        /// <param name="enemyCount">敵数。</param>
        public void Initialize(int enemyCount)
        {
            _enemies = new List<Enemy>(enemyCount);
            for (var i = 0; i < enemyCount; i++)
            {
                var enemy = Instantiate(enemyPrefab, transform);
                _enemies.Add(enemy);
            }
        }

        #endregion

        #region variables

        /// <summary>
        /// 敵のプレハブ。
        /// </summary>
        [SerializeField] private Enemy enemyPrefab;

        /// <summary>
        /// 敵のリスト。
        /// </summary>
        private List<Enemy> _enemies = new();

        #endregion
    }
}
