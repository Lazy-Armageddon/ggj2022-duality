using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class VignetteManager : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public PlayerInput playerInput;

    [Header("Camera")]
    public CameraFollow cameraFollow;
    public PixelPerfectCamera pixelCamera;

    [Header("Vignette")]
    public float vignetteTransitionDuration = 1.5f;
    public AudioClip vignetteStartSound;
    public AudioClip vignetteMusic;

    [Header("UI")]
    public Transform angelDemonRoot;
    public Transform playerNPCRoot;

    private VignetteData currentVignetteData;
    private Vector3 lastPlayerPurgatoryPosition;
    private Vector3 lastCameraPurgatoryPosition;
    public GameManager gameManager;

    //-----------------------------------------------------------------------------
    private void Start()
    {
        (int x, int y) = GetPreferredPixelResolution();
        pixelCamera.refResolutionX = x;
        pixelCamera.refResolutionY = y;
    }

    //-----------------------------------------------------------------------------
    (int, int) GetPreferredPixelResolution()
    {
        return ((int)(Screen.width * 0.76f), (int)(Screen.height * 0.63f));
    }

    //-----------------------------------------------------------------------------
    public void StartVignette(VignetteData newVignetteData)
    {
        if (currentVignetteData)
        {
            currentVignetteData.gameObject.SetActive(false);
            var enemy = currentVignetteData.npc?.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.DestroyAction -= OnNpcDeath;
            }
        }
        newVignetteData.gameObject.SetActive(true);
        currentVignetteData = newVignetteData;

        if (currentVignetteData.npc)
        {
            currentVignetteData.npc.gameObject.SetActive(false);
            var enemy = currentVignetteData.npc?.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.DestroyAction += OnNpcDeath;
            }
        }

        WarpPlayerToNewLocation(newVignetteData.playerStart.position);
        LerpCameraToNewLocation(newVignetteData.cameraStart.position, () =>
        {
            currentVignetteData.npc.gameObject.SetActive(true);

            VignetteLogic logic = newVignetteData.GetComponent<VignetteLogic>();
            if (logic != null)
            {
                logic.OnVignetteStart(this);
            }
        });
        LerpUIOffscreen();

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(vignetteStartSound);
        audioSource.clip = vignetteMusic;
        audioSource.Play();
    }

    //-----------------------------------------------------------------------------
    void OnNpcDeath()
    {
        gameManager.InformNpcDeath();
    }

    //-----------------------------------------------------------------------------
    public void StopVignette()
    {
        if (currentVignetteData)
        {
            currentVignetteData.gameObject.SetActive(false);
            currentVignetteData = null;
        }

        WarpPlayerToNewLocation(lastPlayerPurgatoryPosition);
        LerpCameraToNewLocation(lastCameraPurgatoryPosition);
        LerpUIOnscreen();

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
    }

    //-----------------------------------------------------------------------------
    void WarpPlayerToNewLocation(Vector3 newLocation)
    {
        lastPlayerPurgatoryPosition = player.position;

        // Warp the player to vignette location once out of camera view
        var sequence = DOTween.Sequence();
        sequence.AppendInterval(vignetteTransitionDuration * 0.6f);
        sequence.Append(player.DOMove(newLocation, 0f));
    }

    //-----------------------------------------------------------------------------
    void LerpCameraToNewLocation(Vector3 newLocation, Action OnCompleteCB = null)
    {
        Camera camera = cameraFollow.GetComponent<Camera>();
        lastCameraPurgatoryPosition = camera.transform.position;
        cameraFollow.enabled = false;
        playerInput.enabled = false;

        // We want to go to where the player is but maintain our z value
        var newCameraPosition = newLocation;
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
            playerInput.enabled = true;
            OnCompleteCB?.Invoke();
        }));
    }

    //-----------------------------------------------------------------------------
    void LerpUIOffscreen()
    {
        float hackDistance = 10f;
        Vector3 uiPosition = angelDemonRoot.position;
        angelDemonRoot.DOMove(uiPosition + Vector3.up * hackDistance, vignetteTransitionDuration);

        Vector3 playerRootUIPosition = playerNPCRoot.position;
        playerNPCRoot.DOMove((playerRootUIPosition - Vector3.up * hackDistance), vignetteTransitionDuration);

        DOTween.To(() => pixelCamera.refResolutionX, (x) => pixelCamera.refResolutionX = x, Screen.width, vignetteTransitionDuration);
        DOTween.To(() => pixelCamera.refResolutionY, (x) => pixelCamera.refResolutionY = x, Screen.height, vignetteTransitionDuration);
    }

    //-----------------------------------------------------------------------------
    void LerpUIOnscreen()
    {
        float hackDistance = 10f;
        Vector3 uiPosition = angelDemonRoot.position;
        angelDemonRoot.DOMove(uiPosition + Vector3.down * hackDistance, vignetteTransitionDuration);

        Vector3 playerRootUIPosition = playerNPCRoot.position;
        playerNPCRoot.DOMove((playerRootUIPosition - Vector3.down * hackDistance), vignetteTransitionDuration);

        (int newWidth, int newHeight) = GetPreferredPixelResolution();
        DOTween.To(() => pixelCamera.refResolutionX, (x) => pixelCamera.refResolutionX = x, newWidth, vignetteTransitionDuration);
        DOTween.To(() => pixelCamera.refResolutionY, (x) => pixelCamera.refResolutionY = x, newHeight, vignetteTransitionDuration);
    }
}
