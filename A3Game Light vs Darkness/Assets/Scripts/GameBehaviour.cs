using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    //protected static GameManager _GM { get { return GameManager.INSTANCE; } }
    //protected static EnemyManager _EM { get { return EnemyManager.INSTANCE; } }
    //protected static UIManager _UI { get { return UIManager.INSTANCE; } }
    //protected static AnimationManager _AM { get { return AnimationManager.INSTANCE; } }

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


}
