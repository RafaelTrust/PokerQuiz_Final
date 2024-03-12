using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JogoSolitario : MonoBehaviour
{
    public GameControler gamecontroller;

    //tela aposta
    public GameObject jogo;
    public GameObject apostaTela;
    public TextMeshProUGUI situacao;
    public Slider sliderAposta;
    public TMP_InputField telaAposta;

    //tela perguntas
    public GameObject temporizadorTela;
    public GameObject questoesTela;
    public TMP_InputField telaApostaFinal;
    public Slider sliderApostaFinal;
    public ListaPerguntas grupoPerguntas;

    //tela loja - power ups
    public GameObject powerUps;
    public TextMeshProUGUI limiteLojaTela;
    public GameObject loja;
    public List<float> lojaValores = new List<float>();
    public List<Sprite> powerUpImages = new List<Sprite>();

    //Erro
    public GameObject telaErro;

    //Fim de jogo
    public GameObject gameOver;

    private List<Perguntas> perguntasInMemory = new List<Perguntas>();
    private float dinheiroInicial;
    private float apostou;
    private int alternativa;
    private float timer;
    private bool passouPunch;
    private float timer2;
    private bool punch;
    private int[] indexPunch = new int[2];
    private bool _5050;
    private int[] index5050 = new int[2];
    private bool calculo;
    private int indexCalculo;
    private bool bloqueio;
    private bool bloqueioJogo;
    private int indexBloqueio;
    private bool temporizadorJogo;
    private float temporizador;
    private float temporizadorTotal;
    private int palpite;
    private float recordDinheiro;
    private int limiteCompra;
    private int gameNumeroPerguntas;

    void Start()
    {
        bloqueioJogo = false;
        temporizadorJogo = false;
        passouPunch = false;
        bloqueio = false;
        calculo = false;
        punch = false;
        _5050 = false;
        dinheiroInicial = 200;
        situacao.text = dinheiroInicial.ToString("N2");
        limiteCompra = 5;
        limiteLojaTela.text = limiteCompra.ToString();
        gameNumeroPerguntas = 20;
    }

    void Update()
    {
        if (punch)
        {
            if (passouPunch)
            {
                timer2 -= Time.deltaTime;
                if (timer2 < 0)
                {
                    passouPunch = false;
                    jogo.GetComponent<Animator>().SetInteger("punch", indexPunch[1]);
                }
            }
            else
            {
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    punch = false;
                    Image[] imagens = powerUps.GetComponentsInChildren<Image>();
                    imagens[indexPunch[0]].enabled = false;
                }
            }
        }
        if (_5050)
        {
            if (passouPunch)
            {
                timer2 -= Time.deltaTime;
                if (timer2 < 0)
                {
                    passouPunch = false;
                    Debug.Log("5050p = " + index5050[0]);
                    jogo.GetComponent<Animator>().SetInteger("5050p", index5050[0]);
                }
            }
            else
            {
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    _5050 = false;
                    powerUps.GetComponentsInChildren<Image>()[index5050[1]].enabled = false;
                }
            }
        }
        if (calculo)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                calculo = false;
                powerUps.GetComponentsInChildren<Image>()[indexCalculo].enabled = false;
            }
        }
        if (bloqueio)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                bloqueio = false;
                powerUps.GetComponentsInChildren<Image>()[indexBloqueio].enabled = false;
            }
        }
        if (temporizadorJogo)
        {
            temporizador -= Time.deltaTime;
            if (temporizador <= 9)
            {
                temporizadorTela.GetComponentsInChildren<TextMeshProUGUI>()[0].text = "0" + temporizador.ToString("N0");
            }
            else
            {
                temporizadorTela.GetComponentsInChildren<TextMeshProUGUI>()[0].text = temporizador.ToString("N0");
            }
            if (temporizador < 0)
            {
                temporizadorJogo = false;
                gamecontroller.audioSource.EfeitoSonoro(gamecontroller.audioSource.errouSom);
                Palpite(0);
            }
        }
    }

    public void RestartGame()
    {
        gamecontroller.FecharJogo = false;
        bloqueioJogo = false;
        temporizadorJogo = false;
        passouPunch = false;
        bloqueio = false;
        calculo = false;
        punch = false;
        _5050 = false;
        dinheiroInicial = 200;
        situacao.text = dinheiroInicial.ToString("N2");
        limiteCompra = 5;
        limiteLojaTela.text = limiteCompra.ToString();
        gameNumeroPerguntas = 20;
        sliderAposta.maxValue = dinheiroInicial;
        sliderApostaFinal.maxValue = dinheiroInicial;

        if (perguntasInMemory.Count > 0)
            perguntasInMemory.Clear();

        foreach (Perguntas pergunta in grupoPerguntas.listaPerguntas)
        {
            Debug.Log("pergunta: " + pergunta.pergunta);
            perguntasInMemory.Add(pergunta);
        }

        InicializacaoValores(false);
    }
    private void InicializacaoValores(bool respondendo)
    {
        Button[] botoesPower = powerUps.GetComponentsInChildren<Button>();
        Button[] botoesAlternativa = questoesTela.GetComponentsInChildren<Button>();
        telaAposta.text = "00";
        telaApostaFinal.text = "00";
        jogo.GetComponent<Animator>().SetInteger("punch", 0);
        jogo.GetComponent<Animator>().SetInteger("5050", 0);
        jogo.GetComponent<Animator>().SetInteger("5050p", 0);
        jogo.GetComponent<Animator>().SetInteger("calculo", 0);
        jogo.GetComponent<Animator>().SetInteger("bloqueio", 0);
        for (int i = 0; i < botoesAlternativa.Length; i++)
        {
            botoesAlternativa[i].interactable = true;
        }
        for (int i = 0; i < botoesPower.Length; i++)
        {
            botoesPower[i].interactable = respondendo;
        }
    }

    public void SairTelaErro()
    {
        telaErro.SetActive(false);
    }

    //Slider de apostas

    public void ValorAposta(float valor)
    {
        if (valor == 0)
        {
            telaAposta.text = "00";
        }
        else
        {
            telaAposta.text = valor.ToString("N2");
        }
    }

    public void ValorApostaFinal(float valor)
    {
        if (valor == 0)
        {
            telaApostaFinal.text = "00";
        }
        else
        {
            telaApostaFinal.text = valor.ToString("N2");
        }
    }

    public void InputValor(string valor)
    {
        sliderAposta.value = float.Parse(valor);
    }

    public void InputValorFinal(string valor)
    {
        sliderApostaFinal.value = float.Parse(valor);
    }

    //Apostas

    public void Apostando(TMP_InputField valor)
    {
        float aposta = float.Parse(valor.text);
        if (aposta > 0)
        {
            dinheiroInicial -= aposta;
            apostou = aposta;
            situacao.text = dinheiroInicial.ToString("N2");
            sliderAposta.maxValue = dinheiroInicial;
            sliderApostaFinal.maxValue = dinheiroInicial;
            TelaPerguntas();
        }
        else
        {
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[0].text = "Precisa de um Valor Maior que Zero";
        }
    }

    private void TelaPerguntas()
    {
        apostaTela.SetActive(false);
        questoesTela.SetActive(true);
        InicializacaoValores(true);

        int indexPergunta = Random.Range(0, (perguntasInMemory.Count - 1));

        alternativa = perguntasInMemory[indexPergunta].alternativaCorreta;
        questoesTela.GetComponentsInChildren<TextMeshProUGUI>()[0].text = perguntasInMemory[indexPergunta].pergunta;

        for (int i = 0; i < 4; i++)
        {
            questoesTela.GetComponentsInChildren<TextMeshProUGUI>()[i + 1].text = perguntasInMemory[indexPergunta].alternativa[i];
            temporizadorTela.GetComponent<Animator>().SetTrigger("temp");
            temporizador = perguntasInMemory[indexPergunta].timer;
            temporizadorTotal = perguntasInMemory[indexPergunta].timer;
            temporizadorJogo = true;
        }

        perguntasInMemory.Remove(perguntasInMemory[indexPergunta]);
    }

    public void ApostaFinal(TMP_InputField valor)
    {
        gameNumeroPerguntas--;
        apostaTela.SetActive(true);
        questoesTela.SetActive(false);
        dinheiroInicial -= float.Parse(valor.text);
        apostou += float.Parse(valor.text);
        InicializacaoValores(false);

        situacao.text = dinheiroInicial.ToString("N2");
        sliderAposta.maxValue = dinheiroInicial;
        sliderApostaFinal.maxValue = dinheiroInicial;
        apostaTela.GetComponentsInChildren<SpriteUIAnimator>()[0].Func_PlayUIAnim();
        if (palpite == alternativa)
        {
            gamecontroller.audioSource.Acertou();
            dinheiroInicial += 2 * apostou;
            situacao.text = dinheiroInicial.ToString("N2");
            sliderAposta.maxValue = dinheiroInicial;
            sliderApostaFinal.maxValue = dinheiroInicial;
        }
        else if (bloqueioJogo)
        {
            gamecontroller.audioSource.Errou();
            dinheiroInicial += apostou;
            situacao.text = dinheiroInicial.ToString("N2");
            sliderAposta.maxValue = dinheiroInicial;
            sliderApostaFinal.maxValue = dinheiroInicial;
        }
        bloqueioJogo = false;
        if (dinheiroInicial > recordDinheiro)
        {
            recordDinheiro = dinheiroInicial;
        }
        if (perguntasInMemory.Count > 0 && gameNumeroPerguntas > 0)
        {
            if (dinheiroInicial <= 0)
            {
                GameOver();
            }
        }
        else
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        gameOver.GetComponent<Animator>().SetTrigger("gameover");
        float recordeAnterior = PlayerPrefs.GetFloat("recorde");
        gameOver.GetComponentsInChildren<TextMeshProUGUI>()[0].text = recordeAnterior.ToString("N2");
        gameOver.GetComponentsInChildren<TextMeshProUGUI>()[1].text = recordDinheiro.ToString("N2");
        if (recordDinheiro > recordeAnterior)
        {
            gameOver.GetComponent<Animator>().SetBool("recorde", true);
            PlayerPrefs.SetFloat("recorde", recordDinheiro);
            PlayerPrefs.Save();
            gameOver.GetComponentsInChildren<TextMeshProUGUI>()[0].text = recordDinheiro.ToString("N2");
        }
        else
        {
            gameOver.GetComponentsInChildren<TextMeshProUGUI>()[0].text = recordeAnterior.ToString("N2");
            gameOver.GetComponent<Animator>().SetBool("recorde", false);
        }
    }

    //Alternativa escolhida
    public void Palpite(int acerto)
    {
        palpite = acerto;
        temporizadorJogo = false;
        temporizadorTela.GetComponent<Animator>().SetTrigger("temp");
    }

    //Loja de PowerUps
    public void Compra(int power)
    {
        Image[] listaImagem = powerUps.GetComponentsInChildren<Image>();
        int indexButton = -1;
        for (int i = 0; i < listaImagem.Length; i++)
        {
            if (!listaImagem[i].enabled)
            {
                indexButton = i;
                i = listaImagem.Length;
            }
        }
        if (limiteCompra > 0 && indexButton >= 0)
        {
            float valor = lojaValores[power];
            if (dinheiroInicial > (valor + 1))
            {
                limiteCompra--;
                situacao.text = limiteCompra.ToString();
                dinheiroInicial -= valor;
                float alterarValor = valor + valor * (0.2f);
                situacao.text = dinheiroInicial.ToString("N2");
                sliderAposta.maxValue = dinheiroInicial;
                sliderApostaFinal.maxValue = dinheiroInicial;
                loja.GetComponentsInChildren<TextMeshProUGUI>()[power + 1].text = alterarValor.ToString("N2");
                powerUps.GetComponentsInChildren<Button>()[indexButton].onClick.RemoveAllListeners();
                powerUps.GetComponentsInChildren<Button>()[indexButton].onClick.AddListener(() => UsarPowerUp(power, indexButton));
                powerUps.GetComponentsInChildren<Image>()[indexButton].enabled = true;
                powerUps.GetComponentsInChildren<Image>()[indexButton].sprite = powerUpImages[power];
            }
            else
            {
                telaErro.SetActive(true);
                telaErro.GetComponentsInChildren<TextMeshProUGUI>()[0].text = "Faltou Grana";
            }
        }
        else
        {
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[0].text = "Limite de Compra!";
        }
    }

    //Usando power ups

    public void UsarPowerUp(int index, int indexButton)
    {
        int aux = 0;
        int i = 0;
        gamecontroller.audioSource.EfeitoSonoro(gamecontroller.audioSource.audioPowerUp[index]);
        Button[] botoesPower = powerUps.GetComponentsInChildren<Button>();
        for (int j = 0; j < botoesPower.Length; j++)
        {
            botoesPower[j].interactable = false;
        }
        if (index == 0)
        {
            //punch 
            jogo.GetComponent<Animator>().SetInteger("punch", (5 + indexButton));
            while (aux == 0)
            {
                i = Random.Range(1, 4);
                if (i != alternativa)
                {
                    aux = 1;
                }
            }
            indexPunch[0] = indexButton;
            indexPunch[1] = i;
            timer2 = 0.1f;
            timer = 2;
            passouPunch = true;
            punch = true;
        }
        else if (index == 1)
        {
            //5050
            Debug.Log("indexButton = " + indexButton);
            jogo.GetComponent<Animator>().SetInteger("5050", (1 + indexButton));
            int y = 0;
            while (aux == 0)
            {
                i = Random.Range(1, 4);
                if (i != alternativa)
                {
                    aux = 1;
                }
            }
            aux = 0;
            while (aux == 0)
            {
                y = Random.Range(1, 4);
                if (y != i && y != alternativa)
                {
                    aux = 1;
                }
            }
            Debug.Log("i = " + i + " ; y = " + y);
            if ((i == 1 && y == 2) || (i == 2 && y == 1))
            {
                index5050[0] = 1;
            }
            else if ((i == 1 && y == 3) || (i == 3 && y == 1))
            {
                index5050[0] = 2;
            }
            else if ((i == 1 && y == 4) || (i == 4 && y == 1))
            {
                index5050[0] = 3;
            }
            else if ((i == 2 && y == 3) || (i == 3 && y == 2))
            {
                Debug.Log("5050p = " + 4);
                index5050[0] = 4;
            }
            else if ((i == 2 && y == 4) || (i == 4 && y == 2))
            {
                index5050[0] = 5;
            }
            else if ((i == 3 && y == 4) || (i == 4 && y == 3))
            {
                index5050[0] = 6;
            }
            timer = 1.2f;
            timer2 = 1.1f;
            passouPunch = true;
            index5050[1] = indexButton;
            _5050 = true;
        }
        else if (index == 2)
        {
            //calcular
            jogo.GetComponent<Animator>().SetInteger("calculo", (1 + indexButton));
            temporizador += temporizadorTotal * 0.5f;
            timer = 2;
            indexCalculo = indexButton;
            calculo = true;
        }
        else if (index == 3)
        {
            //analise
            jogo.GetComponent<Animator>().SetInteger("calculo", (1 + indexButton));
            temporizador += temporizadorTotal;
            timer = 2;
            indexCalculo = indexButton;
            calculo = true;
        }
        else if (index == 4)
        {
            //bloquear
            jogo.GetComponent<Animator>().SetInteger("bloqueio", (1 + indexButton));
            bloqueioJogo = true;
            timer = 2;
            indexBloqueio = indexButton;
            bloqueio = true;
        }
    }
}
