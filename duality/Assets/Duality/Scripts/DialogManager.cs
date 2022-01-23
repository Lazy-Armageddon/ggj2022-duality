using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
//using System.Diagnostics;

public class DialogManager : MonoBehaviour
{
    bool debug = false;

    void DebugMsg(string msg)
    {
        if (debug)
        {
            Debug.Log(msg);
        }
    }

    // called before Start()
    void Awake()
    {
        // find panels with names by convention
        goodPanel = GameObject.Find("Good Panel");
        badPanel = GameObject.Find("Bad Panel");

        // warn about those that could not be find
        if (!goodPanel) { Debug.Log("warning: could not find 'Good Panel'"); }
        if (!badPanel) { Debug.Log("warning: could not find 'Bad Panel'"); }
    }

    void Start()
    {
        // note: do NOT clear scene elements here! (May have already started a story...)
    }

    public void OnChoices(List<Ink.Runtime.Choice> choices)
    {
        DebugMsg("DialogManager.OnChoices(...)");

        // clear whatever we may have had from before
        //RemoveChildren(canvas);
        RemoveChildren(goodPanel);
        RemoveChildren(badPanel);

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
        DebugMsg("DialogManager.OnTextLine(" + text + ")");

        // clear whatever we may have had from before
        //RemoveChildren(canvas);
        RemoveChildren(goodPanel);
        RemoveChildren(badPanel);

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
        DebugMsg("DialogManager.CreateContentView(" + text + ")");

        Text storyText = Instantiate(textPrefab) as Text;
        storyText.text = text;
        //storyText.transform.SetParent(canvas.transform, false);

        // default to good panel as target
        GameObject target = goodPanel;
        if (text.StartsWith("Alice"))
        {
            target = badPanel;
        }
        storyText.transform.SetParent(target.transform, false);
    }

    // Creates a button showing the choice text
    Button CreateChoiceView(string text)
    {
        DebugMsg("DialogManager.CreateChoiceView(" + text + ")");

        // Creates the button from a prefab
        Button choice = Instantiate(buttonPrefab) as Button;
        //GameObject target = canvas;
        GameObject target = goodPanel;
        choice.transform.SetParent(target.transform, false);

        // Gets the text from the button prefab
        Text choiceText = choice.GetComponentInChildren<Text>();
        choiceText.text = text;

        // Make the button expand to fit the text
        HorizontalLayoutGroup layoutGroup = choice.GetComponent<HorizontalLayoutGroup>();
        layoutGroup.childForceExpandHeight = false;

        return choice;
    }

    // Destroys all the children of this gameobject (all the UI)
    void RemoveChildren(GameObject target)
    {
        if (target == null)
        {
            // just don't for now
            Debug.Log("warning: bailing on DialogManager.RemoveChildren()...");
            return;
        }

        int childCount = target.transform.childCount;
        for (int i = childCount - 1; i >= 0; --i)
        {
            GameObject.Destroy(target.transform.GetChild(i).gameObject);
        }

        DebugMsg("DialogManager.RemoveChildren() did the thing for " + target.name);
    }

    void AdvanceStory()
    {
        if (OnAdvanceStory != null)
        {
            OnAdvanceStory();
        }
    }

    public void OnFinishStory()
    {
        RemoveChildren(goodPanel);
        RemoveChildren(badPanel);
        //Button choice = CreateChoiceView("End of story.\nRestart?");
        //choice.onClick.AddListener(delegate {
        //    StartStory();
        //});
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

    private GameObject goodPanel;
    private GameObject badPanel;
}
