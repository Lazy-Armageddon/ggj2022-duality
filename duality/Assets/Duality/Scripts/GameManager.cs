using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Dialogue")]
    private InkManager inkManager;
    private DialogManager dialogManager;
    private GameObject player;
    private GameObject npc;

    //-----------------------------------------------------------------------------
    void Awake()
    {
        // look for special game objects automatically (managers etc)
        inkManager = GameObject.Find("InkManager")?.GetComponent<InkManager>();
        dialogManager = GameObject.Find("DialogManager")?.GetComponent<DialogManager>();
        player = GameObject.Find("Player");
        npc = GameObject.Find("NPC");

        // warn about those that could not be find
        if (!inkManager) { Debug.Log("warning: could not find 'InkManager'"); }
        if (!dialogManager) { Debug.Log("warning: could not find 'DialogManager'"); }
        if (!player) { Debug.Log("warning: could not find 'Player'"); }
        if (!npc) { Debug.Log("warning: could not find 'NPC'"); }

        // hook ink manager and dialog manager up with each other
        if (inkManager != null && dialogManager != null)
        {
            inkManager.OnTextLine += dialogManager.OnTextLine;
            dialogManager.OnAdvanceStory += inkManager.AdvanceStory;
            dialogManager.OnSelectChoice += inkManager.OnClickChoiceButton;

            inkManager.OnChoices += dialogManager.OnChoices;
            inkManager.OnFinishStory += dialogManager.OnFinishStory;
            inkManager.OnFinishStory += OnFinishStory;
        }
    }

    //-----------------------------------------------------------------------------
    void Start()
    {
        //
    }

    //-----------------------------------------------------------------------------
    void Update()
    {
        // if player gets within range of NPC, start story
        if (player != null && npc != null)
        {
            const float storyThreshold = 2.0f;
            if ((player.transform.position - npc.transform.position).sqrMagnitude < storyThreshold*storyThreshold)
            {
                // within range -- start the story, if one isn't already underway
                if (!inkManager.StoryStarted)
                {
                    inkManager.StartStory();
                    var playerInput = player.GetComponent<PlayerInput>();
                    playerInput.enabled = false;
                    //playerInput.DeactivateInput();
                }
            }
        }
    }

    void OnFinishStory()
    {
        if (player != null)
        {
            var playerInput = player.GetComponent<PlayerInput>();
            playerInput.enabled = true;
        }
    }
}
