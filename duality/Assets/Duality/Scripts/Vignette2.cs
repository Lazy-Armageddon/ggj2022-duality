using System.Collections;
using UnityEngine;

public abstract class VignetteLogic : MonoBehaviour
{
    public abstract void OnVignetteStart(VignetteManager manager);
}

public class Vignette2: VignetteLogic
{
    private VignetteManager manager;
    private bool isRunning;

    //-----------------------------------------------------------------------------
    public override void OnVignetteStart(VignetteManager mgr)
    {
        this.manager = mgr;
        isRunning = true;
        GetComponent<AudioSource>().Play();

        StartCoroutine(DoNotPushInFrontOfTrain());
    }

    //-----------------------------------------------------------------------------
    IEnumerator DoNotPushInFrontOfTrain()
    {
        yield return new WaitForSeconds(9f);
        if (isRunning)
        {
            manager?.StopVignette();
        }
    }

    //-----------------------------------------------------------------------------
    public void OnPushedInFrontOfTrain()
    {
        Debug.Log("pushed in front of train");
        isRunning = false;
        manager?.StopVignette();
    }
}
