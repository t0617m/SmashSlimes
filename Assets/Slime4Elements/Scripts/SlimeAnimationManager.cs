using UnityEngine;
using System.Collections;

public class SlimeAnimationManager : MonoBehaviour {

    private Animator SlimeAnimator; //create animator
    internal string _SlimeAnimation = null; //Animation name
    private SlimeAnimationController _SlimeAnimationController; //Animation UI
    internal string _SlimeLastAnimation = null; //Last animation

    // Use this for initialization
    void Start () {
        SlimeAnimator = this.gameObject.GetComponent<Animator>();
        _SlimeAnimationController = GameObject.Find("SlimeAnimationController").GetComponent<SlimeAnimationController>();
    }

    void GetAnimation() {
        _SlimeLastAnimation = _SlimeAnimation;
        if (_SlimeAnimation == null) {
            _SlimeAnimation = "Idle";
        }
        else {
            _SlimeAnimation = _SlimeAnimationController._Animation;
        }
    }

    void SetAllAnimationFlatsFalse() {
        SlimeAnimator.SetBool("idleToAttack", false);
        SlimeAnimator.SetBool("idleToCast", false);
        SlimeAnimator.SetBool("idleToDie", false);
        SlimeAnimator.SetBool("idleToEvolve", false);
        SlimeAnimator.SetBool("idleToGetHit", false);
        SlimeAnimator.SetBool("idleToRun", false);
        SlimeAnimator.SetBool("idleToJump", false);
        SlimeAnimator.SetBool("idleToSpecialAttack", false);
        SlimeAnimator.SetBool("idleToUpgrade", false);
    }

    void SetAnimation() {
        SetAllAnimationFlatsFalse();

        if (_SlimeAnimation == "Idle") {
            SlimeAnimator.SetBool("idleToAttack", false);
            SlimeAnimator.SetBool("idleToCast", false);
            SlimeAnimator.SetBool("idleToDie", false);
            SlimeAnimator.SetBool("idleToEvolve", false);
            SlimeAnimator.SetBool("idleToGetHit", false);
            SlimeAnimator.SetBool("idleToRun", false);
            SlimeAnimator.SetBool("idleToJump", false);
            SlimeAnimator.SetBool("idleToSpecialAttack", false);
            SlimeAnimator.SetBool("idleToUpgrade", false);
        }
        else if (_SlimeAnimation == "Attack")
        {
            SlimeAnimator.SetBool("idleToAttack", true);
        }
        else if (_SlimeAnimation == "Cast")
        {
            SlimeAnimator.SetBool("idleToCast", true);
        }
        else if (_SlimeAnimation == "Die")
        {
            SlimeAnimator.SetBool("idleToDie", true);
        }
        else if (_SlimeAnimation == "Evolve")
        {
            SlimeAnimator.SetBool("idleToEvolve", true);
        }
         else if (_SlimeAnimation == "GetHit")
        {
            SlimeAnimator.SetBool("idleToGetHit", true);
        }
        else if (_SlimeAnimation == "SpecialAttack")
        {
            SlimeAnimator.SetBool("idleToSpecialAttack", true);
        }
        else if (_SlimeAnimation == "Jump")
        {
            SlimeAnimator.SetBool("idleToJump", true);
        }
        else if (_SlimeAnimation == "Upgrade")
        {
            SlimeAnimator.SetBool("idleToUpgrade", true);
        }
        else if (_SlimeAnimation == "Run")
        {
            SlimeAnimator.SetBool("idleToRun", true);
        }
    }

    void RetureToIdle() {
        if (SlimeAnimator.GetCurrentAnimatorStateInfo(0).IsName(_SlimeAnimation)) {
            if(_SlimeAnimation != "Run" && 
               _SlimeAnimation != "Die"){
                SetAllAnimationFlatsFalse();
                SlimeAnimator.SetBool("idletoDefualt",true);
            }
            
        }
    }
	// Update is called once per frame
	void Update () {
        GetAnimation();

        if (_SlimeLastAnimation != _SlimeAnimation)
        {
            SetAnimation();
        }
        else {
            RetureToIdle();
        }
    }
}
