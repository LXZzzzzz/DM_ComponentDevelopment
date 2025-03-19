using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;

public class HelicopterController_S76 : HelicopterController
{
    private Dictionary<string,Animation> zhuanDic;
    private List<AudioSource> audios;
    public override void Init(EquipBase baseData, List<ZiYuanBase> sceneAllZiyuan)
    {

        zhuanDic = new Dictionary<string, Animation>();
        var anis = GetComponentsInChildren<Animation>(true);
        for (int i = 0; i < anis.Length; i++)
        {
            zhuanDic.Add(anis[i].name, anis[i]);
        }
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
            zhuanDic["MainWing"].clip = zhuanDic["MainWing"]["S76_xuanyi_loop2"].clip;
            zhuanDic["Tail"].clip = zhuanDic["Tail"]["weiyi_loop"].clip;
            zhuanDic["MainWing"].Play();
            zhuanDic["Tail"].Play();
            
            audios.ForEach(a => a.volume = 0.3f);
            audios.ForEach(x => x.gameObject.SetActive(MyDataInfo.MyLevel == 3));
        }
        else
        {
            zhuanDic["MainWing"].clip = zhuanDic["MainWing"]["S76_xuanyi_close"].clip;
            zhuanDic["Tail"].clip = zhuanDic["Tail"]["weiyi_close"].clip;
            zhuanDic["MainWing"].Play();
            zhuanDic["Tail"].Play();
            zhuanDic["MainWing"]["S76_xuanyi_close"].normalizedTime = 1;
            zhuanDic["Tail"]["weiyi_close"].normalizedTime = 1;

            audios.ForEach(x => x.gameObject.SetActive(false));
        }
    }
}
