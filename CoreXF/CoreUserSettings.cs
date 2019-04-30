
using Plugin.Settings;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Plugin.SecureStorage;
using System;

namespace CoreXF
{
    public class CoreUserSettings : ObservableObject
    {
        static public string LastLogin { get => GetString(); set => SetString(value); }
        static public string LastPasswordSecure { get => GetStringSecure(); set => SetStringSecure(value); }

        static public bool IsOnboardingShowed { get => GetBool(); set => SetBool(value); }

        static public string FirebaseToken { get => GetString(); set => SetString(value); }
        static public string FirebaseTokenPrevious { get => GetString(); set => SetString(value); }
        static public string AppUUID { get => GetString(); set => SetString(value); }
        static public string PreferredLanguage { get => GetString(); set => SetString(value); }
        static public int CurrentAppVersion { get => GetInt(); set => SetInt(value); }

        static public string AccessToken { get => GetString(); set => SetString(value); }
        static public DateTime AccessTokenUpdateDate { get => GetDateTime(); set => SetDateTime(value); }

        static public int GetInt(int defaultValue = 0, [CallerMemberName] string propertyName = null) =>
            CrossSettings.Current.GetValueOrDefault(propertyName, defaultValue);

        static public void SetInt(int value, [CallerMemberName] string propertyName = null) =>
            CrossSettings.Current.AddOrUpdateValue(propertyName, value);

        public void SetIntAndRaisePropertyChanged(int value, [CallerMemberName] string propertyName = null)
        {
            SetInt(value, propertyName);
            RaisePropertyChanged(propertyName);
        }


        static public bool GetBool(bool defaultValue = false, [CallerMemberName] string propertyName = null) =>
            CrossSettings.Current.GetValueOrDefault(propertyName, defaultValue);

        static public void SetBool(bool value, [CallerMemberName] string propertyName = null) =>
            CrossSettings.Current.AddOrUpdateValue(propertyName, value);

        static public string GetString(string defaultValue = null, [CallerMemberName] string propertyName = null) =>
            CrossSettings.Current.GetValueOrDefault(propertyName, defaultValue);

        static public void SetString(string value, [CallerMemberName] string propertyName = null) =>
            CrossSettings.Current.AddOrUpdateValue(propertyName, value);

        static public DateTime GetDateTime(DateTime defaultValue = default(DateTime), [CallerMemberName] string propertyName = null) =>
            CrossSettings.Current.GetValueOrDefault(propertyName, defaultValue);

        static public void SetDateTime(DateTime value, [CallerMemberName] string propertyName = null) =>
            CrossSettings.Current.AddOrUpdateValue(propertyName, value);

        public void SetDateTimeAndRaisePropertyChanged(DateTime value, [CallerMemberName] string propertyName = null)
        {
            SetDateTime(value, propertyName);
            RaisePropertyChanged(propertyName);
        }

        public void SetStringAndRaisePropertyChanged(string value, [CallerMemberName] string propertyName = null)
        {
            SetString(value, propertyName);
            RaisePropertyChanged(propertyName);
        }

        static public string GetStringSecure(string defaultValue = null, [CallerMemberName] string propertyName = null) =>
            CrossSecureStorage.Current.GetValue(propertyName) ?? defaultValue;

        static public void SetStringSecure(string value, [CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (CrossSecureStorage.Current.HasKey(propertyName))
                {
                    CrossSecureStorage.Current.DeleteKey(propertyName);
                }
            }
            else
            {
                CrossSecureStorage.Current.SetValue(propertyName, value);
            }
        }
            

        static T Get<T>([CallerMemberName] string propertyName = null) where T : class, new()
        {
            string valAsString = CrossSettings.Current.GetValueOrDefault(propertyName, null);
            if (valAsString == null) return null;
            T val = JsonConvert.DeserializeObject<T>(valAsString);
            return val;
        }

        static void Set<T>(T value, [CallerMemberName] string propertyName = null) where T : class, new()
        {
            string valAsString = value == null ? null : JsonConvert.SerializeObject(value);
            CrossSettings.Current.AddOrUpdateValue(propertyName, valAsString);
        }
    }
}
