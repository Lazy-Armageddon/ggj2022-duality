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

        AdvanceStory();
    }

    public void AdvanceStory()
    {
        // clear whatever we may have had from before
        RemoveChildren();

        if (_story.canContinue)
        {
            // handle next line w/ "next" button

            // get next line and trim whitespace off
            string text = _story.Continue();
            text = text?.Trim();

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
        else if (_story.currentChoices.Count > 0)
        {
            // a fork in the road!

            // display all the choices
            for (int i = 0; i < _story.currentChoices.Count; i++)
            {
                Choice choice = _story.currentChoices[i];
                Button button = CreateChoiceView(choice.text.Trim());

                // Tell the button what to do when we press it
                button.onClick.AddListener(delegate {
                    OnClickChoiceButton(choice);
                });

                // cheat button downwards to help keep it from overlapping the others
                var buttonRectTransform = button.GetComponent<RectTransform>();
                buttonRectTransform.localPosition += Vector3.down * i * 30;
            }
        }
        else
        {
            // we've read all the content and there's no choices
            // the story is finished!

            Button choice = CreateChoiceView("End of story.\nRestart?");
            choice.onClick.AddListener(delegate {
                StartStory();
            });
        }
    }

    // When we click the choice button, tell the story to choose that choice!
    void OnClickChoiceButton(Choice choice)
    {
        _story.ChooseChoiceIndex(choice.index);
        AdvanceStory();
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
