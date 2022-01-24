using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VignetteLogic : MonoBehaviour
{
    public abstract void OnVignetteStart();
}

public class Vignette2: VignetteLogic
{
    public override void OnVignetteStart()
    {
        GetComponent<AudioSource>().Play();
    }
}
