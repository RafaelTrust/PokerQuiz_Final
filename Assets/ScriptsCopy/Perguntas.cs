using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Perguntas 
{
    public string pergunta;
    public string[] alternativa = new string[4];
    public int alternativaCorreta;
    public float timer;
}
