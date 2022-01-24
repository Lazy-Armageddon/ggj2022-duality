using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Temp Debug")]
    public VignetteData tempVignetteDataInstance1;
    public VignetteData tempVignetteDataInstance2;

    [Header("Dialogue")]
    private InkManager activeInkManager;
    private DialogManager dialogManager;
    private GameObject player;
    private GameObject[] npcs;
    private Dictionary<string, object> storyState = new Dictionary<string, object>();

    [Header("Misc")]
    public VignetteManager vignetteManager;
    public GameObject purgatoryBridge1;

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
        // warn about those that could not be found
        if (!dialogManager) { Debug.Log("warning: could not find 'DialogManager'"); }
        if (!player) { Debug.Log("warning: could not find 'Player'"); }
        if (npcs == null) { Debug.Log("warning: could not find any GameObjects tagged \"npc\""); }

        if (vignetteManager != null)
        {
            vignetteManager.gameManager = this;
        }

        StartCoroutine(DebugUpdate());
    }

    //-----------------------------------------------------------------------------
    void Update()
    {
        // if player gets within range of any NPC, start story
        foreach (GameObject npc in npcs)
        {
            TryTalkNPC(npc);
        }
    }

    //-----------------------------------------------------------------------------
    IEnumerator DebugUpdate()
    {
        // test go to vignette
        while (true)
        {
            /*
            // TEMP DEBUG - do this in response to dialogue
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                vignetteManager.StartVignette(tempVignetteDataInstance1);
            }

            // TEMP DEBUG - do this in response to dialogue
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                vignetteManager.StopVignette();
                purgatoryBridge1.SetActive(true);
            }
            //*/

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                vignetteManager.StartVignette(tempVignetteDataInstance2);
            }

            yield return null;
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

        // all clear to begin starting the story
        HookUpInkManager(ink);

        // load the story
        ink.LoadStory();

        // todo -- provide global state variables
        ink.WriteStateVariables(storyState);

        // start the story
        ink.StartStory();

        // disable player input
        var playerInput = player.GetComponent<PlayerInput>();
        playerInput.enabled = false;
    }

    public void InformNpcDeath()
    {
        vignetteManager.StopVignette();
        purgatoryBridge1.SetActive(true);
    }

    void OnFinishStory()
    {
        // wrap up story details
        if (activeInkManager != null)
        {
            // todo: pull out story variables
            activeInkManager.ReadStateVariables(storyState);

            // unhook callbacks
            UnhookInkManager(activeInkManager);
        }

        // let player move again
        if (player != null)
        {
            var playerInput = player.GetComponent<PlayerInput>();
            playerInput.enabled = true;
        }

        // try trigger special story events
        if (CheckStoryStateBool("vignette"))
        {
            storyState["vignette"] = false;
            vignetteManager.StartVignette(tempVignetteDataInstance1);
        }

        if (CheckStoryStateBool("vignette2"))
        {
            storyState["vignette2"] = false;
            vignetteManager.StartVignette(tempVignetteDataInstance1);
        }
    }

    bool CheckStoryStateBool(string name)
    {
        return storyState.ContainsKey(name) && storyState[name] is bool && (bool)storyState[name];
    }
}
