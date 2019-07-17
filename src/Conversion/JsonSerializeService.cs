﻿using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.Text;

namespace Enjaxel.Conversion
{
    /// <summary>
    /// DataContractJsonSerializerのWrapperクラス
    /// </summary>
    public static class JsonSerializeService
    {
        #region Deserialize
        /// <summary>
        /// Stream → T型Entityに変換します
        /// </summary>
        /// <typeparam name="T"> 変換する型 </typeparam>
        /// <param name="stream"> 変換するStream </param>
        /// <returns> T型Entity </returns>
        /// <exception cref="MissingMethodException"></exception>
        public static T JsonDeserialize<T>(Stream stream)
            where T : new()
        {
            var result = default(T);

            try
            {
                // T型インスタンスを作成
                result = Activator.CreateInstance<T>();

                // デシリアライズの準備
                var serializer = new DataContractJsonSerializer(typeof(T));

                // Json文字列をメモリ上に展開
                using (stream)
                {
                    // メモリ上のJsonからT型インスタンスに値を詰め込む
                    result = (T)serializer.ReadObject(stream);
                }
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// Json文字列 → T型Entityに変換します
        /// </summary>
        /// <typeparam name="T"> 変換する型 </typeparam>
        /// <param name="json"> Json文字列 </param>
        /// <param name="encode"> 文字コード </param>
        /// <returns> T型Entity </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="MissingMethodException"></exception>
        /// <exception cref="EncoderFallbackException"></exception>
        public static T JsonDeserialize<T>(string json, Encoding encode)
            where T : new()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json))
                {
                    throw new ArgumentException("文字列はnullか空文字列です。");
                }

                // Json文字列をメモリ上に展開
                using (var stream = new MemoryStream(encode.GetBytes(json)))
                {
                    return JsonDeserialize<T>(stream);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Json文字列 → T型Entityに変換します（UTF-8固定）
        /// </summary>
        /// <typeparam name="T"> 変換する型 </typeparam>
        /// <param name="json"> Json文字列 </param>
        /// <returns> T型Entity </returns>
        /// <remarks> 文字コードはUTF-8 </remarks>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="MissingMethodException"></exception>
        /// <exception cref="EncoderFallbackException"></exception>
        public static T JsonDeserialize<T>(string json)
            where T : new()
        {
            return JsonDeserialize<T>(json, Encoding.UTF8);
        }

        /// <summary>
        /// Jsonテキストファイル → T型Entityに変換します
        /// </summary>
        /// <typeparam name="T"> 変換する型 </typeparam>
        /// <param name="fileInfo"> JsonテキストファイルのFileInfo </param>
        /// <param name="encode"> 文字コード </param>
        /// <returns> T型Entity </returns>
        /// <remarks> 戻り値はNullの可能性あり </remarks>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="MissingMethodException"></exception>
        public static T JsonDeserialize<T>(FileInfo fileInfo, Encoding encode)
            where T : new()
        {
            try
            {
                if (!fileInfo.Exists)
                {
                    throw new FileNotFoundException("対象のファイルが存在しません。");
                }

                // Json文字列をメモリ上に展開
                using (var sr = new StreamReader(fileInfo.FullName, encode))
                {
                    return JsonDeserialize<T>(sr.BaseStream);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Jsonテキストファイル → T型Entityに変換します（UTF-8固定）
        /// </summary>
        /// <typeparam name="T"> 変換する型 </typeparam>
        /// <param name="fileInfo"> JsonテキストファイルのFileInfo </param>
        /// <returns> T型Entity </returns>
        /// <remarks> 文字コードはUTF-8 </remarks>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="MissingMethodException"></exception>
        public static T JsonDeserialize<T>(FileInfo fileInfo)
            where T : new()
        {
            return JsonDeserialize<T>(fileInfo, Encoding.UTF8);
        }
        #endregion

        #region Serialize
        /// <summary>
        /// T型Entity → Json文字列に変換するメソッド
        /// </summary>
        /// <param name="target"> T型Entity </param>
        /// <param name="encode"> 文字コード </param>
        /// <returns> Json文字列 </returns>
        /// <remarks> エラーの場合は空文字 </remarks>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidDataContractException"></exception>
        /// <exception cref="SerializationException"></exception>
        /// <exception cref="QuotaExceededException"></exception>
        /// <exception cref="DecoderFallbackException"></exception>
        public static string JsonSerialize(object target, Encoding encode)
        {
            string result = string.Empty;

            try
            {
                using (var stream = new MemoryStream())
                {
                    // シリアライズの準備
                    var serializer = new DataContractJsonSerializer(target.GetType());
                    // T型オブジェクトをメモリストリームへ展開
                    serializer.WriteObject(stream, target);
                    // 結果を取得
                    result = encode.GetString(stream.ToArray());
                }
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// T型Entity → Json文字列に変換するメソッド（UTF-8固定）
        /// </summary>
        /// <param name="target"> T型Entity </param>
        /// <returns> Json文字列 </returns>
        /// <remarks> エラーの場合は空文字 </remarks>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidDataContractException"></exception>
        /// <exception cref="SerializationException"></exception>
        /// <exception cref="QuotaExceededException"></exception>
        /// <exception cref="DecoderFallbackException"></exception>
        public static string JsonSerialize(object target)
        {
            return JsonSerialize(target, Encoding.UTF8);
        }
        #endregion
    }
}
