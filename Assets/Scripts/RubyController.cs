using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{

    public float movementSpeed = 3.0f;
    public int maxHealth = 5;
    public float timeInvincible = 2.0f;

    public int health { get { return currentHealth; }}
    int currentHealth;

    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    public GameObject projectilePrefab;
    AudioSource audioSource;
    public AudioClip thrownClip;

    public void PlaySound(AudioClip clip)

    {
        audioSource.PlayOneShot(clip);
    }

    bool isInvincible;
    float invincibleTimer;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        Vector2 move = new Vector2(horizontal, vertical);

        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            Launch();

        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NPC character = hit.collider.GetComponent<NPC>();
                if (character != null) {
                    character.DisplayDialog();
                }
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 position = transform.position;
        position.x = position.x + movementSpeed * horizontal * Time.deltaTime;
        position.y = position.y + movementSpeed * vertical * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount) {
        if (amount < 0)
        {
            if (isInvincible)
                return;
            animator.SetTrigger("Hit");
            isInvincible = true;
            invincibleTimer = timeInvincible;
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth/ (float) maxHealth);
    }
    public void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);
        PlaySound(thrownClip);

        animator.SetTrigger("Launch");
    }
}
