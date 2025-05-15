using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float normalSpeed = 2;
    [SerializeField] private float sprintSpeed = 3;
    private Animator animator;
    private Vector3 direction;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        direction = new Vector3(horizontal, vertical);

        AnimateMovement(direction);
    }

    private void FixedUpdate()
    {
        //move the player
        direction.Normalize();
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
            transform.position += direction * sprintSpeed * Time.deltaTime;
        } else {
            transform.position += direction * normalSpeed * Time.deltaTime;
        }
        
    }

    void AnimateMovement(Vector3 direction)
    {
        direction.Normalize(); //so diagonal isn't faster
        if (animator != null)
        {
            if(direction.magnitude > 0) //check if moving
            {
                if(!animator.enabled){
                    animator.enabled = true;
                }
                if (direction.x > 0 && transform.localScale.x < 0){
                    Vector3 scale = transform.localScale;
                    scale.x = -1 * scale.x;
                    transform.localScale = scale;
                } else if (direction.x < 0 && transform.localScale.x > 0){
                    Vector3 scale = transform.localScale;
                    scale.x = -1 * scale.x;
                    transform.localScale = scale;
                }
;            }
            else
            {
                if(animator.enabled){
                    animator.enabled = false;
                }
            }
        }
    }

    void FlipAllChildren()
    {
        foreach (Transform child in transform)
        {
            Vector3 currentScale = child.localScale;
            currentScale.x *= -1;
            child.localScale = currentScale;
        }
    }

}
