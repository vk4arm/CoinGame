using UnityEngine;

public class AnimationRandom : MonoBehaviour
{
    void Start()
    {
        int offsetFloat = Animator.StringToHash("Offset");
        Animator animatior = GetComponent<Animator>();

        animatior.SetFloat(offsetFloat, Random.Range(0f, 0.5f));
    }
}
