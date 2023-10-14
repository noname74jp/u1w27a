using System.Reflection;
using UnityEngine;
using unityroom.Api;

namespace Library
{
    /// <summary>
    /// unityroomのAPIキーを隠蔽するためのコンポーネント。
    /// </summary>
    /// <remarks>DefaultExecutionOrder で <see cref="UnityroomApiClient.Awake" /> よりも前に <see cref="UnityroomApiClient.HmacKey" /> に値を設定する。</remarks>
    /// <remarks>正攻法で対応するならビルド時に設定するほうが良い。</remarks>
    [DefaultExecutionOrder(-1)]
    public class UnityroomApiKey : MonoBehaviour
    {
        #region variables

        /// <summary>
        /// APIキー。
        /// </summary>
        [SerializeField] private TextAsset hmacKeyTextAsset;

        #endregion

        #region methods

        /// <summary>
        /// Unityイベント関数Awake。
        /// </summary>
        private void Awake()
        {
            // テキストアセットがnullなら抜ける
            if (hmacKeyTextAsset == null)
            {
                return;
            }

            // UnityroomApiClientを取得し、nullなら抜ける
            var client = GetComponent<UnityroomApiClient>();
            if (client == null)
            {
                return;
            }

            // HmacKeyを設定
            // privateなためリフレクションでアクセス
            var hmacKey = hmacKeyTextAsset.text;
            var type = client.GetType();
            var clientHmacKeyFieldInfo = type.GetField("HmacKey", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.SetField);
            clientHmacKeyFieldInfo?.SetValue(client, hmacKey);
        }

        #endregion
    }
}
