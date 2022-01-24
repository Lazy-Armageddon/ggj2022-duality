using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillZone : MonoBehaviour
{
    public Transform player;
    private Vector3 playerStartPosition;

    void Start()
    {
        playerStartPosition = player.position;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            player.position = playerStartPosition;
            // SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
        // else
        // {
        //     Destroy(col.gameObject);
        // }
    }
}
