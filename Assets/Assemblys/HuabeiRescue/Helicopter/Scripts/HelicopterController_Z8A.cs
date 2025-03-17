using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;

public class HelicopterController_Z8A : HelicopterController
{
    private Animation zhuan;
    private List<AudioSource> audios;

    public override void Init(EquipBase baseData, List<ZiYuanBase> sceneAllZiyuan)
    {
        base.Init(baseData, sceneAllZiyuan);
        zhuan = GetComponentInChildren<Animation>(true);
        audios = new List<AudioSource>();
        var ass = transform.GetComponentsInChildren<AudioSource>(true);
        for (int i = 0; i < ass.Length; i++)
        {
            audios.Add(ass[i]);
        }

        playanim(false);
    }

    public override void playanim(bool isPlay)
    {
        if (isPlay)
        {
            foreach (AnimationState state in zhuan)
            {
                if (string.Equals(state.clip.name, "rotorLoop"))
                {
                    zhuan.clip = state.clip;
                    break;
                }
            }


            audios.ForEach(a => a.volume = 0.3f);
            audios.ForEach(a => a.pitch = 1);
            audios.ForEach(a => a.Play());
            audios.ForEach(x => x.gameObject.SetActive(MyDataInfo.MyLevel == 3));
            zhuan.Play();
        }
        else
        {
            foreach (AnimationState state in zhuan)
            {
                if (string.Equals(state.clip.name, "rotorStop"))
                {
                    zhuan.clip = state.clip;
                    break;
                }
            }

            zhuan.Play();
            zhuan["rotorStop"].normalizedTime = 1;
            audios.ForEach(x => x.gameObject.SetActive(false));
        }
    }
}