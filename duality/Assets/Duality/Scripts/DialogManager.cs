using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
//using System.Diagnostics;
using TMPro;

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

    TextMeshProUGUI findTextInScene(string name)
    {
        var text = GameObject.Find(name)?.GetComponent<TextMeshProUGUI>();
        if (!text) { Debug.Log("warning: could not find \"" + name + "\" GameObject w/ TextMeshProUGUI instance."); }
        return text;
    }

    // called before Start()
    void Awake()
    {
        // find panels with names by convention
        angelText = findTextInScene("AngelText");
        demonText = findTextInScene("DemonText");
        playerText = findTextInScene("PlayerText");
        npcText = findTextInScene("NpcText");
    }

    void Start()
    {
        // note: do NOT clear scene elements here! (May have already started a story...)
    }

    public void OnChoices(List<Ink.Runtime.Choice> choices)
    {
        DebugMsg("DialogManager.OnChoices(...)");

        // clear whatever we may have had from before
        ClearAllText();

        // display all the choices
        for (int i = 0; i < choices.Count; i++)
        {
            Ink.Runtime.Choice choice = choices[i];
            Button button = CreateChoiceView(choice.text.Trim());

            // Tell the button what to do when we press it
            button.onClick.AddListener(delegate {
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
        ClearAllText();

        // display new text
        CreateContentView(text);

        // create "next" button
        var button = CreateChoiceView("next");
        button.onClick.AddListener(delegate {
            AdvanceStory();
        });

        // cheat button downwards to help keep it from overlapping text
        var buttonRectTransform = button.GetComponent<RectTransform>();
        //buttonRectTransform.localPosition += Vector3.down * 150;
    }

    // Creates a textbox showing the the line of text
    void CreateContentView(string text)
    {
        DebugMsg("DialogManager.CreateContentView(" + text + ")");

        // determine first where we want to write the text

        // default to npc panel as target
        TextMeshProUGUI target = npcText;

        // pick others depending on special text start patterns
        if (text.StartsWith("Angel"))
        {
            target = angelText;
        }
        else if (text.StartsWith("Demon") || text.StartsWith("Devil"))
        {
            target = demonText;
        }
        else if (text.StartsWith("Player") || text.StartsWith("Me") || text.StartsWith("Character"))
        {
            target = playerText;
        }

        if (!target)
        {
            Debug.Log("warning: could not find target to write text message...");
            return;
        }

        target.text = text;
    }

    // Creates a button showing the choice text
    Button CreateChoiceView(string text)
    {
        DebugMsg("DialogManager.CreateChoiceView(" + text + ")");

        // Creates the button from a prefab
        Button choice = Instantiate(buttonPrefab) as Button;

        GameObject target = playerText.gameObject;
        choice.transform.SetParent(target.transform, false);

        // Gets the text from the button prefab
        Text choiceText = choice.GetComponentInChildren<Text>();
        choiceText.text = text;

        // Make the button expand to fit the text
        HorizontalLayoutGroup layoutGroup = choice.GetComponent<HorizontalLayoutGroup>();
        layoutGroup.childForceExpandHeight = false;

        return choice;
    }

    void ClearAllText()
    {
        RemoveChildren(playerText.gameObject);

        if (angelText)
        {
            angelText.text = "";
        }
        if (demonText)
        {
            demonText.text = "";
        }
        if (playerText)
        {
            playerText.text = "";
        }
        if (npcText)
        {
            npcText.text = "";
        }
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
        ClearAllText();
        //Button choice = CreateChoiceView("End of story.\nRestart?");
        //choice.onClick.AddListener(delegate {
        //    StartStory();
        //});
    }

    public event Action OnAdvanceStory;
    public event Action<Ink.Runtime.Choice> OnSelectChoice;

    // UI Prefabs
    [SerializeField]
    private Button buttonPrefab = null;

    // panels
    private TextMeshProUGUI angelText;
    private TextMeshProUGUI demonText;
    private TextMeshProUGUI playerText;
    private TextMeshProUGUI npcText;
}
