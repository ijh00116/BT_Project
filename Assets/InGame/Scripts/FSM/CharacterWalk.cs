using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWalk : CharacterAbility
{
    protected override void Start()
    {
        base.Start();
    }
    protected override void onEnter()
    {
        Debug.Log("�ȱ� ���� ����");
    }

    protected override void onExit()
    {
        Debug.Log("�ȱ� ���� ����");
    }

    protected override void onUpdate()
    {
        //Walk playing
        Debug.Log("�ȱ� ���� ������");
    }
}
