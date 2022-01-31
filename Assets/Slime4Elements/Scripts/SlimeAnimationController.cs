using UnityEngine;
using System.Collections;

public class SlimeAnimationController : MonoBehaviour {

    internal string _Animation = null;

    public void SetAnimation_Attack()
    {
        _Animation = "Attack";
    }
    public void SetAnimation_Cast()
    {
        _Animation = "Cast";
    }
    public void SetAnimation_Die()
    {
        _Animation = "Die";
    }
    public void SetAnimation_GetHit()
    {
        _Animation = "GetHit";
    }
    public void SetAnimation_Evolve()
    {
        _Animation = "Evolve";
    }
    public void SetAnimation_Run()
    {
        _Animation = "Run";
    }
    public void SetAnimation_Jump()
    {
        _Animation = "Jump";
    }
    public void SetAnimation_SpecialAttack()
    {
        _Animation = "SpecialAttack";
    }
    public void SetAnimation_Upgrade()
    {
        _Animation = "Upgrade";
    }
    public void SetAnimation_Idle()
    {
        _Animation = "Idle";
    }
}
