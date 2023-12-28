using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SuperMOMOutfit
{ 
    Flying,
    Standing
}

public class SuperMOMController : MonoBehaviour
{
    public GameObject OutfitFlying;
    public GameObject OutfitStanding;
    public GameObject scoreAnimationPrefab;
    public GameObject objectContainer;
    public bool isPlaying = false;
    public AudioSource SFXFlyUp;

    private SuperMOMOutfit _outfit = SuperMOMOutfit.Flying;
    public SuperMOMOutfit outfit {
        get {
            return _outfit;
        }

        set {
            _outfit = value;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        switch (_outfit) {
            case SuperMOMOutfit.Standing:
                OutfitFlying.GetComponent<SpriteRenderer>().enabled = false;
                OutfitStanding.GetComponent<SpriteRenderer>().enabled = true;
                break;
            default:
                OutfitFlying.GetComponent<SpriteRenderer>().enabled = true;
                OutfitStanding.GetComponent<SpriteRenderer>().enabled = false;
                break;
        }
    }

    private void ReadyToPlay() {
        GameLoader.Instance.mapKinectToCharacter = true;
    }

    private void ResetCharacterPosition() {
        outfit = SuperMOMOutfit.Standing;
        this.gameObject.transform.parent.transform.position = Vector3.zero;
    }

    private void Awake()
    {
        outfit = SuperMOMOutfit.Standing;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPlaying) {
            Debug.Log(collision.gameObject.name);
            int score = collision.gameObject.GetComponent<Item>().score;
            Vector3 pos = collision.gameObject.transform.position;
            Destroy(collision.gameObject);

            GameObject scoreGO = GameObject.Instantiate(scoreAnimationPrefab, objectContainer.transform);
            scoreGO.transform.position = pos;
            scoreGO.GetComponent<ScoreAnimationController>().score = score;

            GameLoader.Instance.score(score);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }

    private void FlyUp() {
        outfit = SuperMOMOutfit.Flying;
        if (SFXFlyUp.isPlaying) {
            SFXFlyUp.Stop();
        }
        SFXFlyUp.Play();
    }
}
