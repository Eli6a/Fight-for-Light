using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region var definitions

    [Header("Movement")]
    private float direction;
    private static float runSpeed = 7.0f;
    private static float walkSpeed = 1.0f;
    [SerializeField] private float speed = runSpeed;
    [SerializeField] private float jumpAmount = 12;
    private bool isOnGround = true;
    private int groundLayerMask;
    private int enemyLayerMask;


    [Header("Player")]
    public Rigidbody2D rigidBody;
    public static float hpMax = 20;
    public static float hp;
    private float knockBack = 20;

    [Header("Attack")] 
    public float attack = 5;
    [SerializeField] private Transform attackPoint;
    private Vector2 attackRange = new Vector2(1.5f, 2.5f);
    private float attackRate = 4.0f;
    float nextAttackTime = 0f;
    private bool enemiesClear;
    private bool superEnemiesClear;

    [Header("Animation")]
    private Animator animator;
    private bool facingRight = true;

    [Header("Invincible Frame")]
    public float invincibilityLength;
    private float invincibilityCounter;
    [SerializeField] private SpriteRenderer[] spriteRenderers;

    [Header("UI")]
    public GameObject inGameUI;
    public GameObject gameOverUI;
    public GameObject winUI;
    public GameObject pauseUI;
    private bool pauseState;
    [SerializeField] private UnityEngine.UI.Image hpImage;
    [SerializeField] private TextMeshProUGUI hpText;
    public AudioManager audioManager;



    void Awake()
    {
        groundLayerMask = LayerMask.GetMask(Globals.GROUND_LAYER);
        enemyLayerMask = LayerMask.GetMask(Globals.ENEMY_LAYER);
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        inGameUI.SetActive(true);
        gameOverUI.SetActive(false);
        winUI.SetActive(false);
        pauseUI.SetActive(false);
        enemiesClear = false;
        superEnemiesClear = false;
        Time.timeScale = 1f;
    }

    void Start()
    {
        hp = hpMax;
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    #endregion

    // Update is called once per frame
    void Update()
    {
        WallCheck();
        Move();    
        ActionInputControl();

        hpImage.fillAmount = (float) hp / hpMax;
        hpText.text = hp + "    " + hpMax;

        if (invincibilityCounter > 0)
        {
            invincibilityCounter -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        Flip();
        EnemyClear();
        DieByFalling();
    }

    private void Move()
    {
        direction = Input.GetAxis(Globals.HORIZONTAL_AXIS);
        transform.Translate(Vector3.right * direction * Time.deltaTime * speed);
        animator.SetFloat("Speed", speed);
        animator.SetFloat("Direction", Mathf.Abs(direction));
    }
    private void Flip()
    {
        if ((direction < 0 && facingRight) || (direction > 0 && !facingRight))
        {
            facingRight = !facingRight;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    private void Jump()
    {
        if (isOnGround)
        {
            rigidBody.AddForce(Vector2.up * jumpAmount, ForceMode2D.Impulse);
            isOnGround = false;
            animator.Play("Player_jump");
        }
    }

    private void WallCheck()
    {
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position - new Vector3(0, 0.2f, 0), Vector2.right, Globals.RAYCAST_CHECK_RANGE, groundLayerMask);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position - new Vector3(0, 0.2f, 0), Vector2.left, Globals.RAYCAST_CHECK_RANGE, groundLayerMask);
        RaycastHit2D hitRightUp = Physics2D.Raycast(transform.position + new Vector3(0, 0.5f, 0), Vector2.right, Globals.RAYCAST_CHECK_RANGE, groundLayerMask);
        RaycastHit2D hitLeftUp = Physics2D.Raycast(transform.position + new Vector3(0, 0.5f, 0), Vector2.left, Globals.RAYCAST_CHECK_RANGE, groundLayerMask);
        if (((hitRight.collider != null || hitRightUp.collider != null) && direction > 0) || ((hitLeft.collider != null || hitLeftUp.collider != null) && direction < 0))  //this is ugly code; better way?
        {
            direction = 0;
        }
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint.position, attackRange, 0, enemyLayerMask);

        if (hitEnemies.Length > 0)
        {
            foreach (Collider2D enemy in hitEnemies)
            {
                try
                {
                    bool bonus = enemy.GetComponent<SuperEnemyController>().TakeDamage(attack);
                    if (bonus)
                    {
                        attack += LevelManager.enemyAttack/2;
                        hpMax += LevelManager.enemyAttack;

                        if (hp + LevelManager.enemyAttack > hpMax)
                        {
                            hp = hpMax;
                        }
                        else
                        {
                            hp += LevelManager.enemyAttack;
                        }
                        LevelManager.SetPlayerAttackHP(attack, hpMax);
                    }
                }
                catch
                {
                    enemy.GetComponent<EnemyController>().TakeDamage(attack);
                }

            }
        }
    }

    private void EnemyClear()
    {

        if (!enemiesClear)
        {
            try
            {
                GameObject child = GameObject.FindGameObjectWithTag(Globals.ENEMIES_GROUP_TAG).transform.GetChild(0).gameObject;
            }
            catch
            {
                enemiesClear = true;
            }
        }

        if (!superEnemiesClear)
        {
            try
            {
                GameObject child = GameObject.FindGameObjectWithTag(Globals.SUPER_ENEMIES_GROUP_TAG).transform.GetChild(0).gameObject;
            }
            catch
            {
                superEnemiesClear = true;
            }
        }

        if (enemiesClear && (!LevelManager.angry || (superEnemiesClear && LevelManager.angry))){
            inGameUI.SetActive(false);
            winUI.SetActive(true);
            Time.timeScale = 0f;
        }

    }

    private void ActionInputControl()
    {
        // inputs are keys of letters, numbers or space
        var input = Input.inputString;
        if (!string.IsNullOrEmpty(input))
        {
            switch (input)
            {
                case " ":
                    Jump();
                    break;   
                case "s":
                    animator.Play("Player_crouch");
                    break;
                case "f":
                    animator.Play("Player_fall");
                    ResetAnimator();
                    break;
                case "p":
                    if (!pauseState)
                    {
                        Pause();
                    }
                    else
                    {
                        Resume();
                    }
                    break;

            }

        }

        // inputs are other keys
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            speed = walkSpeed;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            animator.Play("Player_crouch");
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseState)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }

        // input are releasing
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            speed = runSpeed;
        }
            
    }

    private void ResetAnimator()
    {
        animator.enabled = false;
        animator.enabled = true;
        animator.Play("Player");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == Globals.GROUND_TAG && !isOnGround)
        {
            ResetAnimator();
            isOnGround = true;
        }
    }

    public void TakeDamage(float damage, float enemyPosition)
    {
        if (invincibilityCounter <= 0) {

            hp = Mathf.RoundToInt(hp - damage);

            if (hp <= 0)
            {
                GameOver();
            }

            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.color = Color.red;
            }

            if (this.transform.position.x < enemyPosition)
            {
                this.rigidBody.AddForce(new Vector2(-knockBack * speed, 0f), ForceMode2D.Impulse);
            }
            else
            {
                this.rigidBody.AddForce(new Vector2(knockBack * speed, 0f), ForceMode2D.Impulse);
            }
            
            invincibilityCounter = invincibilityLength;
            StartCoroutine(FadeToWhite());
        }
    }

    private void DieByFalling()
    {
        if (transform.position.y <= -40)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        gameOverUI.SetActive(true);
        inGameUI.SetActive(false);
        audioManager.Stop();
        Time.timeScale = 0f;

    }

    protected IEnumerator FadeToWhite()
    {
        while (spriteRenderers[0].color != Color.white)
        {
            yield return null;
            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.white, Time.deltaTime * 3);
            }
        }
    }

    public void Pause()
    {
        pauseState = true;
        inGameUI.SetActive(false);
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        audioManager.Pause();
    }

    public void Resume()
    {
        inGameUI.SetActive(true);
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        pauseState = false;
        audioManager.UnPause();
    }

    public void SetVolume(float volume)
    {
        audioManager.SetVolume(volume);
    }


    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        else
            Gizmos.DrawWireCube(new Vector3(attackPoint.position.x, attackPoint.position.y) , new Vector3(attackRange.x, attackRange.y));
    }

}
