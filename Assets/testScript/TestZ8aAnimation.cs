using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;

public class TestZ8aAnimation : MonoBehaviour
{
    public Animation zhuan,zhuanWei;
    private List<AudioSource> myass;
    private List<WingMark> wingmarks;

    private void Start()
    {
        myass = new List<AudioSource>();
        if (myass.Count == 0)
        {
            var ass = transform.GetComponentsInChildren<AudioSource>(true);
            for (int i = 0; i < ass.Length; i++)
            {
                myass.Add(ass[i]);
            }
        }

        wingmarks = new List<WingMark>();
        var wms = transform.GetComponentsInChildren<WingMark>(true);
        for (int i = 0; i < wms.Length; i++)
        {
            wingmarks.Add(wms[i]);
        }
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     foreach (AnimationState state in zhuan)
        //     {
        //         Debug.Log(state.clip.name);
        //         if (string.Equals(state.clip.name, "rotorStart"))
        //         {
        //             zhuan.clip = state.clip;
        //             break;
        //         }
        //     }
        //
        //     zhuan.Play();
        // }

        if (Input.GetKeyDown(KeyCode.B))
        {
            // foreach (AnimationState state in zhuan)
            // {
            //     Debug.Log(state.clip.name);
            //     if (string.Equals(state.clip.name, "S76_xuanyi_loop2"))
            //     {
            //         zhuan.clip = state.clip;
            //         break;
            //     }
            // }
            // zhuan.clip = zhuan["S76_xuanyi_loop2"].clip;
            // zhuanWei.clip = zhuanWei["weiyi_loop"].clip;

            // myass.ForEach(a => a.volume = 0.3f);
            // myass.ForEach(a => a.pitch = 1);
            // myass.ForEach(a => a.Play());
            // myass.ForEach(x => x.gameObject.SetActive(true));
            // zhuan.Play();
            // zhuanWei.Play();
            for (int i = 0; i < myass.Count; i++)
            {
                Debug.Log(myass[i].name);
            }
            myass.ForEach(a => a.volume = 0.3f);
            myass.ForEach(a => a.Play());
            myass.ForEach(x => x.gameObject.SetActive(true));
            // wingmarks.ForEach(a => a.gameObject.SetActive(true));
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            foreach (AnimationState state in zhuan)
            {
                if (string.Equals(state.clip.name, "S76_xuanyi_close"))
                {
                    zhuan.clip = state.clip;
                    break;
                }
            }

            zhuan.Play();
            zhuan["S76_xuanyi_close"].normalizedTime = 1;
            foreach (AnimationState state in zhuanWei)
            {
                if (string.Equals(state.clip.name, "weiyi_close"))
                {
                    zhuanWei.clip = state.clip;
                    break;
                }
            }

            zhuanWei.Play();
            zhuanWei["weiyi_close"].normalizedTime = 1;
            // zhuan.Stop();
            // myass.ForEach(x => x.gameObject.SetActive(false));
        }
    }
}