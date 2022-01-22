using UnityEngine;
using UnityEngine.UI;
using System;
using Ink.Runtime;

public class InkManager : MonoBehaviour
{
    // called before Start()
    void Awake () {
        // Remove the default message
        RemoveChildren();
    }

    void Start()
    {
        StartStory();
    }

    private void StartStory()
    {
        _story = new Story(_inkJsonAsset.text);
        if (OnCreateStory != null)
        {
            OnCreateStory(_story);
        }

        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (!_story.canContinue) { return; }

        string text = _story.Continue(); // gets next line
        text = text?.Trim(); // removes white space from text

        CreateContentView(text); // displays new text
    }

    // Creates a textbox showing the the line of text
    void CreateContentView(string text)
    {
        Text storyText = Instantiate (textPrefab) as Text;
        storyText.text = text;
        storyText.transform.SetParent(canvas.transform, false);
    }

    // Creates a button showing the choice text
    Button CreateChoiceView(string text)
    {
        // Creates the button from a prefab
        Button choice = Instantiate (buttonPrefab) as Button;
        choice.transform.SetParent (canvas.transform, false);

        // Gets the text from the button prefab
        Text choiceText = choice.GetComponentInChildren<Text>();
        choiceText.text = text;

        // Make the button expand to fit the text
        HorizontalLayoutGroup layoutGroup = choice.GetComponent<HorizontalLayoutGroup>();
        layoutGroup.childForceExpandHeight = false;

        return choice;
    }

    // Destroys all the children of this gameobject (all the UI)
    void RemoveChildren()
    {
        int childCount = canvas.transform.childCount;
        for (int i = childCount - 1; i >= 0; --i)
        {
            GameObject.Destroy (canvas.transform.GetChild (i).gameObject);
        }
    }

    public static event Action<Story> OnCreateStory;

    [SerializeField]
    private TextAsset _inkJsonAsset = null;
    private Story _story;

    [SerializeField]
    private Canvas canvas = null;

    // UI Prefabs
    [SerializeField]
    private Text textPrefab = null;
    [SerializeField]
    private Button buttonPrefab = null;
}
