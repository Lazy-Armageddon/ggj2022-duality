using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

public class DialogManager : MonoBehaviour
{
    // called before Start()
    void Awake()
    {
    }

    void Start()
    {
        // clear whatever we may have had from before
        RemoveChildren();
    }

    public void OnChoices(List<Ink.Runtime.Choice> choices)
    {
        // clear whatever we may have had from before
        RemoveChildren();

        // display all the choices
        for (int i = 0; i < choices.Count; i++)
        {
            Ink.Runtime.Choice choice = choices[i];
            Button button = CreateChoiceView(choice.text.Trim());

            // Tell the button what to do when we press it
            button.onClick.AddListener(delegate {
                //OnClickChoiceButton(choice);
                if (OnSelectChoice != null)
                {
                    OnSelectChoice(choice);
                }
            });

            // cheat button downwards to help keep it from overlapping the others
            var buttonRectTransform = button.GetComponent<RectTransform>();
            buttonRectTransform.localPosition += Vector3.down * i * 30;
        }
    }

    public void OnTextLine(string text)
    {
        // clear whatever we may have had from before
        RemoveChildren();

        // display new text
        CreateContentView(text);

        // create "next" button
        var button = CreateChoiceView("next");
        button.onClick.AddListener(delegate {
            AdvanceStory();
        });

        // cheat button downwards to help keep it from overlapping text
        var buttonRectTransform = button.GetComponent<RectTransform>();
        buttonRectTransform.localPosition += Vector3.down * 150;
    }

    // Creates a textbox showing the the line of text
    void CreateContentView(string text)
    {
        Text storyText = Instantiate(textPrefab) as Text;
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
            GameObject.Destroy(canvas.transform.GetChild (i).gameObject);
        }
    }

    void AdvanceStory()
    {
        if (OnAdvanceStory != null)
        {
            OnAdvanceStory();
        }
    }

    public event Action OnAdvanceStory;
    public event Action<Ink.Runtime.Choice> OnSelectChoice;

    [SerializeField]
    private Canvas canvas = null;

    // UI Prefabs
    [SerializeField]
    private Text textPrefab = null;
    [SerializeField]
    private Button buttonPrefab = null;
}
