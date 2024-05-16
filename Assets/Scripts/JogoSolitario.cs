using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class JogoSolitario : MonoBehaviour
{
    public GameControler gamecontroller;
    public Animator carregamento;

    //Menu Mesas
    public GameObject[] listaMesasTela;
    private List<List<Mesa>> gruposPaginacaoMesa;
    public GameObject paginacaoAnteriorMesa;
    public GameObject paginacaoProximoMesa;
    public GameObject telaNick;
    public GameObject telaErroPrincipal;
    public Toggle offlineToggle;
    private SalaPerguntas salaPerguntas;
    private int paginaMesa;
    private bool offline;

    //Inicio Jogo
    public Animator mainCamera;
    public Animator fichasAnimator;
    public Animator fundoAnimator;
    public Animator solitarioAnimator;
    public SpriteUIAnimator handle;
    public SpriteUIAnimator image;
    public AudioControler audioSolitario;
    public AudioClip iniciar;
    public Animator telaAnimator;
    public Animator apostaFinal;

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
    public Animator acerto;

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
    public GameObject gameOverOffline;

    //Recorde
    public GameObject[] listaRecorde;
    public GameObject filtroTela;
    public List<TMP_Dropdown> filtrosData;
    public GameObject paginacaoAnteriorRecorde;
    public GameObject paginacaoProximoRecorde;
    public Toggle logadoToggle;
    public Toggle tudoDataToggle;
    public Toggle nomesRepetidosToggle;
    public TMP_Dropdown nickFiltroDropdown;
    public ListaRecorde recordes;
    public ListaRecorde recordesCompleto;
    public TextMeshProUGUI recordeTextoOff;
    private string codSala;
    private RecordeLimitado recorde;
    private Usuario player;
    private string nick;
    private int acertos;
    private List<List<Recorde>> gruposPaginacaoRecorde;
    private int paginaRecorde;
    private bool logadoFiltro;
    private DateTime dataAnterior;
    private DateTime dataPosterior;
    private bool tudoData;
    private bool nomesRepetidos;
    private string nickFiltro;

    private List<Perguntas> perguntasInMemory = new List<Perguntas>();
    private Perguntas perguntaAtual;
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
    private ListaMesas conjuntoMesas;
    private ListaMesas auxPesqMesas;
    private string codBusca;
    private List<Estatistica> listaEstatisticaJogo = new List<Estatistica>();
    private int indexPerguntaAtual;
    private bool timeOut;

    void Start()
    {
        recorde = new RecordeLimitado();
        dataAnterior = DateTime.Now;
        dataPosterior = new DateTime(dataAnterior.Year + 1, dataAnterior.Month, dataAnterior.Day);
        filtrosData[0].value = dataAnterior.Day - 1;
        filtrosData[1].value = dataAnterior.Month - 1;
        filtrosData[2].value = dataAnterior.Year - 2022;
        filtrosData[3].value = dataPosterior.Day - 1;
        filtrosData[4].value = dataPosterior.Month - 1;
        filtrosData[5].value = dataPosterior.Year - 2022;
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
            if (temporizador <= 0)
            {
                temporizadorJogo = false;
                gamecontroller.audioSource.EfeitoSonoro(gamecontroller.audioSource.errouSom);
                telaAposta.text = "0";
                timeOut = true;
                Palpite(0);
                ApostaFinal(telaAposta);
            }
        }
    }

    public void OfflineToggle(bool valor)
    {
        offline = valor;
    }

    public void NickEscolhidoDropdown(int index)
    {
        if(nickFiltroDropdown.options[index].text != "Todos")
        {
            nickFiltro = nickFiltroDropdown.options[index].text;
        }
        else
        {
            nickFiltro = string.Empty;
        }
       
    }

    public void TudoDataToggle(bool tudo)
    {
        tudoData = tudo;
        foreach(TMP_Dropdown dropdown in filtrosData)
        {
            dropdown.interactable = !tudo;
        }
    }

    public void NomesRepetidosDataToggle(bool nick)
    {
        nomesRepetidos = nick; 
    }

    public void DropdownBeforeDaySelected(int index)
    {
       dataAnterior = dataAnterior.AddDays(int.Parse(filtrosData[0].options[index].text) - dataAnterior.Day);
    }

    public void DropdownBeforeMonthSelected(int index)
    {
        dataAnterior = dataAnterior.AddMonths(int.Parse(filtrosData[1].options[index].text) - dataAnterior.Month);
    }

    public void DropdownBeforeYearSelected(int index)
    {
        dataAnterior = new DateTime(int.Parse(filtrosData[2].options[index].text), dataAnterior.Month, dataAnterior.Day);
    }

    public void DropdownAfterDaySelected(int index)
    {
        dataPosterior = dataAnterior.AddDays(int.Parse(filtrosData[3].options[index].text) - dataAnterior.Day);
    }

    public void DropdownAfterMonthSelected(int index)
    {
        dataPosterior = dataAnterior.AddMonths(int.Parse(filtrosData[4].options[index].text) - dataAnterior.Month);
    }

    public void DropdownAfterYearSelected(int index)
    {
        dataPosterior = new DateTime(int.Parse(filtrosData[5].options[index].text), dataAnterior.Month, dataAnterior.Day);
    }

    public void MostrarLogadosFiltro(bool logado)
    {
        logadoFiltro = logado;
    }

    public void BuscarMinhaMesaSolitariaSorteio()
    {
        Mesa mesaSorteada = conjuntoMesas.mesas[UnityEngine.Random.Range(0, conjuntoMesas.mesas.Length - 1)];
        if (offline)
        {
            JogoOffline(mesaSorteada.codSala);
        }
        else
        {
            StartCoroutine(GetMesaPerguntasRequest(mesaSorteada.codSala));
        }
    }

    public void BuscarMinhaMesaSolitariaButton()
    {
        if (offline)
        {
            JogoOffline(codBusca);
        }
        else
        {
            StartCoroutine(GetMesaPerguntasRequest(codBusca));
        }
    }

    public void BuscarMinhaMesaSolitaria(TextMeshProUGUI cod)
    {
        if (offline)
        {
            JogoOffline(cod.text);
        }
        else
        {
            StartCoroutine(GetMesaPerguntasRequest(cod.text));
        }
    }

    private IEnumerator GetMesaPerguntasRequest(string cod)
    {
        carregamento.SetTrigger("carregar");
        var getRequest = gamecontroller.CreateRequest(
            gamecontroller.UrlRota + "/salas/solitario/" + cod,
            false,
            GameControler.RequestType.GET,
            null
            );
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            carregamento.SetTrigger("carregar");
            telaErroPrincipal.SetActive(true);
            telaErroPrincipal.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Mesa não encontrada. \nVerifique sua conexão";
            offline = true;
            Debug.LogError(getRequest.error);
            ListaJogoOffline auxSalaPerguntas = JsonUtility.FromJson<ListaJogoOffline>(PlayerPrefs.GetString("salaPerguntasOffiline"));
            salaPerguntas = new SalaPerguntas();
            telaAnimator.SetTrigger("Solitario");
            if (auxSalaPerguntas != null)
            {
                offlineToggle.isOn = true;
                foreach (SalaPerguntas salaPergunta in auxSalaPerguntas.listaOffline)
                {
                    if (salaPergunta.sala.codSala == cod)
                    {
                        salaPerguntas = salaPergunta;
                    }
                }
                if(salaPerguntas.sala != null)
                {
                    OrganizandoPerguntas();
                    StartGame();
                }
                else
                {
                    telaErroPrincipal.SetActive(true);
                    telaErroPrincipal.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Mesa não encontrada. Mesa não baixada";
                }
            }
        }
        else
        {
            carregamento.SetTrigger("carregar");
            offlineToggle.isOn = false;
            offline = false;
            telaAnimator.SetTrigger("Solitario");
            codSala = cod;
            GravandoSalaPerguntasOffline(getRequest.downloadHandler.text);
            SalaPerguntas auxSalaPerguntas = JsonUtility.FromJson<SalaPerguntas>(getRequest.downloadHandler.text);
            salaPerguntas = new SalaPerguntas();
            salaPerguntas.sala = auxSalaPerguntas.sala;
            List<PerguntaOnline> listaAuxPergunta = new List<PerguntaOnline>();
            for(int i = 0; i < auxSalaPerguntas.listaPerguntas.Length; i++)
            {
                listaAuxPergunta.Add(auxSalaPerguntas.listaPerguntas[i]);
            }
            salaPerguntas.listaPerguntas = listaAuxPergunta.ToArray();
            OrganizandoPerguntas();
            StartGame();
        }
    }

    private void JogoOffline(string cod)
    {
        offline = true;
        ListaJogoOffline auxSalaPerguntas = JsonUtility.FromJson<ListaJogoOffline>(PlayerPrefs.GetString("salaPerguntasOffiline"));
        salaPerguntas = new SalaPerguntas();
        telaAnimator.SetTrigger("Solitario");
        if (auxSalaPerguntas != null)
        {
            offlineToggle.isOn = true;
            foreach (SalaPerguntas salaPergunta in auxSalaPerguntas.listaOffline)
            {
                if (salaPergunta.sala.codSala == cod)
                {
                    salaPerguntas = salaPergunta;
                }
            }
            if (salaPerguntas.sala != null)
            {
                OrganizandoPerguntas();
                StartGame();
            }
            else
            {
                telaErroPrincipal.SetActive(true);
                telaErroPrincipal.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Mesa não encontrada. Mesa não baixada";
            }
        }
    }

    public void GravandoSalaPerguntasOffline(string json)
    {
        ListaJogoOffline oldLista = JsonUtility.FromJson<ListaJogoOffline>(PlayerPrefs.GetString("salaPerguntasOffiline"));
        SalaPerguntas novaSala = JsonUtility.FromJson<SalaPerguntas>(json);
        List<SalaPerguntas> novaLista = new List<SalaPerguntas>();
        if(oldLista != null && oldLista.listaOffline.Length > 0)
        {
            bool adicionar = true;
            for(int i = 0; i < oldLista.listaOffline.Length; i++)
            {
                if(novaSala.sala.codSala == oldLista.listaOffline[i].sala.codSala)
                {
                    adicionar = false;
                }
                novaLista.Add(oldLista.listaOffline[i]);
            }
            if (adicionar)
            {
                novaLista.Add(novaSala);
            }
        }
        else
        {
            novaLista.Add(novaSala);
        }
        ListaJogoOffline listaFinal = new ListaJogoOffline();
        listaFinal.listaOffline = novaLista.ToArray();
        PlayerPrefs.SetString("salaPerguntasOffiline", JsonUtility.ToJson(listaFinal));
    }

    public void ListarMinhasMesasSolitario()
    {
        StartCoroutine(GetListaMesasSolitarioRequest());
    }
    private IEnumerator GetListaMesasSolitarioRequest()
    {
        carregamento.SetTrigger("carregar");
        var getRequest = gamecontroller.CreateRequest(
            gamecontroller.UrlRota + "/salas/publicas",
            false,
            GameControler.RequestType.GET,
            null
            );
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            carregamento.SetTrigger("carregar");
            telaErroPrincipal.SetActive(true);
            telaErroPrincipal.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Nenhuma mesa encontrada online. \nVerifique sua conexão.";
            Debug.LogError(getRequest.error);
            offline = true;
            ListaJogoOffline auxListaMesas = JsonUtility.FromJson<ListaJogoOffline>(PlayerPrefs.GetString("salaPerguntasOffiline"));
            if(auxListaMesas != null)
            {
                offlineToggle.isOn = true;
                telaAnimator.SetTrigger("Solitario");
                conjuntoMesas = new ListaMesas();
                auxPesqMesas = new ListaMesas();
                List<Mesa> auxLista = new List<Mesa>();
                for (int i = 0; i < auxListaMesas.listaOffline.Length; i++)
                {
                    auxLista.Add(auxListaMesas.listaOffline[i].sala);
                }
                conjuntoMesas.mesas = auxLista.ToArray();
                auxPesqMesas.mesas = auxLista.ToArray();
                OrganizandoMesas();
            }       
        }
        else
        {
            carregamento.SetTrigger("carregar");
            offlineToggle.isOn = false;
            offline = false;
            telaAnimator.SetTrigger("Solitario");
            conjuntoMesas = new ListaMesas();
            auxPesqMesas = new ListaMesas();
            conjuntoMesas = JsonUtility.FromJson<ListaMesas>(getRequest.downloadHandler.text);
            auxPesqMesas = JsonUtility.FromJson<ListaMesas>(getRequest.downloadHandler.text);
            ListaMesas auxListaMesas = JsonUtility.FromJson<ListaMesas>(getRequest.downloadHandler.text);
            List<Mesa> listaAux = new List<Mesa>();
            ListaJogoOffline auxListaMesasOff = JsonUtility.FromJson<ListaJogoOffline>(PlayerPrefs.GetString("salaPerguntasOffiline"));
            foreach (Mesa mesa in auxListaMesas.mesas)
            {
                listaAux.Add(mesa);
            }
            if(auxListaMesasOff != null)
            {
                foreach (SalaPerguntas salaMesa in auxListaMesasOff.listaOffline)
                {
                    bool grava = true;
                    foreach (Mesa mesa in listaAux)
                    {
                        if (salaMesa.sala.codSala == mesa.codSala)
                        {
                            grava = false;
                        }
                    }
                    if (grava)
                    {
                        listaAux.Add(salaMesa.sala);
                    }
                }
            }
            conjuntoMesas.mesas = listaAux.ToArray();
            auxPesqMesas.mesas = listaAux.ToArray();
            OrganizandoMesas();
        }
    }
    
    public void StartGame()
    {
        timeOut = false;
        acertos = 0;
        listaEstatisticaJogo = new List<Estatistica>();
        nickFiltroDropdown.options = new List<TMP_Dropdown.OptionData>();
        TMP_Dropdown.OptionData dado = new TMP_Dropdown.OptionData();
        dado.text = "Todos";
        nickFiltroDropdown.options.Add(dado);
        nomesRepetidosToggle.isOn = true;
        nomesRepetidos = true;
        logadoToggle.isOn = false;
        logadoFiltro = false;
        tudoDataToggle.isOn = true;
        tudoData = true;
        foreach (TMP_Dropdown dropdown in filtrosData)
        {
            dropdown.interactable = false;
        }
        nickFiltro = string.Empty;
        mainCamera.SetTrigger("Entrar");
        fichasAnimator.SetTrigger("Sair");
        fundoAnimator.SetTrigger("Entrando");
        solitarioAnimator.SetTrigger("Entrando");
        solitarioAnimator.SetTrigger("Menu");
        telaAnimator.SetTrigger("Sair");
        handle.IniciaAnimacao();
        image.IniciaAnimacao();
        audioSolitario.EfeitoSonoro(iniciar);
        gamecontroller.IniciarJogoSolitario();
    }

    public void RestartGame()
    {
        gamecontroller.FecharJogo = false;
        timeOut = false;
        recordDinheiro = 0;
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

        int indexPergunta = UnityEngine.Random.Range(0, (perguntasInMemory.Count - 1));

        indexPerguntaAtual = 0;
        perguntaAtual = new Perguntas();
        for(int i = 0; i < salaPerguntas.listaPerguntas.Length; i++)
        {
            if(perguntasInMemory[indexPergunta].pergunta == salaPerguntas.listaPerguntas[i].enun)
            {
                indexPerguntaAtual = i;
                perguntaAtual.pergunta = perguntasInMemory[indexPergunta].pergunta;
                perguntaAtual.alternativa[0] = perguntasInMemory[indexPergunta].alternativa[0];
                perguntaAtual.alternativa[1] = perguntasInMemory[indexPergunta].alternativa[1];
                perguntaAtual.alternativa[2] = perguntasInMemory[indexPergunta].alternativa[2];
                perguntaAtual.alternativa[3] = perguntasInMemory[indexPergunta].alternativa[3];
                perguntaAtual.alternativaCorreta = perguntasInMemory[indexPergunta].alternativaCorreta;
            }
        }
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
            acerto.SetTrigger("acerto");
            gamecontroller.audioSource.Acertou();
            acertos++;
            dinheiroInicial += 2 * apostou;
            situacao.text = dinheiroInicial.ToString("N2");
            sliderAposta.maxValue = dinheiroInicial;
            sliderApostaFinal.maxValue = dinheiroInicial;
        }
        else if (bloqueioJogo)
        {
            acerto.SetTrigger("erro");
            gamecontroller.audioSource.Errou();
            dinheiroInicial += apostou;
            situacao.text = dinheiroInicial.ToString("N2");
            sliderAposta.maxValue = dinheiroInicial;
            sliderApostaFinal.maxValue = dinheiroInicial;
        }
        else
        {
            acerto.SetTrigger("erro");
            gamecontroller.audioSource.Errou();
        }

        string pergunta_fk = salaPerguntas.listaPerguntas[indexPerguntaAtual]._id;
        int alternativaAux = 0;
        int correto = salaPerguntas.listaPerguntas[indexPerguntaAtual].correto;
        int[] auxAlternativasErradas = { 0, 0, 0 };

        if (!timeOut)
        {
            if (salaPerguntas.listaPerguntas[indexPerguntaAtual].alternativa1 == perguntaAtual.alternativa[palpite - 1])
            {
                alternativaAux = 1;
            }
            else if (salaPerguntas.listaPerguntas[indexPerguntaAtual].alternativa2 == perguntaAtual.alternativa[palpite - 1])
            {
                alternativaAux = 2;
            }
            else if (salaPerguntas.listaPerguntas[indexPerguntaAtual].alternativa3 == perguntaAtual.alternativa[palpite - 1])
            {
                alternativaAux = 3;
            }
            else if (salaPerguntas.listaPerguntas[indexPerguntaAtual].alternativa4 == perguntaAtual.alternativa[palpite - 1])
            {
                alternativaAux = 4;
            }
        }
        else
        {
            int j = 0;
            for(int i = 0; i < 4; i++)
            {
                if(i != correto)
                {
                    auxAlternativasErradas[j] = i;
                    j++;
                }
            }
            int erro = UnityEngine.Random.Range(0, (2));
            if (salaPerguntas.listaPerguntas[indexPerguntaAtual].alternativa1 == perguntaAtual.alternativa[auxAlternativasErradas[erro]])
            {
                alternativaAux = 1;
            }
            else if (salaPerguntas.listaPerguntas[indexPerguntaAtual].alternativa2 == perguntaAtual.alternativa[auxAlternativasErradas[erro]])
            {
                alternativaAux = 2;
            }
            else if (salaPerguntas.listaPerguntas[indexPerguntaAtual].alternativa3 == perguntaAtual.alternativa[auxAlternativasErradas[erro]])
            {
                alternativaAux = 3;
            }
            else if (salaPerguntas.listaPerguntas[indexPerguntaAtual].alternativa4 == perguntaAtual.alternativa[auxAlternativasErradas[erro]])
            {
                alternativaAux = 4;
            }
        }
        timeOut = false;
        Estatistica formEstatistica = new Estatistica();
        formEstatistica.sala_cod = codSala;
        formEstatistica.pergunta_fk = pergunta_fk;
        formEstatistica.alternativa = alternativaAux;
        formEstatistica.correto = correto;
        listaEstatisticaJogo.Add(formEstatistica);
        bloqueioJogo = false;
        if (dinheiroInicial > recordDinheiro)
        {
            recordDinheiro = dinheiroInicial;
        }
        if (perguntasInMemory.Count > 0 && gameNumeroPerguntas > 0)
        {
            if (dinheiroInicial <= 0)
            {
                if (!offline)
                {
                    if (!string.IsNullOrEmpty(PlayerPrefs.GetString("nick")))
                    {
                        StartCoroutine(VerificandoConta(PlayerPrefs.GetString("nick")));
                    }
                    else
                    {
                        //GameOver(false, true);
                        telaNick.GetComponent<Animator>().SetTrigger("nick");
                    }
                }
                else
                {
                    GameOver(false, false);
                }
            }
        }
        else
        {
            if (!offline)
            {
                if (!string.IsNullOrEmpty(PlayerPrefs.GetString("nick")))
                {
                    StartCoroutine(VerificandoConta(PlayerPrefs.GetString("nick")));
                }
                else
                {
                    GameOver(false, true);
                }
            }
            else
            {
                GameOver(false, false);
            }
        }
    }

    private IEnumerator VerificandoConta(string nick)
    {
        carregamento.SetTrigger("carregar");
        var getRequest = gamecontroller.CreateRequest(
            gamecontroller.UrlRota + "/usuarios/" + nick,
            true,
            GameControler.RequestType.GET,
            null
            );
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            carregamento.SetTrigger("carregar");
            telaNick.GetComponent<Animator>().SetTrigger("nick");
        }
        else
        {
            carregamento.SetTrigger("carregar");
            player = JsonUtility.FromJson<Usuario>(getRequest.downloadHandler.text);
            GameOver(true, true);
        }
    }

    public void InputNick(string nick)
    {
        this.nick = nick;
    }

    public void SairTelaNick()
    {
        telaNick.GetComponent<Animator>().SetTrigger("nick");
        GameOver(false, false);
    }

    public void ConfirmarTelaNick()
    {
        if (string.IsNullOrEmpty(nick.Trim()))
        {
            telaErroPrincipal.SetActive(true);
            telaErroPrincipal.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Nick não preenchido.";
        }
        else
        {
            telaNick.GetComponent<Animator>().SetTrigger("nick");
            GameOver(false, true);
        }
    }

    public IEnumerator ListaRecordeSala(string cod, bool menu)
    {
        carregamento.SetTrigger("carregar");
        var getRequest = gamecontroller.CreateRequest(
            gamecontroller.UrlRota + "/recorde/sala/" + cod,
            false,
            GameControler.RequestType.GET,
            null
            );
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            carregamento.SetTrigger("carregar");
            telaErroPrincipal.SetActive(true);
            fichasAnimator.SetTrigger("Sair");
            telaAnimator.SetTrigger("Sair");
            telaErroPrincipal.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Recordes não disponíveis. \nVerifique sua conexão";
            Debug.LogError(getRequest.error);
            GameOver(false, false);
        }
        else
        {
            carregamento.SetTrigger("carregar");
            ListaRecorde auxListaRecordes = JsonUtility.FromJson<ListaRecorde>(getRequest.downloadHandler.text);
            recordes.listaRecorde = new Recorde[auxListaRecordes.listaRecorde.Length];
            recordesCompleto.listaRecorde = new Recorde[auxListaRecordes.listaRecorde.Length];
            limpaRecordesTela();
            if (auxListaRecordes.listaRecorde.Length > 0)
            {
                List<Recorde> listaRecorde = new List<Recorde>();
                bool adicionar = true;
                for (int i = 0; i < auxListaRecordes.listaRecorde.Length; i++)
                {
                    listaRecorde.Add(auxListaRecordes.listaRecorde[i]);

                    for(int j = 0; j < nickFiltroDropdown.options.Count; j++)
                    {
                        if (auxListaRecordes.listaRecorde[i].nick == nickFiltroDropdown.options[j].text)
                        {
                            adicionar = false;
                        }
                    }

                    if (adicionar)
                    {
                        TMP_Dropdown.OptionData nickOption = new TMP_Dropdown.OptionData();

                        nickOption.text = auxListaRecordes.listaRecorde[i].nick;
                        nickFiltroDropdown.options.Add(nickOption);
                    }
                }
                listaRecorde.Sort((player1, player2) => player2.valor.CompareTo(player1.valor));
                for (int i = 0; i < listaRecorde.Count; i++)
                {
                    recordes.listaRecorde[i] = listaRecorde[i];
                    recordesCompleto.listaRecorde[i] = listaRecorde[i];
                }

                OrganizandoRecordesTela();
                if (menu)
                {
                    gameOver.GetComponent<Animator>().SetBool("menu", true);
                    gameOver.GetComponent<Animator>().SetTrigger("recorde");
                }
            }
            else
            {
                telaErroPrincipal.SetActive(true);
                telaErroPrincipal.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Recordes não disponíveis.";
            }
        }
    }

    private void limpaRecordesTela()
    {
        foreach (GameObject recorde in listaRecorde)
        {
            recorde.SetActive(true);
        }
        paginacaoAnteriorRecorde.SetActive(false);
        paginacaoProximoRecorde.SetActive(false);
        for (int i = 0; i < 11; i++)
        {
            listaRecorde[i].GetComponentsInChildren<TextMeshProUGUI>()[0].text = string.Empty;
            listaRecorde[i].GetComponentsInChildren<TextMeshProUGUI>()[1].text = string.Empty;
            listaRecorde[i].GetComponentsInChildren<TextMeshProUGUI>()[2].text = string.Empty;
        }
    }

    private IEnumerator GravarRecordeSala(string cod)
    {
        if (!recorde.logado)
        {
            carregamento.SetTrigger("carregar");
        }
        var getRequest = gamecontroller.CreateRequest(
            gamecontroller.UrlRota + "/recorde/criar",
            false,
            GameControler.RequestType.POST,
            JsonUtility.ToJson(recorde)
            );
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            carregamento.SetTrigger("carregar");
            fichasAnimator.SetTrigger("Sair");
            telaAnimator.SetTrigger("Sair");
            Debug.LogError(getRequest.error);
            telaErroPrincipal.SetActive(true);
            telaErroPrincipal.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Recordes não disponíveis. \nVerifique sua conexão";
            GameOver(false, false);
        }
        else
        {
            StartCoroutine(ListaRecordeSala(cod, false));
        }
    }

    public void GameOver(bool logado, bool gravar)
    {
        if (gravar)
        {
            if (logado)
            {
                recorde.player_fk = player._id;
                recorde.nick = player.nick;
                recorde.logado = true;
            }
            else
            {
                recorde.nick = nick;
                recorde.logado = false;
            }
            recorde.sala_cod = codSala;
            recorde.valor = recordDinheiro;
            recorde.pcent_acertos = (float)acertos / salaPerguntas.sala.limitPerguntas * 100f;
            ListaEstatisticaDTO dto = new ListaEstatisticaDTO();
            dto.listaEstatistica = listaEstatisticaJogo.ToArray();
            StartCoroutine(CreateEstatisticaRequest(dto));
            OrganizandoRecordesTela();
            gameOver.GetComponent<Animator>().SetBool("menu", false);
            gameOver.GetComponent<Animator>().SetTrigger("recorde");
        }
        else
        {
            gameOverOffline.GetComponent<Animator>().SetTrigger("offline");
            recordeTextoOff.text = recordDinheiro.ToString("F2");
        }
        gamecontroller.FecharJogo = true;
    }

    public void AbrirFecharFiltro()
    {
        filtroTela.GetComponent<Animator>().SetTrigger("filtro");
    }

    public void ConfirmarFiltro()
    {
        List<RecordeData> listaRecordeAux = new List<RecordeData>();
        List<RecordeData> listaFiltrada = new List<RecordeData>();
        List<Recorde> listaFiltradaFinal = new List<Recorde>();
        Dictionary<string, float> maiorPontuacaoPorJogador = new Dictionary<string, float>();
        foreach (Recorde recorde in recordesCompleto.listaRecorde)
        {
            if(maiorPontuacaoPorJogador.ContainsKey(recorde.nick))
            {
                if (recorde.valor > maiorPontuacaoPorJogador[recorde.nick])
                {
                    maiorPontuacaoPorJogador[recorde.nick] = recorde.valor;
                }
            }
            else
            {
                maiorPontuacaoPorJogador.Add(recorde.nick, recorde.valor);
            }
            string dataCorrigida = "";
            if (recorde.data[6] == '-')
            {
                dataCorrigida = recorde.data.Insert(5, "0");
            }
            else
            {
                dataCorrigida = recorde.data;
            }

            DateTime data;
            if (DateTime.TryParseExact(dataCorrigida, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out data))
            {
                Debug.Log("Data Convertida: " + data.ToString("dd/MM/yyyy"));
            }
            else
            {
                Debug.Log("Falha ao converter data.");
            }
            RecordeData recordeAux = new RecordeData();
            recordeAux._id = recorde._id;
            recordeAux.sala_fk = recorde.sala_fk;
            recordeAux.player_fk = recorde.player_fk;
            recordeAux.nick = recorde.nick;
            recordeAux.logado = recorde.logado;
            recordeAux.valor = recorde.valor;
            recordeAux.pcent_acertos = recorde.pcent_acertos;
            recordeAux.data = data;
            recordeAux.__v = recorde.__v;

            listaRecordeAux.Add(recordeAux);
        }

        listaFiltrada = listaRecordeAux.Where(objeto =>
        {
            bool logado = ((logadoFiltro && objeto.logado) || (!logadoFiltro && objeto.logado) || (!logadoFiltro && !objeto.logado));
            bool dataFiltro = (objeto.data >= dataAnterior) && (objeto.data <= dataPosterior);
            bool data = (tudoData || dataFiltro);
            bool nickRepetido = true;
            bool igualNick = true;

            if (nickFiltro != string.Empty)
            {
                igualNick = objeto.nick == nickFiltro;
            }
             
            if (!nomesRepetidos)
            {
                nickRepetido = maiorPontuacaoPorJogador[objeto.nick] == objeto.valor;
            }

            return logado && data && igualNick && nickRepetido;
        }).ToList();

        recordes = new ListaRecorde();
        foreach(RecordeData recordeData in listaFiltrada)
        {
            Recorde recorde = new Recorde();
            recorde._id = recordeData._id;
            recorde.sala_fk = recordeData.sala_fk;
            recorde.player_fk = recordeData.player_fk;
            recorde.nick = recordeData.nick;
            recorde.logado = recordeData.logado;
            recorde.valor = recordeData.valor;
            recorde.pcent_acertos = recordeData.pcent_acertos;
            recorde.data = recordeData.data.ToString("yyyy-MM-dd");
            recorde.__v = recordeData.__v;
            listaFiltradaFinal.Add(recorde);
        }
        recordes.listaRecorde = listaFiltradaFinal.ToArray();
        filtroTela.GetComponent<Animator>().SetTrigger("filtro");
        OrganizandoRecordesTela();
    }

    //Alternativa escolhida
    public void Palpite(int acerto)
    {
        palpite = acerto;
        temporizadorJogo = false;
        temporizadorTela.GetComponent<Animator>().SetTrigger("temp");
        if(dinheiroInicial <= 0)
        {
            telaAposta.text = "0";
            ApostaFinal(telaAposta);
        }
        else
        {
            apostaFinal.SetTrigger("aposta");
        }
    }

    private IEnumerator CreateEstatisticaRequest(ListaEstatisticaDTO form)
    {
        carregamento.SetTrigger("carregar");
        var getRequest = gamecontroller.CreateRequest(
            gamecontroller.UrlRota + "/recorde/criarEstatiscas",
            false,
            GameControler.RequestType.POST,
            JsonUtility.ToJson(form)
            );
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            carregamento.SetTrigger("carregar");
            telaErroPrincipal.SetActive(true);
            telaErroPrincipal.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Não gravou a estatistica. Verifique sua conexão";
            Debug.LogError(getRequest.error);
        }
        else
        {
            StartCoroutine(GravarRecordeSala(codSala));
        }
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
                i = UnityEngine.Random.Range(1, 4);
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
            jogo.GetComponent<Animator>().SetInteger("5050", (1 + indexButton));
            int y = 0;
            while (aux == 0)
            {
                i = UnityEngine.Random.Range(1, 4);
                if (i != alternativa)
                {
                    aux = 1;
                }
            }
            aux = 0;
            while (aux == 0)
            {
                y = UnityEngine.Random.Range(1, 4);
                if (y != i && y != alternativa)
                {
                    aux = 1;
                }
            }
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

    private void OrganizandoPerguntas()
    {
        List<PerguntaOnline> auxListPerguntas = new List<PerguntaOnline>();
        foreach (PerguntaOnline pergunta in salaPerguntas.listaPerguntas)
        {
            auxListPerguntas.Add(pergunta);
        }
        grupoPerguntas.listaPerguntas.Clear();
        for (int i = 0; i < salaPerguntas.sala.limitPerguntas; i++)
        {
            int index = UnityEngine.Random.Range(0, (auxListPerguntas.Count - 1));
            Perguntas perguntaSolitario = new Perguntas();
            perguntaSolitario.pergunta = auxListPerguntas[index].enun;
            perguntaSolitario.alternativa[0] = auxListPerguntas[index].alternativa1;
            perguntaSolitario.alternativa[1] = auxListPerguntas[index].alternativa2;
            perguntaSolitario.alternativa[2] = auxListPerguntas[index].alternativa3;
            perguntaSolitario.alternativa[3] = auxListPerguntas[index].alternativa4;
            perguntaSolitario.alternativaCorreta = auxListPerguntas[index].correto;
            perguntaSolitario.timer = auxListPerguntas[index].timer;
            grupoPerguntas.listaPerguntas.Add(perguntaSolitario);
            auxListPerguntas.RemoveAt(index);
        }
    }

    public void ListarPesquisaMinhasMesasSolitario(string pesq)
    {
        codBusca = pesq.Replace("\u200b", "");
        conjuntoMesas = new ListaMesas();
        if (pesq.Replace("\u200b", "") != "")
        {
            conjuntoMesas = GetPesquisaListaMesasRequest(pesq.Replace("\u200b", ""));
        }
        else
        {
            conjuntoMesas = auxPesqMesas;
        }
        OrganizandoMesas();
    }

    private ListaMesas GetPesquisaListaMesasRequest(string pesq)
    {
        string valorProcurado = gamecontroller.RemoverAcentos(pesq.ToLowerInvariant());

        List<Mesa> resultado = new List<Mesa>();

        List<Mesa> restoListaMesas = new List<Mesa>();

        foreach (var mesa in auxPesqMesas.mesas)
        {
            string valorFormatado = gamecontroller.RemoverAcentos(mesa.nome?.ToString()?.ToLowerInvariant());

            if (!string.IsNullOrEmpty(valorFormatado) && valorFormatado.Contains(valorProcurado))
            {
                resultado.Add(mesa);
            }
            else
            {
                restoListaMesas.Add(mesa);
            }
        }

        foreach (var mesa in restoListaMesas)
        {
            string valorFormatado = gamecontroller.RemoverAcentos(mesa.codSala?.ToString()?.ToLowerInvariant());

            if (!string.IsNullOrEmpty(valorFormatado) && valorFormatado.Contains(valorProcurado))
            {
                resultado.Add(mesa);
            }
        }

        ListaMesas retorno = new ListaMesas();
        retorno.mesas = resultado.ToArray();
        return retorno;
    }

    private void OrganizandoRecordesTela()
    {
        foreach (GameObject recorde in listaRecorde)
        {
            recorde.SetActive(true);
        }
        if (recordes.listaRecorde.Length > 11)
        {
            paginacaoAnteriorRecorde.SetActive(false);
            paginacaoProximoRecorde.SetActive(true);
            for (int i = 0; i < 11; i++)
            {
                listaRecorde[i].GetComponentsInChildren<TextMeshProUGUI>()[0].text = recordes.listaRecorde[i].nick;
                listaRecorde[i].GetComponentsInChildren<TextMeshProUGUI>()[1].text = recordes.listaRecorde[i].valor.ToString("F2");
                listaRecorde[i].GetComponentsInChildren<TextMeshProUGUI>()[2].text = recordes.listaRecorde[i].pcent_acertos.ToString("F2");
            }
            gruposPaginacaoRecorde = gamecontroller.DividirArray(recordes.listaRecorde, 11);
            paginaRecorde = 1;
        }
        else
        {
            paginacaoAnteriorRecorde.SetActive(false);
            paginacaoProximoRecorde.SetActive(false);
            for (int i = 0; i < 11; i++)
            {
                if (i <= recordes.listaRecorde.Length - 1)
                {
                    listaRecorde[i].GetComponentsInChildren<TextMeshProUGUI>()[0].text = recordes.listaRecorde[i].nick;
                    listaRecorde[i].GetComponentsInChildren<TextMeshProUGUI>()[1].text = recordes.listaRecorde[i].valor.ToString("F2");
                    listaRecorde[i].GetComponentsInChildren<TextMeshProUGUI>()[2].text = recordes.listaRecorde[i].pcent_acertos.ToString("F2");
                }
                else
                {
                    listaRecorde[i].gameObject.SetActive(false);
                }
            }
        }
    }

    private void OrganizandoMesas()
    {
        foreach (GameObject mesa in listaMesasTela)
        {
            mesa.SetActive(true);
        }
        if (conjuntoMesas.mesas.Length > 4)
        {
            paginacaoAnteriorMesa.SetActive(false);
            paginacaoProximoMesa.SetActive(true);
            for (int i = 0; i < 4; i++)
            {
                listaMesasTela[i].GetComponentsInChildren<TextMeshProUGUI>()[0].text = conjuntoMesas.mesas[i].nome;
                listaMesasTela[i].GetComponentsInChildren<TextMeshProUGUI>()[1].text = conjuntoMesas.mesas[i].codSala;
            }
            gruposPaginacaoMesa = gamecontroller.DividirArray(conjuntoMesas.mesas, 4);
            paginaMesa = 1;
        }
        else
        {
            paginacaoAnteriorMesa.SetActive(false);
            paginacaoProximoMesa.SetActive(false);
            for (int i = 0; i < 4; i++)
            {
                if (i <= conjuntoMesas.mesas.Length - 1)
                {
                    listaMesasTela[i].GetComponentsInChildren<TextMeshProUGUI>()[0].text = conjuntoMesas.mesas[i].nome;
                    listaMesasTela[i].GetComponentsInChildren<TextMeshProUGUI>()[1].text = conjuntoMesas.mesas[i].codSala;   
                }
                else
                {
                    listaMesasTela[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void ProximoRecorde()
    {
        gamecontroller.ProximoPaginacao(gruposPaginacaoRecorde.Count, paginacaoAnteriorRecorde, paginacaoProximoRecorde, paginaRecorde);
        paginaRecorde++;
        gamecontroller.ExibirGrupoPaginacaoTela(listaRecorde, GameControler.DadosType.RECORDE, gruposPaginacaoRecorde[paginaRecorde - 1], 11);
    }

    public void AnteriorRecorde()
    {
        gamecontroller.AnteriorPaginacao(gruposPaginacaoRecorde.Count, paginacaoAnteriorMesa, paginacaoProximoMesa, paginaRecorde);
        paginaRecorde--;
        gamecontroller.ExibirGrupoPaginacaoTela(listaRecorde, GameControler.DadosType.RECORDE, gruposPaginacaoRecorde[paginaRecorde - 1], 11);
    }

    public void ProximoMesa()
    {
        gamecontroller.ProximoPaginacao(gruposPaginacaoMesa.Count, paginacaoAnteriorMesa, paginacaoProximoMesa, paginaMesa);
        paginaMesa++;
        gamecontroller.ExibirGrupoPaginacaoTela(listaMesasTela, GameControler.DadosType.MESA, gruposPaginacaoMesa[paginaMesa - 1], 4);
    }

    public void AnteriorMesa()
    {
        gamecontroller.AnteriorPaginacao(gruposPaginacaoMesa.Count, paginacaoAnteriorMesa, paginacaoProximoMesa, paginaMesa);
        paginaMesa--;
        gamecontroller.ExibirGrupoPaginacaoTela(listaMesasTela, GameControler.DadosType.MESA, gruposPaginacaoMesa[paginaMesa - 1], 4);
    }
}
