
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public bool isPlaying = false;
    public GameObject objectContainer;
    public GameObject scoreAnimationPrefab;
    public SpriteRenderer characterSprite;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPlaying)
        {
            //Debug.Log(collision.gameObject.name);
            int score = collision.gameObject.GetComponent<Item>().score;
            Vector3 pos = collision.gameObject.transform.position;
            //Destroy(collision.gameObject);
            despawn(collision.gameObject);

            //GameObject scoreGO = GameObject.Instantiate(scoreAnimationPrefab, objectContainer.transform);
            GameObject scoreGO = ObjectPoolManager.SpawnGameObject(scoreAnimationPrefab);
            if (scoreGO != null) scoreGO.transform.SetParent(objectContainer.transform);

            scoreGO.transform.position = pos;

            if(scoreGO.GetComponent<ScoreAnimationController>() != null)
                scoreGO.GetComponent<ScoreAnimationController>().score = score;

            if (scoreGO.GetComponent<ScoreController>() != null)
                scoreGO.GetComponent<ScoreController>().score = score;

            GameLoader.Instance.score(score);
        }
    }

    void despawn(GameObject collision)
    {
        ObjectPoolManager.DespawnGameObject(collision);
        ObjectPoolManager.PermanentlyDestroyGameObjectsOfType(collision);
    }



}
