using UnityEngine;

public class ActivateObjectsTrigger : MonoBehaviour
{
    public GameObject[] objectsToEnable;

    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("triggered 2d");
        foreach (var obj in objectsToEnable)
        {
            obj.SetActive(true);
        }
    }
}
