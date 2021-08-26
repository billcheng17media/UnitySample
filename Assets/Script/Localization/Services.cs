using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

public class Localization
{
    public enum Language
    {
        en,
        zh_tw,
        jp
    }

    public Dictionary<string, SimpleJSON.JSONNode> LocalizeRecordDictionary;

    // Default english
    public Language CurrentLanguage = Language.en;

    private SimpleJSON.JSONNode languageKeyAndValues;

    public Localization()
    {
        // url to localization json file
        //TODO: this will not be used for now, only local loads
        //string url = MainController.Instance.LocalizationFileURL;

        //Initialize(url, CurrentLanguage);
        LocalizeRecordDictionary = new Dictionary<string, SimpleJSON.JSONNode>();
    }

    public void AddLocalizationFile(string fileName, bool isLocal = true)
    {
        if (isLocal)
        {
            InitializeLocal(fileName, CurrentLanguage);
        }
    }

    public void ChangeLocalizeLanguage(string fileName, Language language)
    {
        InitializeLocal(fileName, language);
    }

    public void AsyncInitializeWithURL(string url, Language language = Language.en)
    {
        WebClient webClient = new WebClient();
        Uri uri = new Uri(url);

        webClient.DownloadStringAsync(uri);
        webClient.DownloadStringCompleted += delegate (System.Object sender, DownloadStringCompletedEventArgs e) {
            Debug.Log($"Download complete: {e}, {sender}");
            DeserializeTextIntoLanguageRecords(e.Result, language);
	    };
    }

    private void DeserializeTextIntoLanguageRecords(string jsonString, Language language)
    {
        string languageKey = GetLanguageKey(language);

        var vals = SimpleJSON.JSON.Parse(jsonString);
        var locales = vals["locales"];
        var langKeys = locales[languageKey];

        languageKeyAndValues = langKeys["keys"];
    }

    private void InitializeLocal(string fileName, Language language = Language.en)
    {
        // Should download assets for now get it from local file
        TextAsset text = Resources.Load<TextAsset>($"Localization/localization_{fileName}");

        var vals = SimpleJSON.JSON.Parse(text.text);
        var locales = vals["locales"];
       
        var values = JsonUtility.FromJson<Dictionary<string, Dictionary<string, Dictionary<string, object>>>>(text.text);
        
        LocalizeRecordDictionary.Add(fileName, locales);
    }

    public string GetLanguageKey(Language language)
    {
        return Enum.GetName(typeof(Language), language);
    }

    /// <summary>
    /// This will add a localization record to be use to retried localization keys by record key.
    /// </summary>
    /// <param name="key"></param>
    public void AddLocalizationRecord(string recordKey)
    {
    }

    public string LocalizationString(string fileName, string key)
    {
        string translation = GetTranslationForKey(fileName, key);
        return translation;
    }

    public string GetTranslationForKey(string fileName, string key)
    {
        SimpleJSON.JSONNode locales = null;
        LocalizeRecordDictionary.TryGetValue(fileName, out locales);

        if (locales == null)
        {
            InitializeLocal(fileName, CurrentLanguage);
            LocalizeRecordDictionary.TryGetValue(fileName, out locales);
        }

        if (locales == null)
        {
            Debug.LogWarning($"Could not find locale filename loaded: {fileName} requested key: {key}. current language selected: {CurrentLanguage}");
            return key;
        }

        var localeStringKey = GetLanguageKey(CurrentLanguage);
        var localesDictionary = locales[localeStringKey];
        var langKeys = localesDictionary["keys"];

        string value = langKeys[key];

        if (value != null)
        {
            return value;
        }
        else {
            Debug.LogWarning($"Could not find translation for key: {key}. current language selected: {CurrentLanguage}");
        }

        // last chance is using key as the default translation
        return key;
    }
}

public sealed class Services
{
    public static Localization Localization { get; private set; }

    static Services()
    {
        Localization = new Localization();
    }
}
