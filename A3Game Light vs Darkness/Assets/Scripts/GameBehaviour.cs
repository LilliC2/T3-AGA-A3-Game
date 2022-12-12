using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    //protected static GameManager _GM { get { return GameManager.INSTANCE; } }
    protected static EnemyManager _EM { get { return EnemyManager.INSTANCE; } }
    protected static UIManager _UI { get { return UIManager.INSTANCE; } }

    protected static ThirdPersonMovement _P { get { return ThirdPersonMovement.INSTANCE; } }
    protected static CameraSwap _CS { get { return CameraSwap.INSTANCE; } }

    public float RandomFloatBetwenTwoFloats(float _float1, float _float2)
    {
        float result =Random.Range(_float1, _float2);

        return result;
    }

    public int RandomFloatBetwenTwoInts(int _int1, int _int2)
    {
        int result = (int) Random.Range(_int1, _int2);

        return result;
    }

    public void AnimationTrigger(string _anim)
    {
        Animator animation;
        animation = GetComponent<Animator>();
        animation.SetTrigger(_anim);
    }

    public IEnumerator PlayAnimationBool(string _animName)
    {
        Animator animation;
        //print(_animName);
        animation = GetComponent<Animator>();
        animation.SetBool(_animName, true);
        yield return new WaitForSeconds(animation.GetCurrentAnimatorStateInfo(0).length);
        animation.SetBool(_animName, false);
    }


}
