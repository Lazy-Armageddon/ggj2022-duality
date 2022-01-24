using UnityEngine;

public abstract class VignetteLogic : MonoBehaviour
{
    public abstract void OnVignetteStart(VignetteManager manager);
}

public class Vignette2: VignetteLogic
{
    private VignetteManager manager;

    //-----------------------------------------------------------------------------
    public override void OnVignetteStart(VignetteManager mgr)
    {
        this.manager = mgr;
        GetComponent<AudioSource>().Play();
    }

    //-----------------------------------------------------------------------------
    public void OnPushedInFrontOfTrain()
    {
        Debug.Log("pushed in front of train");
        manager?.StopVignette();
    }
}
