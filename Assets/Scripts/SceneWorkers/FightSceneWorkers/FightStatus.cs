using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus
{
    public string name { get; protected set; }
    public int maxHp { get; protected set; }
    public int remainHp { get; protected set; }
    public int atk { get; protected set; }

    public CharacterStatus(string p_Name, int p_maxHp, int p_remainHp, int p_Atk)
    {
        name = p_Name;
        maxHp = p_maxHp;
        remainHp = p_remainHp;
        atk = p_Atk;
    }

    public bool DamageToCharacter(int damage)
    {
        remainHp -= damage;

        if (remainHp <= 0)
        {
            remainHp = 0;
            return true;
        }
        return false;
    }

    public void HealToCharacter(int heal)
    {
        remainHp += heal;

        if (remainHp > maxHp)
        {
            remainHp = maxHp;
        }
    }
}

public class EnemyCharacterStatus : CharacterStatus
{
    public int dropParts { get; private set; }

    public EnemyCharacterStatus(string p_Name, int p_maxHp, int p_remainHp, int p_Atk, int p_Parts) 
        : base(p_Name, p_maxHp, p_remainHp, p_Atk)
    {
        dropParts = p_Parts; 
    }
}

public class FightStatus
{
    public int remainEnemy { get; private set; }
    public int gainParts { get; private set; }

    public CharacterStatus playerStatus { get; private set; }
    public Dictionary<GameObject, EnemyCharacterStatus> enemies { get; private set; }

    public FightStatus()
    {
        remainEnemy = 0;
        gainParts = 0;

        enemies = new Dictionary<GameObject, EnemyCharacterStatus>();
    }

    public void SetRemainEnemy(int num)
    {
        remainEnemy = num;
    }

    public void SetPlayerStatus(CharacterStatus status)
    {
        playerStatus = status;
    }
    public void AddEnemyInstance(GameObject enemy, EnemyCharacterStatus status)
    {
        enemies.Add(enemy, status);
    }

    public void RemoveEnemyInstance(GameObject enemy)
    {
        if (enemies.ContainsKey(enemy))
        {
            //enemies.Remove(enemy);
            remainEnemy--;
        }
    }

    public void AddParts(int parts)
    {
        gainParts += parts;
    }
}
