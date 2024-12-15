using UnityEngine;

public class AvatarController : MonoBehaviour
{
    [SerializeField] private Animator animator;


    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

    }

    public void SetTalking()
    {
        animator.SetTrigger("talking");
    }

    public void SetThinking()
    {
        animator.SetTrigger("thinking");
    }


}
