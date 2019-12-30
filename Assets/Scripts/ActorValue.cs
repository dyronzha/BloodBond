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

    [SerializeField]
    float _rotateSpeed;
    public float RotateSpeed
    {
        get { return _rotateSpeed; }
        set { _rotateSpeed = value; }
    }

    [SerializeField]
    LayerMask _hurtAreaLayer;
    public LayerMask HurtAreaLayer
    {
        get { return _hurtAreaLayer; }
        set { _hurtAreaLayer = value; }
    }

    public ActorValue(int hp, int damage, float moveSpeed, float rotateSpeed, LayerMask layer) {
        _health = hp;
        _damage = damage;
        _moveSpeed = moveSpeed;
        _rotateSpeed = rotateSpeed;
        _hurtAreaLayer = layer;
    }
}

[System.Serializable]
public class PlayerValue : ActorValue 
{
    public PlayerValue(int hp, int damage, float moveSpeed, float rotateSpeed, LayerMask layer) : base(hp, damage, moveSpeed, rotateSpeed, layer)
    {

    }
    [SerializeField]
    float _dodgeSpeed;
    public float DodgeSpeed
    {
        get { return _dodgeSpeed; }
        set { _dodgeSpeed = value; }
    }
}

[System.Serializable]
public class EnemyValue : ActorValue
{
    public EnemyValue(int hp, int damage, float moveSpeed, float rotateSpeed, LayerMask layer) : base(hp, damage, moveSpeed, rotateSpeed, layer)
    {

    }
}

