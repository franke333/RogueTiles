using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : PersistentSingletonClass<AudioManager>
{
    public enum SFXType
    {
        None,
        TakeDamage,
        Melee,
        Range,
        Magic,
        Heal,
        PickUp
    }

    private AudioSource _audioSource;

    private Dictionary<SFXType, List<AudioClip>> _sfxClips;

    private Dictionary<SFXType, AudioSource> _sfxSources;

    private void LoadClips()
    {
        _sfxClips = new Dictionary<SFXType, List<AudioClip>>();
        int count = 0; 
        foreach(SFXType sfxType in System.Enum.GetValues(typeof(SFXType)))
        {
            if(sfxType == SFXType.None)
                continue;
            var objects = Resources.LoadAll<AudioClip>("Audio/SFX/" + sfxType.ToString());
            var list = new List<AudioClip>();
            foreach (var obj in objects)
            {
                list.Add(obj);
                count++;
            }
            _sfxClips.Add(sfxType, list);
        }
        Log.Debug($"Loaded {count} clips");
    }

    private void CreateSources()
    {

        if (_sfxSources != null && _sfxSources.Count > 0)
            return;

        _sfxSources = new Dictionary<SFXType, AudioSource>();
        foreach (SFXType sfxType in System.Enum.GetValues(typeof(SFXType)))
        {
            if (sfxType == SFXType.None)
                continue;
            var source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            source.spatialBlend = 0;
            source.volume = 0.5f;
            _sfxSources.Add(sfxType, source);
        }
    }

    private void Start()
    {
        if (Instance != this)
            return;
        _audioSource = GetComponent<AudioSource>();
        if(_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        LoadClips();
        CreateSources();
    }

    public void PlaySFX(SFXType sfxType)
    {
        if(sfxType == SFXType.None)
            return;
        var list = _sfxClips[sfxType];
        if (list == null || list.Count == 0)
            return;
        var source = _sfxSources[sfxType];
        if (source == null)
            return;
        var clip = MyRandom.Choice(list);
        source.clip = clip;
        source.Play();
    }
}
