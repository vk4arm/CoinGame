using UnityEngine;


public class DieBehaviour : GenericBehaviour
{
    public GameObject canvas;
    private int dieBool;
    private bool isDie;

    // Start is always called after any Awake functions.
    void Start()
    {
        // Set up the references.
        dieBool = Animator.StringToHash("Die");
        canSprint = false;
    }

    // Update is used to set features regardless the active behaviour.
    void Update()
    {
        if (isDie)
        {
            behaviourManager.GetAnim.SetBool(dieBool, isDie);
            canvas.GetComponent<Menu>().Defeat();
        }
        else if (GetComponent<BasicBehaviour>().time == 0)
        {
            SetDied();
        }
    }

    public override void LocalFixedUpdate() { }

    public override void LocalLateUpdate() { }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Water")
        {
            SetDied();
        }
    }

    private void SetDied()
    {
        isDie = true;
        GetComponent<MoveBehaviour>().enabled = false;
    }
}

