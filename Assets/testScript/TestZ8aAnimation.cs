using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;

public class TestZ8aAnimation : MonoBehaviour
{
    public Animation zhuan;
    private List<AudioSource> myass;

    private void Start()
    {
        myass = new List<AudioSource>();
        if (myass.Count == 0)
        {
            var ass = transform.GetComponentsInChildren<AudioSource>();
            for (int i = 0; i < ass.Length; i++)
            {
                if (ass[i].enabled) myass.Add(ass[i]);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            foreach (AnimationState state in zhuan)
            {
                Debug.Log(state.clip.name);
                if (string.Equals(state.clip.name, "rotorStart"))
                {
                    zhuan.clip = state.clip;
                    break;
                }
            }

            zhuan.Play();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            foreach (AnimationState state in zhuan)
            {
                Debug.Log(state.clip.name);
                if (string.Equals(state.clip.name, "rotorLoop"))
                {
                    zhuan.clip = state.clip;
                    break;
                }
            }

            myass.ForEach(a => a.volume = 0.3f);
            myass.ForEach(a => a.pitch = 1);
            myass.ForEach(a => a.Play());
            myass.ForEach(x => x.gameObject.SetActive(true));
            zhuan.Play();
        }

        if (Input.GetKeyDown(KeyCode.D))
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
            // zhuan.Stop();
            myass.ForEach(x => x.gameObject.SetActive(false));
        }
    }
}