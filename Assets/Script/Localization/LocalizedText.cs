using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TextWrapper
{
    private object _textObject;

    public enum TextType
    {
        pro,
        normal
    }

    public TextType Type { get; private set; }

    public TextWrapper(object textObject)
    {
        _textObject = textObject;

        var objectType = textObject.GetType();

        if(objectType == typeof(TextMeshProUGUI))
        {
            Type = TextType.pro;
        }

        if(objectType == typeof(Text))
        {
            Type = TextType.normal;
        }
    }

    public void SetText(string text)
    {
        if(_textObject == null)
        {
            Debug.LogWarning($"Localize text fail trying to load text for {text}, {_textObject}");
            return;
        }

        switch(Type)
        {
            case TextType.pro:
                    var obj = (TextMeshProUGUI)_textObject;
                    obj.text = text;
                break;

                case TextType.normal:
                    var obj2 = (Text)_textObject;
                    obj2.text = text;
                break;
        }
    }
}

public class LocalizedText : MonoBehaviour
{
    private TextMeshProUGUI _textField;
    public string StringTranslationKey;

    private TextWrapper _textFieldWrapped;

    public string LocalizationKey;

    //[ExecuteInEditMode]
    private void Awake()
    {
        GetTextFieldRef();
        RefreshLocalizedStringValue();
    }

    private void GetTextFieldRef()
    {
        object textField = gameObject.GetComponent<TextMeshProUGUI>();

        if(textField == null)
        {
            textField = gameObject.GetComponent<Text>();
        }

        _textFieldWrapped = new TextWrapper(textField);
    }

    private void OnEnable()
    {
       GetTextFieldRef();
        //RefreshLocalizedStringValue();
    }

    [ExecuteInEditMode]
    private void OnValidate()
    {
        GetTextFieldRef();
        RefreshLocalizedStringValue();
    }

    private void RefreshLocalizedStringValue()
    {
        string value = Services.Localization.GetTranslationForKey(LocalizationKey, StringTranslationKey);
        _textFieldWrapped.SetText(value);
    }

    // Start is called before the first frame update
    void Start()
    {
        //RefreshLocalizedStringValue();
    }
}
