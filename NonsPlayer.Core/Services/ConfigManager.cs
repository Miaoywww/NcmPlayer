﻿using System.IO;
using System.Text;
using System.Text.Json;
using NonsPlayer.Core.Resources;

namespace NonsPlayer.Core.Services;

public class ConfigManager
{
    #region 事件注册

    public delegate void ConfigManagerEventHandler(string param);

    public event ConfigManagerEventHandler ConfigLoadFailed;
    #endregion
    public static ConfigManager Instance { get; } = new();
    public LocalSettings Settings;
    private Dictionary<string, object?> otherSettings;

    public ConfigManager()
    {
        Settings = new();
        otherSettings = new();
    }

    public void Load()
    {
        try
        {
            if (File.Exists(Settings.ConfigFilePath))
            {
                var json = File.ReadAllText(Settings.ConfigFilePath);
                Settings = JsonSerializer.Deserialize<LocalSettings>(json);
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Settings.ConfigFilePath));
                File.Create(Settings.ConfigFilePath).Close();
            }

            if (File.Exists(Settings.OtherConfigFilePath))
            {
                var otherJson = File.ReadAllText(Settings.OtherConfigFilePath);
                otherSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(otherJson);
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Settings.OtherConfigFilePath));
                File.Create(Settings.OtherConfigFilePath).Close();
            }
        }
        catch(Exception e)
        {
            ConfigLoadFailed?.Invoke(e.ToString());
        }

    }

    public void Save()
    {
        if (!File.Exists(Settings.ConfigFilePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Settings.ConfigFilePath));
            File.Create(Settings.ConfigFilePath).Close();
        }

        if (!File.Exists(Settings.OtherConfigFilePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Settings.OtherConfigFilePath));
            File.Create(Settings.OtherConfigFilePath).Close();
        }

        var options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        var json = JsonSerializer.Serialize(Settings, options);
        var otherJson = JsonSerializer.Serialize(otherSettings, options);
        File.WriteAllText(Settings.ConfigFilePath, json, Encoding.UTF8);
        File.WriteAllText(Settings.OtherConfigFilePath, otherJson, Encoding.UTF8);
    }

    /// <summary>
    /// 设置一个新的配置
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetConfig<T>(string key, T? value)
    {
        if (otherSettings.ContainsKey(key))
        {
            otherSettings[key] = value;
            return;
        }

        otherSettings.Add(key, value);
    }

    public bool TryGetConfig<T>(string key, out T? value) where T : class
    {
        if (otherSettings.TryGetValue(key, out object result))
        {
            value = result as T;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGetConfig(string key ,out string? value)
    {
        if (otherSettings.TryGetValue(key, out object result))
        {
            value = result as string;
            if (value != null) return true;
            try
            {
                value = ((System.Text.Json.JsonElement)result).ToString();
            }
            catch
            {
                return false;
            }
            return true;
        }

        value = default;
        return false;
    }
}