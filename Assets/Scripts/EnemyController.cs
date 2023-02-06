using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{

    #region var definitions
    protected Rigidbody2D rigidBody;
    protected float attack;
    protected bool facingRight;

    [Header("HP")]
    protected float hpMax;
    protected float hp;
    [SerializeField] protected GameObject hpBar;
    protected Vector3 hpBarLocalScale;
    [SerializeField] protected SpriteRenderer[] spriteRenderers;

    [Header("AI")]
    [SerializeField] protected float speed = 3;
    [SerializeField] protected Transform target;
    [SerializeField] protected float minDistance = 1.0f;
    [SerializeField] protected float maxDistance = 7.0f;

    #endregion


    protected void Start()
    {
        hpMax = LevelManager.enemyHp;
        hp = hpMax;
        attack = LevelManager.enemyAttack;
        rigidBody = GetComponent<Rigidbody2D>();
        hpBarLocalScale = hpBar.transform.localScale;
        target = GameObject.FindGameObjectWithTag(Globals.PLAYER_TAG).transform;
    }

    protected void Update()
    {
        hpBarLocalScale.x = 0.6f * (hp/hpMax);
        hpBar.transform.localScale = hpBarLocalScale;
        Flip();
        Chase();
    }

    protected void LateUpdate()
    {
        DieByFalling();
    }

    protected void Chase()
    {
        if ((Vector2.Distance(transform.position, target.position) > minDistance) && (Vector2.Distance(transform.position, target.position) < maxDistance))
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }

    protected void Flip()
    {
        if (((transform.position.x - target.position.x >= 0) && facingRight) || ((transform.position.x - target.position.x < 0) && !facingRight))
        {
            facingRight = !facingRight;
            this.GetComponent<SpriteRenderer>().flipX = !this.GetComponent<SpriteRenderer>().flipX;
        }
    }
    protected void DieByFalling()
    {
        if (transform.position.y <= -40)
        {
            Destroy(this.gameObject);
        }
    }
    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == Globals.PLAYER_TAG)
        {
            other.GetComponent<PlayerController>().TakeDamage(attack, this.transform.position.x);
        }
    }

    protected void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == Globals.PLAYER_TAG)
        {
            other.GetComponent<PlayerController>().TakeDamage(attack, this.transform.position.x);
        }

    }

    public bool TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Destroy(this.gameObject);
            return false;
        }
        else
        {
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.color = Color.red;
            }

            if (facingRight)
            {
                rigidBody.AddForce(Vector2.left * speed * 2, ForceMode2D.Impulse);
            }
            else
            {
                rigidBody.AddForce(Vector2.right * speed * 2, ForceMode2D.Impulse);
            }
            StartCoroutine(FadeToWhite());
            return false;
        }
    }

    protected IEnumerator FadeToWhite()
    {
        while (spriteRenderers[0].color != Color.white)
        {
            yield return null;
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.white, Time.deltaTime * 3);
            }
        }
    }

    protected void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == Globals.PLAYER_TAG)
        {
            other.rigidbody.AddForce(Vector2.up * speed, ForceMode2D.Impulse);
        }
    }

}
