using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private KeyCode testDamageKey = KeyCode.H;
    [SerializeField] private int testDamageAmount = 10;

    private int currentHealth;
    private bool isDead;

    public bool IsDead
    {
        get { return isDead; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1f;
        currentHealth = maxHealth;
        isDead = false;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        UpdateHealthText();
    }

    // Update is called once per frame
    void Update()
    {
        HandleTestDamageInput();
    }

    private void HandleTestDamageInput()
    {
        if (Input.GetKeyDown(testDamageKey))
        {
            TakeDamage(testDamageAmount);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead == true)
        {
            return;
        }

        if (damageAmount <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);
        UpdateHealthText();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthText()
    {
        if (healthText == null)
        {
            return;
        }

        healthText.text = $"HP {currentHealth} / {maxHealth}";
    }

    private void Die()
    {
        if (isDead == true)
        {
            return;
        }

        isDead = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Heal(int healAmount)
    {
        if(isDead == true)
        {
            return;
        }

        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UpdateHealthText();
    }
}
