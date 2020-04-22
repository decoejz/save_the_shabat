using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public class BadGolem : MonoBehaviour
{
    public CharacterController2D.CharacterCollisionState2D flags;

    public GameObject barreira;
    public bool isBadAttacking;      // Se está atacando
    public bool isBadDying;      // Se está morrendo

    public int vidas;
    public AudioClip somDead; // Quando o inimigo morre

    // private Vector3 moveDirection = Vector3.zero; // direção que o personagem se move
    private CharacterController2D characterController;	//Componente do Char. Controller

    private Animator animator;

    void Start()
    {
        characterController = GetComponent<CharacterController2D>(); //identif. o componente
        animator = GetComponent<Animator>();
        transform.eulerAngles = new Vector3(0, 180, 0);
        vidas = 5;
        StartCoroutine(working());
    }

    void Update()
    {
        animator.SetBool("isBadAttacking", isBadAttacking);
        animator.SetBool("isBadDying", isBadDying);

        if (vidas <= 0)
        {
            barreira.GetComponent<EdgeCollider2D>().enabled = false;
            StartCoroutine(die());
        }
    }

    IEnumerator die()
    {
        StopCoroutine(working());
        isBadAttacking = false;
        AudioSource.PlayClipAtPoint(somDead, this.gameObject.transform.position);
        isBadDying = true;
        yield return new WaitForSeconds(1.5f);
        Destroy(this.gameObject);
    }

    IEnumerator working()
    {
        while (true)
        {
            isBadAttacking = true;
            yield return new WaitForSeconds(3.5f);
            isBadAttacking = false;
            yield return new WaitForSeconds(2.5f);
        }
    }
}
