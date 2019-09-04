using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaFramework;
using Scripts;

public class AudioManager
{
    /// <summary>
    /// 是否静音(只用来控制音乐）
    /// </summary>
    public static float Volumn = 1.0f;

    /// <summary>
    /// 是否静音(只用来控制音效）
    /// </summary>
    public static float SoundVolumn = 1.0f;

    /// <summary>
    /// 缓存所有音乐文件
    /// </summary>
    private Dictionary<string, AudioClip> m_soundsDic;

    private string m_bgMusicName = "";

    private Task m_setBgMusicTask;//切换背景音乐用的
    private AudioSource m_bgMusic;//背景音乐
    private AudioManager()
    {
        Volumn = LuaToCSFunction.GetMusic() ? 1 : 0;
        SoundVolumn = LuaToCSFunction.GetMusic() ? 1 : 0;
        m_soundsDic = new Dictionary<string, AudioClip>();
    }

    private AudioSource GetAudioSoundSource(AudioClip clip, bool isAutoDestroy)
    {
        GameObject go = new GameObject(clip.name);
        go.transform.parent = AppConst.GameManager;
        AudioSource source = Util.AddComponent<AudioSource>(go);
        source.clip = clip;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.volume = SoundVolumn;
        source.Play();

        if (isAutoDestroy)
        {
            GameObject.Destroy(go, clip.length);
        }

        return source;
    }

    public AudioSource PlaySound(string soundName, bool isAutoDestroy = true, bool isLocal = true)
    {
        isAutoDestroy = true;
        AudioSource source = null;
        //if (SoundVolumn > 0)
        //{
        AudioClip clip = null;
        if (m_soundsDic.ContainsKey(soundName))
        {
            clip = m_soundsDic[soundName];
        }
        else
        {
            //这里修改加载方式
            clip = ObjectPoolManager.Instance.LoadAudioClipByName(soundName, isLocal);
            m_soundsDic[soundName] = clip;
        }
        if (clip != null)
        {
            source = GetAudioSoundSource(clip, isAutoDestroy);
        }
        //}
        return source;
    }

    public AudioSource Play(string soundName, bool isAutoDestroy = true, bool isLocal = true)
    {
        AudioSource source = null;
        //if (Volumn > 0)
        //{
            AudioClip clip = null;
            if (m_soundsDic.ContainsKey(soundName))
            {
                clip = m_soundsDic[soundName];
            }
            else
            {
                //这里修改加载方式
                clip = ObjectPoolManager.Instance.LoadAudioClipByName(soundName, isLocal);
                m_soundsDic[soundName] = clip;
            }
            if (clip != null)
            {
                source = GetAudioSource(clip, isAutoDestroy);
            }
        //}
        return source;
    }

    private AudioSource GetAudioSource(AudioClip clip, bool isAutoDestroy)
    {
        GameObject go = new GameObject(clip.name);
        go.transform.parent = AppConst.GameManager;
        AudioSource source = Util.AddComponent<AudioSource>(go);
        source.clip = clip;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.volume = Volumn;
        source.Play();

        if (isAutoDestroy)
        {
            GameObject.Destroy(go, clip.length);
        }

        return source;
    }

    /// <summary>
    /// PlayBgMusic函数只用于加载背景音乐
    /// </summary>
    /// <param name="name"></param>
    /// <param name="isAutoDestroy"></param>
    public void PlayBgMusic(string name, bool isAutoDestroy, bool loop, bool isLocal = false)
    {
        //Debug.Log("背景音乐：" + name);
        if (m_bgMusicName.CompareTo(name) != 0)
        {
            m_bgMusicName = name;
            if (m_bgMusic != null)
            {
                if (m_setBgMusicTask != null)
                {
                    m_setBgMusicTask.Stop();
                }
                TweenVolume.Begin(m_bgMusic.gameObject, 1, 0);
                m_setBgMusicTask = new Task(SetBgMusicDispear(name, isAutoDestroy, loop));
            }
            else
            {
                CreatBgMusic(name, isAutoDestroy, loop, isLocal);
            }
        }
    }

    private IEnumerator SetBgMusicDispear(string name, bool isAutoDestroy, bool loop)
    {
        yield return new WaitForSeconds(1);
        
        if (m_bgMusic != null)
        {
            GameObject.Destroy(m_bgMusic.gameObject);
            m_bgMusic = null;
        }
        CreatBgMusic(name, isAutoDestroy, loop);

//         float voiceNum = m_bgMusic.volume;
//         while (voiceNum > 0)
//         {
//             voiceNum -= 0.02f;
//             if (m_bgMusic != null)
//             {
//                 m_bgMusic.volume = voiceNum;
//                 yield return new WaitForSeconds(0.03f);
//             }
//             else
//             {
//                 yield return null;
//             }
//         }
//         if (m_bgMusic != null)
//         {
//             GameObject.Destroy(m_bgMusic.gameObject);
//             m_bgMusic = null;
//         }
//         CreatBgMusic(name, isAutoDestroy, loop);
    }

    private void CreatBgMusic(string name, bool isAutoDestroy, bool loop, bool isLocal = true)
    {
        m_bgMusic = Play(name, isAutoDestroy, isLocal);
        if (m_bgMusic != null)
        {
            m_bgMusic.loop = loop;
        }
    }

    internal void SetBgMusicVolumn(bool openFlag)
    {
        if (m_bgMusic != null)
        {
            Volumn = openFlag ? 1 : 0;
            m_bgMusic.volume = Volumn;
        }
    }

    internal void SetSoundVolumn(bool openFlag)
    {
        SoundVolumn = openFlag ? 1 : 0;
    }

    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AudioManager();
            }
            return _instance;
        }
    }
}
