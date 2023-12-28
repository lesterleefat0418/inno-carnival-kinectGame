using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemMark itemMark = ItemMark.None;
    public int score;
    public Vector3 moveSpeed;

    public enum ItemMark { 
        None,
        type1 = 1,
        type2 = 2,
        type3 = 3
    }

    // Start is called before the first frame update
    void Start()
    {
        switch (itemMark)
        {
            case ItemMark.None:
                this.score = 0;
                break;
            case ItemMark.type1:
                this.score = ConfigPage.Instance.configData.itemMarks.type1;
                break;
            case ItemMark.type2:
                this.score = ConfigPage.Instance.configData.itemMarks.type2;
                break;
            case ItemMark.type3:
                this.score = ConfigPage.Instance.configData.itemMarks.type3;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveSpeed * Time.deltaTime);
    }
}
