using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioControler : MonoBehaviour
{
    public AudioSource fundo;
    public AudioSource efeitos;
    public List<AudioClip> listaMusicas = new List<AudioClip>();
    public List<Sprite> iconeSom = new List<Sprite>();
    public List<AudioClip> audioPowerUp = new List<AudioClip>();
    public AudioClip acertouSom;
    public AudioClip errouSom;
    public Slider sliderSom;
    private int next;
    private bool efeito;
    private AudioClip somEfeito;
    

    private void Start()
    {
        next = 0;
        sliderSom.value = 0.5f;
        fundo.volume = 0.5f;
        efeitos.volume = 0.5f;
    }

    private void Update()
    {
        if (!fundo.isPlaying)
        {
            if(listaMusicas.Count < next)
            {
                fundo.PlayOneShot(listaMusicas[next]);
                next = next < 4 ? next + 1 : 0;
            }
            else
            {
                next = 0;
            }
            
        }

        if (efeito)
        {
            if (!efeitos.isPlaying)
            {
                efeito = false;
                efeitos.PlayOneShot(somEfeito);
            }
        }
    }

    public void EfeitoSonoroSlider(AudioClip audio)
    {
        efeito = true;
        somEfeito = audio;
    }

    public void EfeitoSonoro(AudioClip audio)
    {
        efeitos.PlayOneShot(audio);
    }

    public void Acertou()
    {
        efeitos.PlayOneShot(acertouSom);
    }

    public void Errou()
    {
        efeitos.PlayOneShot(errouSom);
    }

    public void VolumeSom(float valor)
    {
        fundo.volume = valor;
        efeitos.volume = valor;
    }

    public void MuteSom(Image icone)
    {
        fundo.mute = !fundo.mute;
        efeitos.mute = !efeitos.mute;
        if (fundo.mute)
        {
            icone.sprite = iconeSom[0];
        }
        else
        {
            icone.sprite = iconeSom[1];
        }
    }
}
