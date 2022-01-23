using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Dialogue")]
    private InkManager inkManager;
    private DialogManager dialogManager;

    //-----------------------------------------------------------------------------
    void Awake()
    {
        // look for a few fellow managers automatically
        inkManager = GameObject.Find("InkManager")?.GetComponent<InkManager>();
        dialogManager = GameObject.Find("DialogManager")?.GetComponent<DialogManager>();

        // warn about those that could not be find
        if (!inkManager) { Debug.Log("warning: could not find 'InkManager'"); }
        if (!dialogManager) { Debug.Log("warning: could not find 'DialogManager'"); }

        // hook ink manager and dialog manager up with each other
        if (inkManager != null && dialogManager != null)
        {
            inkManager.OnTextLine += dialogManager.OnTextLine;
            dialogManager.OnAdvanceStory += inkManager.AdvanceStory;
            dialogManager.OnSelectChoice += inkManager.OnClickChoiceButton;

            inkManager.OnChoices += dialogManager.OnChoices;
        }
    }

    //-----------------------------------------------------------------------------
    void Start()
    {
        //
    }
}
