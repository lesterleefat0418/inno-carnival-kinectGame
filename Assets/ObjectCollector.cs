
using UnityEngine;

public class ObjectCollector : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Destroy: " + collision.gameObject.name);
        //Destroy(collision.gameObject);
        despawn(collision.gameObject);
    }


    void despawn(GameObject collider)
    {
        ObjectPoolManager.DespawnGameObject(collider);
        ObjectPoolManager.PermanentlyDestroyGameObjectsOfType(collider);
    }
}
