using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Image vinhoImg;
    public Image paoImg;
    public Image velaImg;

    public CharacterController2D.CharacterCollisionState2D flags;
    public float walkSpeed = 4.0f;     // Depois de incluido, alterar no Unity Editor
    public float jumpSpeed = 8.0f;     // Depois de incluido, alterar no Unity Editor
    public float gravity = 9.8f;       // Depois de incluido, alterar no Unity Editor
    public float doubleJumpSpeed = 6.0f; //Depois de incluido, alterar no Editor

    public bool doubleJumped; // informa se foi feito um pulo duplo
    public bool isGrounded;     // Se está no chão
    public bool isJumping;      // Se está pulando
    public bool isFacingRight;      // Se está olhando para a direita
    public bool isAttacking;      // Se está atacando
    public bool isDying;      // Se está morrendo
    public LayerMask mask;  // para filtrar os layers a serem analisados

    private Vector3 moveDirection = Vector3.zero; // direção que o personagem se move
    private CharacterController2D characterController;	//Componente do Char. Controller
    private Vector3 posInicial = new Vector3(-5f, -3f, 0f); // Posicao inicial
    private Vector3 posLuta = new Vector3(138.0f, 27.0f, 0.0f); // Posicao luta com golem

    private Animator animator;

    public bool getVela = false;
    public bool getVinho = false;
    public bool getPao = false;

    public AudioClip estrela;
    public AudioClip somEspeciais;
    public AudioClip somKilled; // Quando o jogador morre
    public AudioClip somCheckpoint; // Quando o jogador atinge o checkpoint
    private bool checkpoint;
    public AudioClip musicaFundo; // Musica do jogo

    private bool colidiu = false;

    void Start()
    {
        characterController = GetComponent<CharacterController2D>(); //identif. o componente
        animator = GetComponent<Animator>();
        AudioSource.PlayClipAtPoint(musicaFundo, this.gameObject.transform.position, 7);
        checkpoint = false;
    }

    void Update()
    {
        moveDirection.x = Input.GetAxis("Horizontal"); // recupera valor dos controles
        moveDirection.x *= walkSpeed;

        if (moveDirection.x < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            isFacingRight = false;
        }
        else if (moveDirection.x > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            isFacingRight = true;
        }

        if (Input.GetButton("Fire1"))
        {
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
        }

        if (isGrounded)
        {               // caso esteja no chão
            moveDirection.y = 0.0f;
            isJumping = false;
            doubleJumped = false; // se voltou ao chão pode faz pulo duplo
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
                isJumping = true;
            }
        }
        else
        {            // caso esteja pulando 
            if (Input.GetButtonUp("Jump") && moveDirection.y > 0) // Soltando botão diminui pulo
                moveDirection.y *= 0.5f;
            if (Input.GetButtonDown("Jump") && !doubleJumped) // Segundo clique faz pulo duplo
            {
                moveDirection.y = doubleJumpSpeed;
                doubleJumped = true;
            }

        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 4f, mask);
        if (hit.collider != null && isGrounded)
        {
            transform.SetParent(hit.transform);
            if (Input.GetAxis("Vertical") < 0 && Input.GetButtonDown("Jump"))
            {
                moveDirection.y = -jumpSpeed;
                StartCoroutine(PassPlatform(hit.transform.gameObject));
            }
        }
        else
        {
            transform.SetParent(null);
        }


        moveDirection.y -= gravity * Time.deltaTime;    // aplica a gravidade
        characterController.move(moveDirection * Time.deltaTime);   // move personagem	

        flags = characterController.collisionState;     // recupera flags
        isGrounded = flags.below;               // define flag de chão


        // Atualizando Animator com estados do jogo
        animator.SetFloat("movementX", Mathf.Abs(moveDirection.x / walkSpeed)); // +Normalizado
        animator.SetFloat("movementY", moveDirection.y);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isDying", isDying);

        if (PlayerPrefs.GetInt("Vidas", 0) < 0)
        {
            SceneManager.LoadScene("GameOver");
        }
    }
    IEnumerator PassPlatform(GameObject platform)
    {
        platform.GetComponent<EdgeCollider2D>().enabled = false;
        yield return new WaitForSeconds(1.0f);
        platform.GetComponent<EdgeCollider2D>().enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Estrela") && other.gameObject.tag == "estrela")
        {
            int temp = PlayerPrefs.GetInt("Pontos", 0);
            temp += 10;
            PlayerPrefs.SetInt("Pontos", temp);
            AudioSource.PlayClipAtPoint(estrela, this.gameObject.transform.position);
            Destroy(other.gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Estrela") && other.gameObject.tag == "pao")
        {
            getPao = true;
            Behaviour bhvr = (Behaviour)paoImg;
            bhvr.enabled = true;
            AudioSource.PlayClipAtPoint(somEspeciais, this.gameObject.transform.position);
            Destroy(other.gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Estrela") && other.gameObject.tag == "vinho")
        {
            getVinho = true;
            Behaviour bhvr = (Behaviour)vinhoImg;
            bhvr.enabled = true;
            AudioSource.PlayClipAtPoint(somEspeciais, this.gameObject.transform.position);
            Destroy(other.gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Checkpoint") && !checkpoint)
        {
            checkpoint = true;
            AudioSource.PlayClipAtPoint(somCheckpoint, this.gameObject.transform.position);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Estrela") && other.gameObject.tag == "vela")
        {
            getVela = true;
            Behaviour bhvr = (Behaviour)velaImg;
            bhvr.enabled = true;
            AudioSource.PlayClipAtPoint(somEspeciais, this.gameObject.transform.position);
            Destroy(other.gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("armadilha"))
        {
            StartCoroutine(dieRoutine(posInicial));
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("BadGolem"))
        {
            if (GameObject.Find("BadGolem").GetComponent<BadGolem>().isBadAttacking)
            {
                StartCoroutine(dieRoutine(posLuta));
            }
            else if (isAttacking && !GameObject.Find("BadGolem").GetComponent<BadGolem>().isBadAttacking)
            {
                GameObject.Find("BadGolem").GetComponent<BadGolem>().vidas--;
            }
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Entrada"))
        {
            SceneManager.LoadScene("Winner");
        }
    }

    IEnumerator dieRoutine(Vector3 vetor)
    {
        colidiu = true;
        isDying = true;
        AudioSource.PlayClipAtPoint(somKilled, this.gameObject.transform.position);
        yield return new WaitForSeconds(1.5f);
        isDying = false;
        gameObject.transform.position = vetor;
        if (colidiu)
        {
            colidiu = false;
            int temp = PlayerPrefs.GetInt("Vidas", 0);
            Destroy(GameObject.Find("Vida" + temp));
            temp--;
            PlayerPrefs.SetInt("Vidas", temp);
        }

    }
}
