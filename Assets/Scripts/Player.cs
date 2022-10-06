using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [Header("Stats")]
    public int curHP;
    public int maxHP;
    public int damage;
    public float moveSpeed;
    public float interactRange;
    public List<string> inventory = new List<string>();

    private Vector2 facingDirection;

    [Header("Experience")]
    public int curLevel;
    public int curXP;
    public int xpToNextLevel;
    public float levelXpModifier;

    [Header("Combat")]
    public KeyCode attackKey;
    public float attackRange;
    public float attackRate;
    private float lastAttackTime;

    [Header("Sprites")]
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    //components
    private Rigidbody2D rig;
    private SpriteRenderer sr;
    private ParticleSystem hitEffect;
    private PlayerUI ui;

    //awake function is used for statements and commands for objects
    void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        hitEffect = gameObject.GetComponentInChildren<ParticleSystem>();
        ui = FindObjectOfType<PlayerUI>();
    }

    void Start()
    {
        ui.UpdateHealthBar();
        ui.UpdateLevelText();
        ui.UpdateXpBar();
    }

    void Update()
    {
        Move();
        
        if(Input.GetKeyDown(attackKey))
        {
            if(Time.time - lastAttackTime >= attackRate)
                Attack();
        }
        else
        {
            ui.DisableInteractText();
        }

        CheckInteract();
    }

    void CheckInteract()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, facingDirection, interactRange, 1 << 7);

        if(hit.collider != null)
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            ui.SetInteractText(hit.collider.transform.position, interactable.interactDescription);

            if(Input.GetKeyDown(attackKey))
                interactable.Interact();
        }
    }

    void Attack()
    {
        lastAttackTime = Time.time;

        //only detecting objects on the 6th layer
        RaycastHit2D hit = Physics2D.Raycast(transform.position, facingDirection, attackRange, 1 << 6);

        if(hit.collider != null)
        {
            hit.collider.GetComponent<Enemy>().TakeDamage(damage);

            hitEffect.transform.position = hit.collider.transform.position;
            hitEffect.Play();
        }
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        //calculate the velocity we are going to move at
        Vector2 velocity = new Vector2(x, y);

        //calculate facing direction
        if(velocity.magnitude != 0)
            facingDirection = velocity;

        UpdateSpriteDirection();

        //set the velocity
        rig.velocity = velocity * moveSpeed;
    }

    void UpdateSpriteDirection()
    {
        if(facingDirection == Vector2.up)
            sr.sprite = upSprite;
        else if(facingDirection == Vector2.down)
            sr.sprite = downSprite;
        else if(facingDirection == Vector2.left)
            sr.sprite = leftSprite;
        else if(facingDirection == Vector2.right)
            sr.sprite = rightSprite;
    }

    public void TakeDamage(int damageTaken)
    {
        curHP -= damageTaken;

        ui.UpdateHealthBar();

        if(curHP <= 0)
            Die();
    }

    void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void AddXP(int xp)
    {
        curXP += xp;

        ui.UpdateXpBar();

        if(curXP >= xpToNextLevel)
            LevelUp();
    }

    void LevelUp()
    {
        curXP -= xpToNextLevel;
        curLevel++;

        xpToNextLevel = Mathf.RoundToInt((float)xpToNextLevel * levelXpModifier);

        ui.UpdateLevelText();
        ui.UpdateXpBar();
    }

    public void AddItemToInventory(string item)
    {
        inventory.Add(item);
        ui.UpdateInventoryText();
    }

}
