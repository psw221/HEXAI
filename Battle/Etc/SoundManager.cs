using UnityEngine;
using System.Collections;

public class SoundManager {
    private static SoundManager inst = null;
    public AudioClip AC_Attack;
    public AudioClip AC_Music;

    public static SoundManager GetInst()
    {
        if (inst == null)
        {
            inst = new SoundManager();
            inst.Init();
        }

        return inst;
    }

    public void Init()
    {
        AC_Attack = (AudioClip)Resources.Load("Sound/Effect/Crash2");
        AC_Music = (AudioClip)Resources.Load("Sound/Music/Music");
    }

    public void PlayAttackSound(Vector3 pos)
    {
        AudioSource.PlayClipAtPoint(AC_Attack, pos);
    }

    public void PlayMusic(Vector3 pos)
    {
       // AudioSource.PlayClipAtPoint(AC_Music, pos);
    }
}
