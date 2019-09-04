using UnityEngine;
using System.Collections;
using Bow;

public class SequenceAnimation : MonoBehaviour
{
    public float AnimSpeed = 10;  //动画播放速度 默认1秒播放10帧图片
    private float m_animTimeInterval = 0;  //帧与帧间隔的时间

    public SpriteRenderer AnimRenderer;//动画载体的渲染器

    public Sprite[] SpriteArray; //序列帧数组
    private int m_frameIndex = 0;  //帧索引
    private int m_animLength = 0;  //多少帧
    private float m_animTimer = 0; //动画时间计时器

    public int PlayCount = -1;
    private int m_playedCount = 0;
    private bool m_pause = false;
    // Use this for initialization
    void Start()
    {
        m_animTimeInterval = 1 / AnimSpeed;//得到每一帧的时间间隔
        m_animLength = SpriteArray.Length; //得到帧数
    }

    // Update is called once per frame
    void Update()
    {
        m_animTimer += Time.deltaTime;
        if (m_animTimer > m_animTimeInterval && m_pause == false)
        {
            m_animTimer -= m_animTimeInterval;//当计时器减去一个周期的时间
            m_frameIndex++;//当帧数自增（播放下一帧）
            m_frameIndex %= m_animLength;//判断是否到达最大帧数，到了就从新开始  这里是循环播放的
            AnimRenderer.sprite = SpriteArray[m_frameIndex]; //替换图片实现动画
            if (m_frameIndex == 0 && PlayCount != -1)
            {
                m_playedCount++;
            }
            if(PlayCount == m_playedCount)
            {
                m_playedCount = 0;
                m_pause = true;
                gameObject.SetActive(false);
            }
        }
    }

    public void Play()
    {
        m_frameIndex = 0;
        m_animTimer = 0;
        m_playedCount = 0;
        m_pause = false;
        gameObject.SetActive(true);
    }
}

