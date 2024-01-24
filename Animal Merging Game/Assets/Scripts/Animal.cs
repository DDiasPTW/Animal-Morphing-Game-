using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Animal : ScriptableObject
{
    public Sprite animalSprite;
    public abstract void Activate(Player_Def player); //needs to play a sound when called
    public abstract void ResetAbility(Player_Def player); //should also play a sound when called to give extra feedback to the player
     public virtual void UpdateAbilityState(Player_Def player) { }
     public virtual bool AllowNormalJump() //for the sheep
    {
        return true; // By default, allow normal jumps
    }
}
