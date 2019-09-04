using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 对于所有需要修改某个对象下面的某个属性值，需要动态修改的情况下使用
/// </summary>
public class TweenProperty : UITweener
{
    private string m_property;
    public string Property
    {
        get { return m_property; }
        set { m_property = value; }
    }

    private float m_from = 1;
    public float From
    {
        get { return m_from; }
        set { m_from = value; }
    }

    private float m_to = 1;
    public float To
    {
        get { return m_to; }
        set { m_to = value; }
    }


    private float m_duration = 1;
    public float Duration
    {
        get { return m_duration; }
        set { m_duration = value; }
    }

    private MonoBehaviour m_target;
    public MonoBehaviour Target
    {
        get { return m_target; }
        set { m_target = value; }
    }

    public float value
    {
        set
        {
            m_target.GetType().GetProperty(m_property).SetValue(m_target, value, null);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    /// <param name="property">属性名称</param>
    /// <param name="duration">修改数据需要的时间</param>
    /// <param name="from">开始修改的数据</param>
    /// <param name="to">最后修改的数据</param>
    public static TweenProperty Begin<T>(GameObject go, Hashtable table) where T : MonoBehaviour
    {
        TweenProperty comp = UITweener.Begin<TweenProperty>(go, (float)table["duration"]);
        comp.Property = (string)table["property"];
        comp.From = (float)table["from"];
        comp.To = (float)table["to"];
        comp.Duration = (float)table["duration"];
        comp.Target = go.GetComponent<T>();

        return comp;
    }


    public static TweenProperty BeginTween(GameObject go, string className, params object[] args)
    {
        string property = (string)args[0];
        float from = Convert.ToSingle(args[1]);
        float to = Convert.ToSingle(args[2]);
        float duration = Convert.ToSingle(args[3]);
        TweenProperty comp = UITweener.Begin<TweenProperty>(go, duration);
        comp.Property = property;
        comp.From = from;
        comp.To = to;
        comp.Duration = duration;
        comp.Target = go.GetComponent(className) as MonoBehaviour;

        return comp;
    }

    // Update is called once per frame
    protected override void OnUpdate (float factor, bool isFinished)
    {
        if (m_target != null)
        {
            value = m_from * (1f - factor) + m_to * factor;
        }
    }
}
