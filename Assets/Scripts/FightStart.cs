using Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class FightStart : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            LuaToCSFunction.CallToLuaFunction("EnterFightScene");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}