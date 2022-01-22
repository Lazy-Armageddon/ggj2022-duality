using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public Transform vignettePlayerStart;

    [Header ("Camera")]
    public CameraFollow cameraController;
    public Transform vignetteCameraStart;
    public float vignetteTransitionDuration = 1.5f;

    //-----------------------------------------------------------------------------
    void Update()
    {
        // DEBUG
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(StartVignette());
        }
    }

    //-----------------------------------------------------------------------------
    IEnumerator StartVignette()
    {
        cameraController.enabled = false;
        float timeElapsed = 0f;
        Camera camera = cameraController.GetComponent<Camera>();
        Vector3 startPos = camera.transform.position;

        StartCoroutine(TeleportPlayerAfterDelay(vignetteTransitionDuration / 2f));

        while (timeElapsed < vignetteTransitionDuration)
        {
            float alpha = timeElapsed / vignetteTransitionDuration;
            camera.transform.position = Vector3.Lerp(startPos, vignetteCameraStart.position, alpha);
            yield return null;

            timeElapsed += Time.deltaTime;
        }

        camera.transform.position = vignetteCameraStart.position;
    }

    //-----------------------------------------------------------------------------
    IEnumerator TeleportPlayerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        player.position = vignettePlayerStart.position;
    }
}
