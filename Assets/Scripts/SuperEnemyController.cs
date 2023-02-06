using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SuperEnemyController : EnemyController
{

    #region var definitions
    public bool angry;
    #endregion


    protected new void Start()
    {
        base.Start();
        hpMax += 5;
        hp = hpMax;
        attack -= 2.5f;
        this.angry = LevelManager.angry;
    }

    protected new void Update()
    {
        hpBarLocalScale.x = 0.6f * (hp / hpMax);
        hpBar.transform.localScale = hpBarLocalScale;
        Flip();
        if (angry)
        {
            Chase();
        }
        else
        {
            this.angry = LevelManager.angry;
        }
    }

    protected new void OnTriggerEnter2D(Collider2D other)
    {
        if (angry) { 
            base.OnTriggerEnter2D(other);
        }
    }

    protected new void OnTriggerStay2D(Collider2D other)
    {
        if (angry)
        {
            base.OnTriggerEnter2D(other);
        }

    }

    public new bool TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            if (!angry)
            {
                angry = true;
                LevelManager.angry = true;
            }
            Destroy(this.gameObject);
            return true;
        }
        else
        {
            foreach (var spriteRenderer in spriteRenderers)
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
            StartCoroutine(FadeToYellow());
            return false;
        }
    }

    protected IEnumerator FadeToYellow()
    {
        while (spriteRenderers[0].color != Color.yellow)
        {
            yield return null;
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.yellow, Time.deltaTime * 3);
            }
        }
    }
}
