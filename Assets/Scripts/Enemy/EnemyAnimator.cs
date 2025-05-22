using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimator : MonoBehaviour
{
    Animator animator;
    private State enemyState;
    private Vector2 moveDirection;

    bool hasIsMoving, hasIsTakingDamage, hasIsDead, hasIsAttacking;

    void Start()
    {
        animator = GetComponent<Animator>();

        hasIsMoving = animator.ContainsParam("isMoving");
        hasIsTakingDamage = animator.ContainsParam("isTakingDamage");
        hasIsDead = animator.ContainsParam("isDead");
        hasIsAttacking = animator.ContainsParam("isAttacking");
    }

    public void ChangeEnemyState(State state)
    {
        enemyState = state;
        Animate();
    }

    public void ChangeEnemyMoveDirection(Vector2 dir)
    {
        moveDirection = dir.normalized;
        Animate();
    }

    private void Animate()
    {
        if (hasIsMoving) animator.SetBool("isMoving", enemyState == State.Moving);
        if (hasIsAttacking) animator.SetBool("isAttacking", enemyState == State.Attacking);
        if (hasIsTakingDamage) animator.SetBool("isTakingDamage", enemyState == State.TakingDamage);
        if (hasIsDead) animator.SetBool("isDead", enemyState == State.Dead);

        animator.SetFloat("x", moveDirection.x);
        animator.SetFloat("y", moveDirection.y);
    }

}