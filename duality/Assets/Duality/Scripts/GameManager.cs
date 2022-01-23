using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public Transform vignettePlayerStart;

    [Header ("Camera")]
    public CameraFollow cameraController;
    public float vignetteTransitionDuration = 1.5f;

    //-----------------------------------------------------------------------------
    void Update()
    {
        // DEBUG
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartVignette();
        }
    }

    //-----------------------------------------------------------------------------
    void StartVignette()
    {
        // Warp the player to vignette location once out of camera view
        var sequence = DOTween.Sequence();
        sequence.AppendInterval(vignetteTransitionDuration * 0.6f);
        sequence.Append(player.DOMove(vignettePlayerStart.position, 0f));

        Camera camera = cameraController.GetComponent<Camera>();
        cameraController.enabled = false;

        // We want to go to where the player is but maintain our z value
        var newCameraPosition = vignettePlayerStart.position;
        newCameraPosition.z = camera.transform.position.z;

        // Lerp the camera through the clouds to the vignette area
        var cameraTween = camera.transform.DOMove(newCameraPosition, vignetteTransitionDuration);
        // cameraTween.SetEase(Ease.InCubic); //<-- change interpolation type

        // restore camera control when done
        cameraTween.OnComplete((() =>
        {
            cameraController.enabled = true;
        }));
    }
}
