using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [FormerlySerializedAs("tempVignetteInstance1")]
    [Header("Temp Debug")]
    public VignetteData tempVignetteDataInstance1;
    [FormerlySerializedAs("tempVignetteInstance2")] public VignetteData tempVignetteDataInstance2;

    [Header("Dialogue")]
    private InkManager activeInkManager;
    private DialogManager dialogManager;
    private GameObject player;
    private GameObject[] npcs;

    [FormerlySerializedAs("cameraManager")]
    [Header("Misc")]
    public VignetteManager vignetteManager;

    //-----------------------------------------------------------------------------
    void Awake()
    {
        // look for special game objects automatically (managers etc)
        dialogManager = GameObject.Find("DialogManager")?.GetComponent<DialogManager>();
        player = GameObject.Find("Player");
        if (npcs == null)
        {
            npcs = GameObject.FindGameObjectsWithTag("Enemy");
        }

        // warn about those that could not be find
        if (!dialogManager) { Debug.Log("warning: could not find 'DialogManager'"); }
        if (!player) { Debug.Log("warning: could not find 'Player'"); }
        if (npcs == null) { Debug.Log("warning: could not find any GameObjects tagged \"npc\""); }
    }

    //-----------------------------------------------------------------------------
    void Update()
    {
        // if player gets within range of any NPC, start story
        foreach (GameObject npc in npcs)
        {
            TryTalkNPC(npc);
        }

        // TEMP DEBUG - do this in response to dialogue
        if (Input.GetKeyDown(KeyCode.T))
        {
            vignetteManager.StartVignette(tempVignetteDataInstance1);
        }
    }

    //-----------------------------------------------------------------------------
    void HookUpInkManager(InkManager ink)
    {
        // hook ink manager and dialog manager up with each other
        if (ink != null && dialogManager != null)
        {
            ink.OnTextLine += dialogManager.OnTextLine;
            dialogManager.OnAdvanceStory += ink.AdvanceStory;
            dialogManager.OnSelectChoice += ink.OnClickChoiceButton;

            ink.OnChoices += dialogManager.OnChoices;
            ink.OnFinishStory += dialogManager.OnFinishStory;
            ink.OnFinishStory += OnFinishStory;

            activeInkManager = ink;
        }
    }

    //-----------------------------------------------------------------------------
    void UnhookInkManager(InkManager ink)
    {
        // unhook ink manager and dialog manager from each other
        if (ink != null && dialogManager != null)
        {
            ink.OnTextLine -= dialogManager.OnTextLine;
            dialogManager.OnAdvanceStory -= ink.AdvanceStory;
            dialogManager.OnSelectChoice -= ink.OnClickChoiceButton;

            ink.OnChoices -= dialogManager.OnChoices;
            ink.OnFinishStory -= dialogManager.OnFinishStory;
            ink.OnFinishStory -= OnFinishStory;

            activeInkManager = null;
        }
    }


    void TryTalkNPC(GameObject npc)
    {
        // bail if references are bogus
        if (player == null || npc == null)
        {
            return;
        }

        // are we within range?
        const float storyThreshold = 2.0f;
        if ((player.transform.position - npc.transform.position).sqrMagnitude > storyThreshold*storyThreshold)
        {
            return;
        }

        // does this npc have an ink manager?
        var ink = npc.GetComponent<InkManager>();
        if (ink == null)
        {
            return;
        }

        // is the story already started?
        if (ink.StoryStarted)
        {
            return;
        }

        // all clear -- start the story! (and disable player input)
        HookUpInkManager(ink);
        ink.StartStory();
        var playerInput = player.GetComponent<PlayerInput>();
        playerInput.enabled = false;
    }

    void OnFinishStory()
    {
        if (player != null)
        {
            var playerInput = player.GetComponent<PlayerInput>();
            playerInput.enabled = true;
            UnhookInkManager(activeInkManager);
        }
    }
}
