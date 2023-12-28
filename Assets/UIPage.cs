using UnityEngine;
using DG.Tweening;

public class UIPage : MonoBehaviour
{
    public CanvasGroup[] cg;
    public bool isAnimated = false;
    public float animationSpeed = 0.5f;
    public int curPageId = 0;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        this.setPage(0);
    }

    public void setPage(int id)
    {
        if (!isAnimated)
        {
            isAnimated = true;
            for (int i = 0; i < this.cg.Length; i++)
            {
                if (this.cg[i] != null)
                {
                    if (i == id)
                    {
                        this.curPageId = id;
                        this.cg[i].DOFade(1f, id == 0 ? 0f : this.animationSpeed).OnComplete(() => isAnimated = false);
                        this.cg[i].interactable = true;
                        this.cg[i].blocksRaycasts = true;
                    }
                    else
                    {
                        this.cg[i].DOFade(0f, 0f);
                        this.cg[i].interactable = false;
                        this.cg[i].blocksRaycasts = false;
                    }
                }
            }
        }
    }
}
