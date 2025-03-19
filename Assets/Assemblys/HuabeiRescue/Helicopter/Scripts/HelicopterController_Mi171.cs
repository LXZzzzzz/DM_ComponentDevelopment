using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;

public class HelicopterController_Mi171 : HelicopterController
{
    private Animation zhuan;
    private List<AudioSource> audios;
    private List<WingMark> wingmarks;

    public override void Init(EquipBase baseData, List<ZiYuanBase> sceneAllZiyuan)
    {
        zhuan = GetComponentInChildren<Animation>(true);
        audios = new List<AudioSource>();
        var ass = transform.GetComponentsInChildren<AudioSource>(true);
        for (int i = 0; i < ass.Length; i++)
        {
            audios.Add(ass[i]);
        }

        wingmarks = new List<WingMark>();
        var wms = transform.GetComponentsInChildren<WingMark>(true);
        for (int i = 0; i < wms.Length; i++)
        {
            wingmarks.Add(wms[i]);
        }

        base.Init(baseData, sceneAllZiyuan);
    }

    public override void playanim(bool isPlay)
    {
        if (isPlay)
        {
            foreach (AnimationState state in zhuan)
            {
                if (string.Equals(state.clip.name, "rotor loop"))
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
            wingmarks.ForEach(a => a.gameObject.SetActive(true));
        }
        else
        {
            foreach (AnimationState state in zhuan)
            {
                if (string.Equals(state.clip.name, "rotor stop"))
                {
                    zhuan.clip = state.clip;
                    break;
                }
            }

            zhuan.Play();
            zhuan["AC313A rotorStop"].normalizedTime = 1;
            audios.ForEach(x => x.gameObject.SetActive(false));
        }
    }
}