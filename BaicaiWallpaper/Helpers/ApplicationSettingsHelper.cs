using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaicaiWallpaper.Helpers
{
    /// <summary>
    /// 保存数据到本地
    /// </summary>
    public class ApplicationSettingsHelper
    {
        private static readonly IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        /// <summary>
        /// 添加或更新数据
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">值</param>
        public static void AddOrUpdateValue(string key, object value)
        {
            bool valueChanged = false;
            if (settings.Contains(key))
            {
                if (settings[key] != null)
                {
                    settings[key] = value;
                    valueChanged = true;
                }
            }
            else
            {
                settings.Add(key, value);
                valueChanged = true;
            }
            if (valueChanged)
            {
                Save();
            }
        }
        /// <summary>
        /// 获取指定的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回指定类型的值</returns>
        public static T GetValueOrDefault<T>(string key, T defaultValue)
        {
            T value;
            try
            {
                if (settings.Contains(key))
                {
                    value = (T)settings[key];
                }
                else
                {
                    value = defaultValue;
                }

                return value;
            }
            catch
            {
                return defaultValue;
            }
        }
        private static void Save()
        {
            settings.Save();
        }

        public static void Clear()
        {
            settings.Clear();
        }
    }
}
