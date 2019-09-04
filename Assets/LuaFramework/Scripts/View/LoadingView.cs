using LuaFramework;
using Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingView
{
    protected GameObject ui = null;
    private UISlider m_Slider = null;
	// Use this for initialization
    public void Init()
    {
        if (LuaToCSFunction.IsIphoneX())
        {
            Camera cam = AppConst.UIRoot.Find("Camera").GetComponent<Camera>();
            LuaToCSFunction.SetUICameraRect(cam, 0, 0.04f, 1, 0.92f);
        }
        ui = ObjectPoolManager.Instance.LoadObjectByName("Prefabs/Common/LoadingView", AppConst.UIRoot);
        ui.transform.localPosition = Vector3.zero;
        ui.transform.localScale = Vector3.one;

        m_Slider = ui.transform.Find("Slider").GetComponent<UISlider>();
        TweenProperty por = TweenProperty.BeginTween(m_Slider.gameObject, "UISlider", "value", 0.01, 1, 10);
        por.ignoreTimeScale = false;
	}

    public void UpdateProgress(float num, int sum)
    {
        //if (m_Slider == null) return;
        //m_Slider.value = num / sum;
    }
    public void TweenComplete()
    {
        TweenProperty por = TweenProperty.BeginTween(m_Slider.gameObject, "UISlider", "value", m_Slider.value, 1, 1);
    }

    public void Destroy()
    {
        ObjectPoolManager.Instance.PoolDestroy(ui);
        LuaFramework.Util.ClearMemory();
        LuaToCSFunction.CallToLuaFunction("LoadingComplete");
    }

}
