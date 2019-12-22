using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorValue
{
    int hp;
    public int HP{
        get { return hp; }
        set { hp = value; }
    }

    int damage;
    public int Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    int moveSpeed;
    public int MoveSpeed {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }

}

public class PlayerValue : ActorValue 
{ 

}

public class EnemyValue : ActorValue
{

}

