using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActorValue
{
    [SerializeField]
    int _health;
    public int Health{
        get { return _health; }
        set { _health = value; }
    }

    [SerializeField]
    int _damage;
    public int Damage
    {
        get { return _damage; }
        set { _damage = value; }
    }

    [SerializeField]
    float _moveSpeed;
    public float MoveSpeed {
        get { return _moveSpeed; }
        set { _moveSpeed = value; }
    }

    public ActorValue(int hp, int damage, float moveSpeed) {
        _health = hp;
        _damage = damage;
        _moveSpeed = moveSpeed;
    }
}

[System.Serializable]
public class PlayerValue : ActorValue 
{
    public PlayerValue(int hp, int damage, float moveSpeed) : base(hp, damage, moveSpeed)
    {

    }
}

public class EnemyValue : ActorValue
{
    public EnemyValue(int hp, int damage, float moveSpeed) : base(hp, damage, moveSpeed)
    {

    }
}

