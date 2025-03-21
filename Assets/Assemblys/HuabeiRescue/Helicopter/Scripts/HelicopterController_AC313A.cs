using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;

public class HelicopterController_AC313A : HelicopterController
{
    private Animation zhuan;
    private List<AudioSource> audios;

    public override void Init(EquipBase baseData, List<ZiYuanBase> sceneAllZiyuan)
    {
        zhuan = GetComponentInChildren<Animation>(true);
        audios = new List<AudioSource>();
        var ass = transform.GetComponentsInChildren<AudioSource>(true);
        for (int i = 0; i < ass.Length; i++)
        {
            audios.Add(ass[i]);
        }

        base.Init(baseData, sceneAllZiyuan);
    }

    public override void playanim(bool isPlay)
    {
        if (isPlay)
        {
            zhuan.clip = zhuan["AC313A rotorLoop"].clip;
            zhuan.Play();
            audios.ForEach(a => a.volume = 0.3f);
            audios.ForEach(a => a.pitch = 1);
            audios.ForEach(a => a.Play());
            audios.ForEach(x => x.gameObject.SetActive(MyDataInfo.MyLevel == 3));
        }
        else
        {
            zhuan.clip = zhuan["AC313A rotorStop"].clip;
            zhuan.Play();
            zhuan["AC313A rotorStop"].normalizedTime = 1;
            audios.ForEach(x => x.gameObject.SetActive(false));
        }
    }
}