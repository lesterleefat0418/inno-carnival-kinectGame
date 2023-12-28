using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEmitter : MonoBehaviour
{
    public GameObject[] objects;
    public GameObject objectContainer;
    public Vector3 emitPosition;
    public Vector3 emitRange;
    public Vector3 minSpeed;
    public Vector3 maxSpeed;
    public float minEmitInterval = 0.2f;
    public float maxEmitInterval = 1f;
    private float emitTimer = 0f;
    public bool isEmitting = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isEmitting) {
            emitTimer -= Time.deltaTime;
            if (emitTimer < 0f) {
                //emit
                EmitItem();
                //set next emit time
                emitTimer = Random.Range(minEmitInterval, maxEmitInterval);
            }
        }
    }

    private void EmitItem() {
        int randomItemIndex = Mathf.FloorToInt(Random.Range(0f, (float)objects.Length - 0.01f));
        //GameObject go = GameObject.Instantiate(objects[randomItemIndex], objectContainer.transform);

        GameObject go = ObjectPoolManager.SpawnGameObject(objects[randomItemIndex], Vector2.zero, Quaternion.identity);
        if(go != null) go.transform.SetParent(objectContainer.transform);

        go.transform.position = new Vector3(emitPosition.x + Random.Range(-emitRange.x, emitRange.x), emitPosition.y + Random.Range(-emitRange.y, emitRange.y), emitPosition.z + Random.Range(-emitRange.z, emitRange.z));
        Item item = go.GetComponent<Item>();
        item.moveSpeed = new Vector3(Random.Range(minSpeed.x, maxSpeed.x), Random.Range(minSpeed.y, maxSpeed.y), Random.Range(minSpeed.z, maxSpeed.z));
    }
}
