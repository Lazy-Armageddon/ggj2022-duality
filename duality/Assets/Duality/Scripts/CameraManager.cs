using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.U2D;

public class CameraManager : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [FormerlySerializedAs("cameraController")]
    [Header("Camera")]
    public CameraFollow cameraFollow;
    public PixelPerfectCamera pixelCamera;

    [Header("Vignette")]
    public Transform vignettePlayerStart;
    public float vignetteTransitionDuration = 1.5f;
    public AudioClip vignetteStartSound;

    [Header("UI")]
    public Transform angelDemonRoot;
    public Transform playerNPCRoot;

    //-----------------------------------------------------------------------------
    private void Start()
    {
        pixelCamera.refResolutionX = Screen.width - 420;
        pixelCamera.refResolutionY = Screen.height - 420;
    }

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
        _WarpPlayerToNewLocation();
        _LerpCameraToNewLocation();
        _LerpUIOffscreen();

        GetComponent<AudioSource>().PlayOneShot(vignetteStartSound);

        void _WarpPlayerToNewLocation()
        {
            // Warp the player to vignette location once out of camera view
            var sequence = DOTween.Sequence();
            sequence.AppendInterval(vignetteTransitionDuration * 0.5f);
            sequence.Append(player.DOMove(vignettePlayerStart.position, 0f));
        }

        void _LerpCameraToNewLocation()
        {
            Camera camera = cameraFollow.GetComponent<Camera>();
            cameraFollow.enabled = false;

            // We want to go to where the player is but maintain our z value
            var newCameraPosition = vignettePlayerStart.position;
            newCameraPosition.z = camera.transform.position.z;

            // Lerp the camera through the clouds to the vignette area
            var sequence = DOTween.Sequence();
            sequence.AppendInterval(0.25f);
            sequence.Append(camera.transform.DOMove(newCameraPosition, vignetteTransitionDuration));
            // cameraTween.SetEase(Ease.InCubic); //<-- change interpolation type

            // restore camera control when done
            sequence.OnComplete((() =>
            {
                cameraFollow.enabled = true;
            }));
        }

        void _LerpUIOffscreen()
        {
            float hackDistance = 10f;
            Vector3 uiPosition = angelDemonRoot.position;
            angelDemonRoot.DOMove(uiPosition + Vector3.up * hackDistance, vignetteTransitionDuration);

            Vector3 playerRootUIPosition = playerNPCRoot.position;
            playerNPCRoot.DOMove((playerRootUIPosition - Vector3.up * hackDistance), vignetteTransitionDuration);

            DOTween.To(() => pixelCamera.refResolutionX, (x) => pixelCamera.refResolutionX = x, Screen.width, vignetteTransitionDuration);
            DOTween.To(() => pixelCamera.refResolutionY, (x) => pixelCamera.refResolutionY = x, Screen.height, vignetteTransitionDuration);
        }
    }
}
