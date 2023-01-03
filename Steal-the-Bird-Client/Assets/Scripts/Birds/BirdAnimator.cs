using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdAnimator : MonoBehaviour {
    [SerializeField] private Animator animator;

    public Animator Animator { get => animator; set => animator = value; }

    private void Awake() {
        animator.SetBool("Fly", true);
    }

    public void SetFly(bool _fly) {
        animator.SetBool("Fly", _fly);
    }

    public void SetDead() {
        animator.SetBool("Fly", true);
        animator.SetBool("Dead", true);
    }

    public void SetLanded() {
        animator.SetBool("Landed", true);
    }

    public void SetExit() {
        animator.SetBool("Exit", true);
    }
}
