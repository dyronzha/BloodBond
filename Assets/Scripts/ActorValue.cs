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

    public ActorValue(int hp, int damage, float moveSpeed, float rotateSpeed, LayerMask hurtAreaLayer) {
        _health = hp;
        _damage = damage;
        _moveSpeed = moveSpeed;
        _rotateSpeed = rotateSpeed;
        _hurtAreaLayer = hurtAreaLayer;
    }
}

[System.Serializable]
public class PlayerValue : ActorValue 
{
    public PlayerValue(int hp, int damage, float moveSpeed, float rotateSpeed, LayerMask hurtAreaLayer) : base(hp, damage, moveSpeed, rotateSpeed, hurtAreaLayer)
    {

    }
    [SerializeField]
    float _dodgeSpeed;
    [SerializeField]
    float _dodgeTime;
    public float DodgeSpeed
    {
        get { return _dodgeSpeed; }
        set { _dodgeSpeed = value; }
    }
    public float DodgeTime
    {
        get { return _dodgeTime; }
        set { _dodgeTime = value; }
    }
}

[System.Serializable]
public class EnemyValue : ActorValue
{
    [SerializeField]
    float sightDist = .0f;
    public float SightDistance
    {
        get { return sightDist; }
        set { sightDist = value; }
    }
    [SerializeField]
    float sightAngle = .0f;
    public float SightAngle
    {
        get { return sightAngle; }
        set { sightAngle = value; }
    }
    [SerializeField]
    float seeConfirmTime = .0f;
    public float SeeConfirmTime
    {
        get { return seeConfirmTime; }
        set { seeConfirmTime = value; }
    }
    [SerializeField]
    float attackDist = .0f;
    public float AttackDist
    {
        get { return attackDist; }
        set { attackDist = value; }
    }
    public EnemyValue(int hp, int damage, float moveSpeed, float rotateSpeed, float _sightDist, float _sightAngle, float seeTime,float atkDist, LayerMask hurtAreaLayer) : base(hp, damage, moveSpeed, rotateSpeed, hurtAreaLayer)
    {
        sightDist = _sightDist;
        sightAngle = _sightAngle;
        seeConfirmTime = seeTime;
        attackDist = atkDist;
    }
}

[System.Serializable]
public class ObjectValue {
    [SerializeField]
    float lifeTime = 0;
    public float LifeTime
    {
        get { return lifeTime; }
        set { lifeTime = value; }
    }
    [SerializeField]
    float speed = .0f;
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }
    [SerializeField]
    int hurtValue = 0;
    public int HurtValue
    {
        get { return hurtValue; }
        set { hurtValue = value; }
    }
}