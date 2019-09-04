using Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDragAndDrop : UIDragDropItem 
{
    public bool CanDrag = true;
    private GameObject sourceParent;
    private Vector3 prePos;
    public int ItemID;
    void Awake()
    {
        enabled = CanDrag;
    }

    protected override void OnDragDropStart()
    {
        //当拖拽开始时存储原始的父对象
        sourceParent = transform.parent.gameObject;
        prePos = transform.localPosition;
        LuaToCSFunction.CallToLuaFunction("OnDragDropStart", ItemID);
        base.OnDragDropStart();
    }

    //该方法用于获取拖拽的物体释放拖拽时，该物体所碰撞的对象
    //所以我们前面需要给Cell和Obj都添加Box Collider
    protected override void OnDragDropRelease(GameObject surface)
    {
        base.OnDragDropRelease(surface);
        LuaToCSFunction.CallToLuaFunction("OnDragDropRelease", gameObject, surface, ItemID);
    }
}
