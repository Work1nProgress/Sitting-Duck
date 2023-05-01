using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats : MonoBehaviour, IEntityHealth, IExperience
{
    [SerializeField] EntityType _entityType;

    [SerializeField] private int _maxHealth;
    private int _health;

    [SerializeField] private int _experience;
    [SerializeField] private int _startLevel;
    [SerializeField] private int _maxLevel;
    private int _currentLevel;

    [SerializeField]private string[] _tags;

    GameManager _gameManager;

    public delegate void EnemyHealthChangeSignature(int oldHealth, int newHealth, int maxHealth);
    public event EnemyHealthChangeSignature OnHealthChanged;

    public delegate void EntityDeathSignature();
    public event EntityDeathSignature OnDeath;

    public delegate void EntityLevelChangeSignature();
    public event EntityLevelChangeSignature OnLevelUpSuccess;
    public event EntityLevelChangeSignature OnLevelUpFail;

    private void Awake()
    {
        _currentLevel = Mathf.Clamp(_startLevel, 1, _maxLevel);
        _experience = Mathf.Abs(_experience);
        _health = _maxHealth;

        GameObject gm = GameObject.Find("GameManager");
        if(gm != false)
        _gameManager = gm.GetComponent<GameManager>();

        if (_gameManager != null)
            _gameManager.AddEntityReference(this, _entityType);
    }

    private void Start() { }

    public void Damage(int ammount)
    {
        int newHealth = _health - ammount;
        newHealth = Mathf.Clamp(newHealth, 0, _maxHealth);

        if (newHealth == 0)
        {
            if(OnDeath != null)
            OnDeath.Invoke();
            return;
        }

        OnHealthChanged.Invoke(_health, newHealth, _maxHealth);
        _health = newHealth;
    }
    public void Heal(int ammount)
    {
        int newHealth = _health + ammount;
        newHealth = Mathf.Clamp(newHealth, 0, _maxHealth);

        if (newHealth == 0)
        {
            OnDeath.Invoke();
            return;
        }
        
        OnHealthChanged.Invoke(_health, newHealth, _maxHealth);
        _health = newHealth;
    }

    public int GetExperienceValue() { return _experience; }
    public void ChangeExperienceValue(int change) { _experience += change; }
    public int GetLevel() { return _currentLevel; }
    public void IncreaseLevel()
    {
        if(_currentLevel + 1 <= _maxLevel)
        {
            _currentLevel++;
            OnLevelUpSuccess.Invoke();
        }
        else { OnLevelUpFail.Invoke(); }
    }

    public bool IsTaggedWith(string searchTag)
    {
        foreach(string tag in _tags)
        {
            if (tag == searchTag)
                return true;
        }

        return false;
    }

    public GameManager GetGameManager() { return _gameManager; }
}

public enum EntityType
{
    Player,
    Enemy
}
