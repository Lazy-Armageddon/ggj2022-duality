using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.U2D;

public class VignetteManager : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public PlayerInput playerInput;

    [FormerlySerializedAs("cameraController")]
    [Header("Camera")]
    public CameraFollow cameraFollow;
    public PixelPerfectCamera pixelCamera;

    [Header("Vignette")]
    public Transform vignettePlayerStart;
    public Transform vignetteCameraStart;
    public float vignetteTransitionDuration = 1.5f;
    public AudioClip vignetteStartSound;
    public AudioClip vignetteMusic;

    [Header("UI")]
    public Transform angelDemonRoot;
    public Transform playerNPCRoot;

    private VignetteData _currentVignetteData;

    //-----------------------------------------------------------------------------
    private void Start()
    {
        pixelCamera.refResolutionX = Screen.width - 420;
        pixelCamera.refResolutionY = Screen.height - 420;
    }

    //-----------------------------------------------------------------------------
    public void StartVignette(VignetteData newVignetteData)
    {
        if (_currentVignetteData)
        {
            _currentVignetteData.gameObject.SetActive(false);
        }
        newVignetteData.gameObject.SetActive(true);
        _currentVignetteData = newVignetteData;

        _WarpPlayerToNewLocation();
        _LerpCameraToNewLocation();
        _LerpUIOffscreen();

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(vignetteStartSound);
        audioSource.clip = vignetteMusic;
        audioSource.Play();

        void _WarpPlayerToNewLocation()
        {
            // Warp the player to vignette location once out of camera view
            var sequence = DOTween.Sequence();
            sequence.AppendInterval(vignetteTransitionDuration * 0.6f);
            sequence.Append(player.DOMove(newVignetteData.playerStart.position, 0f));
        }

        void _LerpCameraToNewLocation()
        {
            Camera camera = cameraFollow.GetComponent<Camera>();
            cameraFollow.enabled = false;
            playerInput.enabled = false;
            _currentVignetteData.npc.SetActive(false);

            // We want to go to where the player is but maintain our z value
            var newCameraPosition = newVignetteData.cameraStart.position;
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
                _currentVignetteData.npc.SetActive(true);
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
