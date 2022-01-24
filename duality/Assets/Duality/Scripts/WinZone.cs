using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinZone : MonoBehaviour
{
    public Transform player;
    private Vector3 playerStartPosition;
    private GameObject gameManager;

    void Start()
    {
        playerStartPosition = player.position;
        gameManager = FindGameObject("Game Manager");
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            //player.position = playerStartPosition;

            //player.gameObject.GetComponent<CharacterController2D>().ResetState();
            // SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);

            gameManager.GetComponent<GameManager>().StartVignette3();
        }
        // else
        // {
        //     Destroy(col.gameObject);
        // }
    }

    GameObject FindGameObject(string name)
    {
        var go = GameObject.Find(name);
        if (!go) { Debug.Log("warning: could not find GameObject \"" + name + "\""); }
        return go;
    }
}
