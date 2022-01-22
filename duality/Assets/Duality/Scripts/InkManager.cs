using UnityEngine;
using UnityEngine.UI;
using System;
using Ink.Runtime;

public class InkManager : MonoBehaviour
{
    // called before Start()
    void Awake ()
    {
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
        // clear whatever we may have had from before
        RemoveChildren();

        if (!_story.canContinue) { return; }

        // get next line and trim whitespace off
        string text = _story.Continue();
        text = text?.Trim();

        // display new text
        CreateContentView(text);

        // create "next" button
        var button = CreateChoiceView("next");
        button.onClick.AddListener(delegate {
            DisplayNextLine();
        });
        // make sure they're not overlapping -- cheat button downwards
        //button.transform.position = new Vector3(0, -10, 0);
        var buttonRectTransform = button.GetComponent<RectTransform>();
        buttonRectTransform.localPosition += Vector3.down * 100;
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
        Button choice = Instantiate(buttonPrefab) as Button;
        choice.transform.SetParent(canvas.transform, false);

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
