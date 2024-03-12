using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteUIAnimator : MonoBehaviour
{
    public Image m_Image;

    public Sprite[] m_SpriteArray;
    public float m_Speed = .02f;

    private int m_IndexSprite;
    Coroutine m_CorotineAnim;
    bool IsDone;
    public float temporizador;
    bool animador;

    void Start()
    {
        Func_PlayUIAnim();
        temporizador = 0;
        animador = false;
    }

    void Update()
    {
        if (animador)
        {
            temporizador -= Time.deltaTime;
            if(temporizador < 0)
            {
                Func_PlayUIAnim();
            }
        }
    }

    public void IniciaAnimacao()
    {
        animador = true;
    }

    public void Func_PlayUIAnim()
    {
        animador = false;
        IsDone = false;
        StartCoroutine(Func_PlayAnimUI());
        
    }

    public void Func_StopUIAnim()
    {
        IsDone = true;
        StopCoroutine(Func_PlayAnimUI());
    }
    IEnumerator Func_PlayAnimUI()
    {
        yield return new WaitForSeconds(m_Speed);
        if (m_IndexSprite >= m_SpriteArray.Length)
        {
            m_IndexSprite = 0;
        }
        m_Image.sprite = m_SpriteArray[m_IndexSprite];
        m_IndexSprite += 1;
        if (IsDone == false)
            m_CorotineAnim = StartCoroutine(Func_PlayAnimUI());
    }
}
