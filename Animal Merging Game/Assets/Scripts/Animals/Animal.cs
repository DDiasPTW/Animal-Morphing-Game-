using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Animal : ScriptableObject
{
    public Sprite animalSprite;
    public abstract void Activate(Player_Def player);
    public abstract void ResetAbility(Player_Def player);
     public virtual void UpdateAbilityState(Player_Def player) { }
    public virtual bool AllowNormalJump() //for the sheep
    {
        return true; // By default, allow normal jumps
    }

    // Coroutine for smoothly changing speed over time
    protected IEnumerator ChangeSpeed(Player_Def player, float targetSpeed, float timer)
    {
        float startSpeed = player.moveSpeed;
        float elapsedTime = 0;

        while (elapsedTime < timer)
        {
            player.moveSpeed = Mathf.Lerp(startSpeed, targetSpeed, elapsedTime / timer);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        player.moveSpeed = targetSpeed; // Ensure exact target speed is set at the end
    }
}
