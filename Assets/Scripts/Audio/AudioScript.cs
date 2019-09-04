using UnityEngine;
using System.Collections;


namespace Scripts.Audio
{
    public class AudioScript : UIPlaySound
    {
        public bool AutoPlayAwake = false;

        void Start()
        {
            if (AutoPlayAwake)
            {
                volume = AudioManager.SoundVolumn;
                Play();
            }
        }

        void Update()
        {
            volume = AudioManager.SoundVolumn;
        }
    }
}