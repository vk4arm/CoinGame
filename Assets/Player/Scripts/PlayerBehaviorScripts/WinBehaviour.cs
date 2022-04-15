using UnityEngine;


public class WinBehaviour : GenericBehaviour
{
    public GameObject canvas;
    public GameObject coins;
    private int winBool;
    private bool isWin;

    void Start()
    {
        winBool = Animator.StringToHash("Win");
        behaviourManager.SubscribeBehaviour(this);
    }

    void Update()
    {
        if (isWin)
        {
            behaviourManager.GetAnim.SetBool(winBool, isWin);
            GetComponent<DieBehaviour>().enabled = false;
            canvas.GetComponent<Menu>().Victory();
        }
        else if (coins.transform.childCount == 0)
        {
            isWin = true;
        }
    }

    public override void LocalFixedUpdate() { }

    public override void LocalLateUpdate() { }
}

