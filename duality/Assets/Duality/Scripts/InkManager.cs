using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
//using System.Diagnostics;
using Ink.Runtime;
using UnityEngine.Events;

public class InkManager : MonoBehaviour
{
    public UnityEvent dialogueComplete;

    // called before Start()
    void Awake()
    {
    }

    void Start()
    {
    }

    public bool _started = false;
    public bool StoryStarted { get { return _started; }}

    public void InkErrorHandler(string message, Ink.ErrorType type)
    {
        Debug.Log("ink error: " + type.ToString() + " - " + message);
    }

    public void LoadStory()
    {
        _story = new Story(_inkJsonAsset.text);

        // report errors and warnings
        Debug.Log("------------------------------");
        Debug.Log("Report for loaded story:");

        // set up an error handler
        _story.onError += InkErrorHandler;

        // continue report (warnings)
        if (_story.hasWarning)
        {
            for (int i = 0; i < _story.currentWarnings.Count; i++)
            {
                var warning = _story.currentWarnings[i];
                Debug.Log("ink warning: " + warning);
            }
        }
        else
        {
            Debug.Log("no warnings -- great!");
        }
        Debug.Log("------------------------------");

        // set up state management
        storyState = _story.variablesState;
    }

    public void StartStory()
    {
        if (OnCreateStory != null)
        {
            OnCreateStory(_story);
        }

        AdvanceStory();

        _started = true;
    }

    public void AdvanceStory()
    {
        if (_story.canContinue)
        {
            // handle next line w/ "next" button

            // get next line and trim whitespace off
            string text = _story.Continue();
            text = text?.Trim();

            // provide text line out
            if (OnTextLine != null)
            {
                OnTextLine(text);
            }
        }
        else if (_story.currentChoices.Count > 0)
        {
            // a fork in the road!

            if (OnChoices != null)
            {
                OnChoices(_story.currentChoices);
            }
        }
        else
        {
            // we've read all the content and there's no choices
            // the story is finished!
            FinishStory();

            dialogueComplete?.Invoke();

            /*
            Button choice = CreateChoiceView("End of story.\nRestart?");
            choice.onClick.AddListener(delegate {
                StartStory();
            });
            //*/
        }
    }

    // When we click the choice button, tell the story to choose that choice!
    public void OnClickChoiceButton(Choice choice)
    {
        _story.ChooseChoiceIndex(choice.index);
        AdvanceStory();
    }

    void FinishStory()
    {
        if (OnFinishStory != null)
        {
            OnFinishStory();
        }
    }

    // write state variables from external to internal state
    public void WriteStateVariables(Dictionary<string, object> externalStoryState)
    {
        Debug.Log("writing global story state to ink script state machine");
        foreach (KeyValuePair<string, object> kvp in externalStoryState)
        {
            Debug.LogFormat("  Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            if (storyState.GlobalVariableExistsWithName(kvp.Key))
            {
                storyState[kvp.Key] = kvp.Value;
            }
        }
    }

    // read state variables from internal state and store externally
    public void ReadStateVariables(Dictionary<string, object> externalStoryState)
    {
        Debug.Log("reading global story state to ink script state machine");
        foreach (string key in storyState)
        {
            object value = storyState[key];
            Debug.LogFormat("  Key = {0}, Value = {1}", key, value);
            externalStoryState[key] = value;
        }
    }

    public event Action<Story> OnCreateStory;
    public event Action<string> OnTextLine;
    public event Action< List<Ink.Runtime.Choice> > OnChoices;
    public event Action OnFinishStory;
    private VariablesState storyState;

    [SerializeField]
    public TextAsset _inkJsonAsset = null;
    private Story _story;
}
