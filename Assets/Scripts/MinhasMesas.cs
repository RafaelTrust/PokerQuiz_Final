using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Collections.Generic;
using System.Reflection;


public class MinhasMesas : MonoBehaviour
{
    public GameControler gameControler;
    public Animator telas;
    public GameObject telaErro;
    public GameObject telaEditar;
    public TextMeshProUGUI contadorJogadoresEditar;
    public TextMeshProUGUI contadorPerguntasEditar;
    public TextMeshProUGUI contadorJogadoresCriar;
    public TextMeshProUGUI contadorPerguntasCriar;
    public GameObject limitJogadoresEditar;
    public GameObject limitPerguntasEditar;
    public GameObject forcaSenhaEditar;
    public GameObject forcaSenhaCriar;
    public GameObject[] listaPerguntas;
    public GameObject[] listaMesasTela;
    public GameObject paginacaoAnteriorPergunta;
    public GameObject paginacaoProximoPergunta;
    public GameObject paginacaoAnteriorMesa;
    public GameObject paginacaoProximoMesa;
    public GameObject perguntaEditar;
    public GameObject telaPergunta;
    public TextMeshProUGUI contadorPerguntas;
    private bool criar;
    private bool entradaCriar;
    private ListaMesas conjuntoMesas;
    private ListaPerguntasOnline conjuntoPergunta;
    private Mesa mesaRes;
    private MesaLimitado formMesa;
    private bool arrumandoCampoSenha;
    private List<List<PerguntaOnline>> gruposPaginacaoPergunta;
    private List<List<Mesa>> gruposPaginacaoMesa;
    private PerguntaOnlineLimitado formPergunta;
    private List<PerguntaOnlineLimitado> listFormPergunta;
    private int indicePerguntaGravando;
    private int pagina;
    private ListaMesas auxPesqMesas;

    private void Start()
    {
        mesaRes = new Mesa();
        arrumandoCampoSenha = true;
        formMesa = new MesaLimitado();
        formPergunta = new PerguntaOnlineLimitado();
        listFormPergunta = new List<PerguntaOnlineLimitado>();
        pagina = 0;
        indicePerguntaGravando = 0;
        criar = false;
        entradaCriar = false;
    }

    public void VoltarCriacaoMesa()
    {
        entradaCriar = false;
        telas.SetTrigger("CriarMesa");
    }

    public void CadastrarPerguntas()
    {
        if (
            !string.IsNullOrEmpty(formPergunta.enun) &&
            !string.IsNullOrEmpty(formPergunta.alternativa1) &&
            !string.IsNullOrEmpty(formPergunta.alternativa2) &&
            !string.IsNullOrEmpty(formPergunta.alternativa3) &&
            !string.IsNullOrEmpty(formPergunta.alternativa4) &&
            formPergunta.correto != 0 &&
            formPergunta.timer != 0f
          )
        {
            AcrescentandoPerguntaParaCadastrar(formPergunta);
        }
        else
        {
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Erro ao Cadastrar Pergunta. \nTodos os campos s„o obrigatorios";
        }
    }

    private void AcrescentandoPerguntaParaCadastrar(PerguntaOnlineLimitado formularioPergunta)
    {
        listFormPergunta.Add(formularioPergunta);
        int index = 0;
        contadorPerguntas.text = listFormPergunta.Count.ToString();
        formPergunta = new PerguntaOnlineLimitado();
        LimparTelaPerguntas(false);
    }

    public void EntrandoCriarMesa()
    {
        formMesa = new MesaLimitado();
        listFormPergunta = new List<PerguntaOnlineLimitado>();
        criar = true;
        entradaCriar = true;
        telas.SetTrigger("CriarMesa");
    }

    public void CriarPerguntasMesa()
    {
        if (
            !string.IsNullOrEmpty(formMesa.nome) &&
            formMesa.limitJogadores != 0 &&
            formMesa.dinheiroPorJogador != 0.0f &&
            formMesa.limitPerguntas != 0
            )
        {
            telas.SetTrigger("CriarPergunta");
            if (entradaCriar)
            {
                listFormPergunta = new List<PerguntaOnlineLimitado>();
            }
            LimparTelaPerguntas(entradaCriar);
        }
        else
        {
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Erro ao Criar Mesa. \nTodos os campos menos a senha s„o obrigatorios";
        }
    }

    private void LimparTelaPerguntas(bool tudo)
    {
        contadorPerguntas.text = tudo ? "00" : listFormPergunta.Count.ToString();
        TMP_InputField[] listaInputs = telaPergunta.GetComponentsInChildren<TMP_InputField>();
        foreach(TMP_InputField input in listaInputs)
        {
            input.text = string.Empty;
        }
        Toggle[] listaToggle = telaPergunta.GetComponentsInChildren<Toggle>();
        foreach(Toggle toggle in listaToggle)
        {
            toggle.isOn = false;
        }
    }

    public void CriarMesa()
    {
        if(listFormPergunta.Count >= formMesa.limitPerguntas)
        {
            formMesa.responsavel_fk = PlayerPrefs.GetString("id");
            indicePerguntaGravando = 0;
            StartCoroutine(CreateMesaRequest());
        }
        else
        {
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Erro ao Criar Mesa. \nTem que ter no minimo a quantidade limite de perguntas de " + formMesa.limitPerguntas + " perguntas";
        }
    }

    private IEnumerator CreateMesaRequest()
    {
        Debug.Log("Mesa: \nNome: " + formMesa.nome 
            + "\nSenha: " + formMesa.senha
            + "\nLimite Jogadores: " + formMesa.limitJogadores
            + "\nDinheiro: " + formMesa.dinheiroPorJogador
            + "\nLimite Perguntas: " + formMesa.limitPerguntas
            + "\nResponsavel: " + formMesa.responsavel_fk
            );
        var getRequest = gameControler.CreateRequest(
            gameControler.UrlRota + "/salas/cadastro",
            true,
            GameControler.RequestType.POST,
            JsonUtility.ToJson(formMesa)
            );
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Erro ao Criar Mesa. \nTodos os campos menos a senha s„o obrigatorios";
            Debug.LogError(getRequest.error);
        }
        else
        {
            mesaRes = new Mesa();
            conjuntoMesas = JsonUtility.FromJson<ListaMesas>(getRequest.downloadHandler.text);
            foreach (Mesa mesa in conjuntoMesas.mesas)
            {
                if(mesa.nome == formMesa.nome)
                {
                    mesaRes._id = mesa._id;
                }
            }
            StartCoroutine(CreateListaPerguntaRequest(listFormPergunta));
        }
    }

    public void DeletarMesa()
    {
        StartCoroutine(DeleteMesaRequest());
    }

    private IEnumerator DeleteMesaRequest()
    {
        var getRequest = gameControler.CreateRequest(
            gameControler.UrlRota + "/salas/" + mesaRes._id,
            true,
            GameControler.RequestType.DELETE,
            null
            );
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Mesa n„o encontrada. \nVerifique sua conex„o";
            Debug.LogError(getRequest.error);
        }
        else
        {
            telas.SetTrigger("MinhaMesa");
            conjuntoMesas = JsonUtility.FromJson<ListaMesas>(getRequest.downloadHandler.text);
            OrganizandoMesas();
        }
    }

    public void DeletarPergunta(TextMeshProUGUI id)
    {
        StartCoroutine(DeletePerguntaRequest(id.text));
    }

    private IEnumerator DeletePerguntaRequest(string id)
    {
        var getRequest = gameControler.CreateRequest(
            gameControler.UrlRota + "/perguntas/" + id,
            true,
            GameControler.RequestType.DELETE,
            null
            );
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Pergunta n„o encontrada. \nVerifique sua conex„o";
            Debug.LogError(getRequest.error);
        }
        else
        {
            telas.SetTrigger("editar");
            if (conjuntoPergunta.listaPerguntas.Length > 6)
            {
                int[] index = encontrarPerguntas(id, true);

            }
        }
    }

    private int[] encontrarPerguntas(string id, bool matriz)
    {
        int[] index = { 0, 0 };
        if (matriz)
        {
            for (int i = 0; i < gruposPaginacaoPergunta.Count; i++)
            {
                for (int j = 0; j < gruposPaginacaoPergunta[i].Count; j++)
                {
                    if (gruposPaginacaoPergunta[i][j]._id == id)
                    {
                        index[0] = i;
                        index[1] = j;
                        j = gruposPaginacaoPergunta[i].Count;
                        i = gruposPaginacaoPergunta.Count;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < conjuntoPergunta.listaPerguntas.Length; i++)
            {
                if (conjuntoPergunta.listaPerguntas[i]._id == id)
                {
                    index[0] = i;
                    break;
                }
            }
        }
        return index;
    }

    public void EntrarCriarPergunta()
    {
        formPergunta = new PerguntaOnlineLimitado();
        telas.SetTrigger("CriarPergunta");
    }

    public void CriarNovaPergunta()
    {
        if(
            !string.IsNullOrEmpty(formPergunta.enun) &&
            !string.IsNullOrEmpty(formPergunta.alternativa1) &&
            !string.IsNullOrEmpty(formPergunta.alternativa2) &&
            !string.IsNullOrEmpty(formPergunta.alternativa3) &&
            !string.IsNullOrEmpty(formPergunta.alternativa4) &&
            formPergunta.correto != 0 &&
            formPergunta.timer != 0f
          )
        {
            formPergunta.sala_fk = mesaRes._id;
            StartCoroutine(CreatePerguntaUnitariaRequest());
            telas.SetTrigger("CriarPergunta");
        }
        else
        {
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Erro ao Cadastrar Pergunta. \nTodos os campos s„o obrigatorios";
        }
    }

    private IEnumerator CreateListaPerguntaRequest(List<PerguntaOnlineLimitado> listaPergunta)
    {
        Debug.Log("id da mesa:" + mesaRes._id);
        foreach(PerguntaOnlineLimitado pergunta in listaPergunta)
        {
            pergunta.sala_fk = mesaRes._id;
            var getRequest = gameControler.CreateRequest(
            gameControler.UrlRota + "/perguntas",
            true,
            GameControler.RequestType.POST,
            JsonUtility.ToJson(pergunta)
            );
            yield return getRequest.SendWebRequest();
            if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                telaErro.SetActive(true);
                telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Pergunta n„o encontrada. \nVerifique sua conex„o";
                Debug.LogError(getRequest.error);
            }
            else
            {
                conjuntoMesas = JsonUtility.FromJson<ListaMesas>(getRequest.downloadHandler.text);
            }
        }
        telas.SetTrigger("CriarPergunta");
        OrganizandoMesas();
    }

    private IEnumerator CreatePerguntaUnitariaRequest()
    {
        var getRequest = gameControler.CreateRequest(
            gameControler.UrlRota + "/perguntas",
            true,
            GameControler.RequestType.POST,
            JsonUtility.ToJson(formPergunta)
            );
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Pergunta n„o encontrada. \nVerifique sua conex„o";
            Debug.LogError(getRequest.error);
        }
        else
        {
            LimparCamposEditarPergunta();
            conjuntoPergunta = JsonUtility.FromJson<ListaPerguntasOnline>(getRequest.downloadHandler.text);
            foreach (GameObject perguntaTela in listaPerguntas)
            {
                perguntaTela.gameObject.SetActive(true);
            }
            ArrumandoListaPerguntas();
        }
    }

    public void GravarEdicaoPergunta(TextMeshProUGUI id)
    {
        StartCoroutine(UpdatePerguntaRequest(id.text.Replace("\u200b", "")));
    }

    private IEnumerator UpdatePerguntaRequest(string id)
    {
        var getRequest = gameControler.CreateRequest(
            gameControler.UrlRota + "/perguntas/" + id,
            true,
            GameControler.RequestType.PATCH,
            JsonUtility.ToJson(formPergunta)
            );
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Pergunta n„o encontrada. \nVerifique sua conex„o";
            Debug.LogError(getRequest.error);
        }
        else
        {
            telas.SetTrigger("Editar");
            LimparCamposEditarPergunta();
            PerguntaOnline perguntaAtualizada = JsonUtility.FromJson<PerguntaOnline>(getRequest.downloadHandler.text);
            if (conjuntoPergunta.listaPerguntas.Length > 6)
            {
                int[] index = encontrarPerguntas(id, true);
                gruposPaginacaoPergunta[index[0]][index[1]].enun = perguntaAtualizada.enun;
                gruposPaginacaoPergunta[index[0]][index[1]].alternativa1 = perguntaAtualizada.alternativa1;
                gruposPaginacaoPergunta[index[0]][index[1]].alternativa2 = perguntaAtualizada.alternativa2;
                gruposPaginacaoPergunta[index[0]][index[1]].alternativa3 = perguntaAtualizada.alternativa3;
                gruposPaginacaoPergunta[index[0]][index[1]].alternativa4 = perguntaAtualizada.alternativa4;
                gruposPaginacaoPergunta[index[0]][index[1]].correto = perguntaAtualizada.correto;
                gruposPaginacaoPergunta[index[0]][index[1]].timer = perguntaAtualizada.timer;
            }
            else
            {
                int[] index = encontrarPerguntas(id, false);
                conjuntoPergunta.listaPerguntas[index[0]].enun = perguntaAtualizada.enun;
                conjuntoPergunta.listaPerguntas[index[0]].alternativa1 = perguntaAtualizada.alternativa1;
                conjuntoPergunta.listaPerguntas[index[0]].alternativa2 = perguntaAtualizada.alternativa2;
                conjuntoPergunta.listaPerguntas[index[0]].alternativa3 = perguntaAtualizada.alternativa3;
                conjuntoPergunta.listaPerguntas[index[0]].alternativa4 = perguntaAtualizada.alternativa4;
                conjuntoPergunta.listaPerguntas[index[0]].correto = perguntaAtualizada.correto;
                conjuntoPergunta.listaPerguntas[index[0]].timer = perguntaAtualizada.timer;
            }
        }
    }

    public void OnEnunciadoPerguntaChanged(string enun)
    {
        if (!string.IsNullOrEmpty(enun))
        {
            formPergunta.enun = enun.Replace("\u200b", "");
        }
    }

    public void OnAlternativa1PerguntaChanged(string alternativa1)
    {
        if (!string.IsNullOrEmpty(alternativa1))
        {
            formPergunta.alternativa1 = alternativa1.Replace("\u200b", "");
        }
    }

    public void OnAlternativa2PerguntaChanged(string alternativa2)
    {
        if (!string.IsNullOrEmpty(alternativa2))
        {
            formPergunta.alternativa2 = alternativa2.Replace("\u200b", "");
        }
    }

    public void OnAlternativa3PerguntaChanged(string alternativa3)
    {
        if (!string.IsNullOrEmpty(alternativa3))
        {
            formPergunta.alternativa3 = alternativa3.Replace("\u200b", "");
        }
    }

    public void OnAlternativa4PerguntaChanged(string alternativa4)
    {
        if (!string.IsNullOrEmpty(alternativa4))
        {
            formPergunta.alternativa4 = alternativa4.Replace("\u200b", "");
        }
    }

    public void OnTimerPerguntaChanged(string timer)
    {
        if (!string.IsNullOrEmpty(timer))
        {
            formPergunta.timer = float.Parse(timer.Replace("\u200b", ""));
        }
    }

    public void OnTogglePerguntaChanged(int correto)
    {
        Toggle[] listToggle = criar? telaPergunta.GetComponentsInChildren<Toggle>() : perguntaEditar.GetComponentsInChildren<Toggle>();
        for(int i = 0; i < listToggle.Length; i++)
        {
            if(i != correto - 1) 
            {
                listToggle[i].isOn = false;
            }
        }
        formPergunta.correto = correto;
    }

    public void BuscarPergunta(TextMeshProUGUI id)
    {
        telas.SetTrigger("Editar");
        foreach (var pergunta in conjuntoPergunta.listaPerguntas)
        {
            if (pergunta._id == id.text.Replace("\u200b", ""))
            {
                PreencherCamposEditarPergunta(pergunta);
                break;
            }
        }
    }

    private void PreencherCamposEditarPergunta(PerguntaOnline pergunta)
    {
        perguntaEditar.GetComponentsInChildren<TMP_InputField>()[0].GetComponentsInChildren<TextMeshProUGUI>()[0].text = pergunta.enun;
        perguntaEditar.GetComponentsInChildren<TMP_InputField>()[1].GetComponentsInChildren<TextMeshProUGUI>()[0].text = pergunta.alternativa1;
        perguntaEditar.GetComponentsInChildren<TMP_InputField>()[2].GetComponentsInChildren<TextMeshProUGUI>()[0].text = pergunta.alternativa2;
        perguntaEditar.GetComponentsInChildren<TMP_InputField>()[3].GetComponentsInChildren<TextMeshProUGUI>()[0].text = pergunta.alternativa3;
        perguntaEditar.GetComponentsInChildren<TMP_InputField>()[4].GetComponentsInChildren<TextMeshProUGUI>()[0].text = pergunta.alternativa4;
        perguntaEditar.GetComponentsInChildren<Toggle>()[pergunta.correto - 1].isOn = true;
        perguntaEditar.GetComponentsInChildren<TMP_InputField>()[5].GetComponentsInChildren<TextMeshProUGUI>()[0].text = pergunta.timer.ToString();
        perguntaEditar.GetComponentsInChildren<TextMeshProUGUI>()[0].text = pergunta._id;
        formPergunta.enun = pergunta.enun;
        formPergunta.alternativa1 = pergunta.alternativa1;
        formPergunta.alternativa2 = pergunta.alternativa2;
        formPergunta.alternativa3 = pergunta.alternativa3;
        formPergunta.alternativa4 = pergunta.alternativa4;
        formPergunta.correto = pergunta.correto;
        formPergunta.timer = pergunta.timer;
        formPergunta.sala_fk = pergunta.sala_fk;
    }

    public void LimparCamposEditarPergunta()
    {
        TMP_InputField[] listInputs = perguntaEditar.GetComponentsInChildren<TMP_InputField>();
        foreach(TMP_InputField input in listInputs)
        {
            input.GetComponentsInChildren<TextMeshProUGUI>()[0].text = string.Empty;
            input.text = string.Empty;
        }
        Toggle[] listToggle = perguntaEditar.GetComponentsInChildren<Toggle>();
        foreach(Toggle toggle in listToggle)
        {
            toggle.isOn = false;
        }
        perguntaEditar.GetComponentsInChildren<TextMeshProUGUI>()[0].text = string.Empty;
        formPergunta = new PerguntaOnlineLimitado();
    }

    public void SairPaginacao()
    {
        pagina = 0;
        gruposPaginacaoPergunta = new List<List<PerguntaOnline>>();
    }

    public void ProximoPergunta()
    {
        gameControler.ProximoPaginacao(gruposPaginacaoPergunta.Count, paginacaoAnteriorPergunta, paginacaoProximoPergunta, pagina);
        pagina++;
        gameControler.ExibirGrupoPaginacaoTela(listaPerguntas, GameControler.DadosType.PERGUNTA, gruposPaginacaoPergunta[pagina - 1]);
    }

    public void AnteriorPergunta()
    {
        gameControler.AnteriorPaginacao(gruposPaginacaoPergunta.Count, paginacaoAnteriorPergunta, paginacaoProximoPergunta, pagina);
        pagina--;
        gameControler.ExibirGrupoPaginacaoTela(listaPerguntas, GameControler.DadosType.PERGUNTA, gruposPaginacaoPergunta[pagina - 1]);
    }

    public void BuscarTodasPerguntas()
    {
        StartCoroutine(GetListPerguntaRequest());
    }

    private IEnumerator GetListPerguntaRequest()
    {
        var getRequest = gameControler.CreateRequest(
            gameControler.UrlRota + "/perguntas/sala/" + mesaRes._id,
            true,
            GameControler.RequestType.GET,
            null
            );
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Lista n„o encontrada. \nVerifique sua conex„o";
            Debug.LogError(getRequest.error);
        }
        else
        {
            telas.SetTrigger("Pergunta");
            conjuntoPergunta = JsonUtility.FromJson<ListaPerguntasOnline>(getRequest.downloadHandler.text);
            ArrumandoListaPerguntas();
        }
    }

    private void ArrumandoListaPerguntas()
    {
        if (conjuntoPergunta.listaPerguntas.Length > 7)
        {
            paginacaoProximoPergunta.SetActive(true);
            paginacaoAnteriorPergunta.SetActive(false);
            for (int i = 0; i < 6; i++)
            {
                listaPerguntas[i].GetComponentsInChildren<TextMeshProUGUI>()[0].text = conjuntoPergunta.listaPerguntas[i].enun;
                listaPerguntas[i].GetComponentsInChildren<TextMeshProUGUI>()[1].text = conjuntoPergunta.listaPerguntas[i]._id;
            }
            gruposPaginacaoPergunta = gameControler.DividirArray(conjuntoPergunta.listaPerguntas, 6);
            pagina = 1;
        }
        else
        {
            paginacaoProximoPergunta.SetActive(false);
            paginacaoAnteriorPergunta.SetActive(false);
            for (int i = 0; i < 6; i++)
            {
                if (conjuntoPergunta.listaPerguntas.Length > i)
                {
                    listaPerguntas[i].GetComponentsInChildren<TextMeshProUGUI>()[0].text = conjuntoPergunta.listaPerguntas[i].enun;
                    listaPerguntas[i].GetComponentsInChildren<TextMeshProUGUI>()[1].text = conjuntoPergunta.listaPerguntas[i]._id;
                }
                else
                {
                    listaPerguntas[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void OnToggleSenhaChanged(bool troca)
    {
        arrumandoCampoSenha = troca;
    }

    public void OnLimiteJogadoresChanged(float value)
    {
        if(value != mesaRes.limitJogadores)
        {
            if (criar)
            {
                contadorJogadoresCriar.text = value.ToString();
            }
            else
            {
                contadorJogadoresEditar.text = value.ToString();
            }
            formMesa.limitJogadores = (int)value;
        }
    }

    public void OnLimitePerguntasChanged(float value)
    {
        if(value != mesaRes.limitPerguntas)
        {
            if (criar)
            {
                contadorPerguntasCriar.text = value.ToString();
            }
            else
            {
                contadorPerguntasEditar.text = value.ToString();
            }
            formMesa.limitPerguntas = (int)value;

        }
    }

    public void OnDinheroByJogadorChanged(string dinheiro)
    {
        formMesa.dinheiroPorJogador = float.Parse(dinheiro.Replace("\u200b", ""));
    }

    public void OnNomeMesaChanged(string nome)
    {
        formMesa.nome = nome.Replace("\u200b", "");
    }

    public void OnSenhaChanged(string senha)
    {
        formMesa.senha = senha.Replace("\u200b", "");
        GameObject forcaSenha = criar ? forcaSenhaCriar : forcaSenhaEditar;
        int forca = gameControler.MedindoForcaSenha(senha);
        forcaSenha.GetComponentsInChildren<Slider>()[0].value = forca;
        forcaSenha.GetComponentsInChildren<TextMeshProUGUI>()[0].text = gameControler.GetForcaText(forca);
        forcaSenha.GetComponentsInChildren<TextMeshProUGUI>()[0].color = gameControler.GetColorForca(forca);
    }

    public void GravandoEdicaoMesa()
    {
        formMesa.nome = !string.IsNullOrEmpty(formMesa.nome) ? formMesa.nome : mesaRes.nome;
        if (arrumandoCampoSenha)
        {
            formMesa.senha = !string.IsNullOrEmpty(formMesa.senha) ? formMesa.senha : mesaRes.senha;
        }
        else
        {
            formMesa.senha = null;
        }
        formMesa.limitJogadores = formMesa.limitJogadores != mesaRes.limitJogadores ? mesaRes.limitJogadores : formMesa.limitJogadores;
        formMesa.limitPerguntas = formMesa.limitPerguntas != mesaRes.limitPerguntas ? mesaRes.limitPerguntas : formMesa.limitPerguntas;
        formMesa.dinheiroPorJogador = formMesa.dinheiroPorJogador != mesaRes.dinheiroPorJogador ? mesaRes.dinheiroPorJogador : formMesa.dinheiroPorJogador;
        formMesa.codSala = mesaRes.codSala;
        formMesa.responsavel_fk = mesaRes.responsavel_fk;
        StartCoroutine(PostMesaRequest());
    }

    private IEnumerator PostMesaRequest()
    { 
        var getRequest = gameControler.CreateRequest(
            gameControler.UrlRota + "/salas/" + mesaRes._id,
            true,
            GameControler.RequestType.PATCH,
            JsonUtility.ToJson(formMesa)
            );
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Mesa n„o encontrada. \nVerifique sua conex„o";
            Debug.LogError(getRequest.error);
        }
        else
        {
            telas.SetTrigger("Editar");
            mesaRes = JsonUtility.FromJson<Mesa>(getRequest.downloadHandler.text);
            ColocandoValores();
        }
    }

    public void LimpandoValores()
    {
        ColocandoValores();
    }

    public void BuscarMinhaMesa(TextMeshProUGUI cod)
    {
        criar = false;
        StartCoroutine(GetMesaRequest(cod.text));
    }

    private IEnumerator GetMesaRequest(string cod)
    {
        var getRequest = gameControler.CreateRequest(
            gameControler.UrlRota + "/salas/" + cod,
            true,
            GameControler.RequestType.GET,
            null
            );
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Mesa n„o encontrada. \nVerifique sua conex„o";
            Debug.LogError(getRequest.error);
        }
        else
        {
            telas.SetTrigger("EditarMesa");
            mesaRes = JsonUtility.FromJson<Mesa>(getRequest.downloadHandler.text);
            ColocandoValores();
        }
    }

    private void ColocandoValores()
    {
        telaEditar.GetComponentsInChildren<TMP_InputField>()[0].text = string.Empty;
        telaEditar.GetComponentsInChildren<TMP_InputField>()[0].GetComponentsInChildren<TextMeshProUGUI>()[0].text = mesaRes.nome;
        if (!string.IsNullOrEmpty(mesaRes.senha))
        {
            telaEditar.GetComponentsInChildren<Toggle>()[0].isOn = true;
            telaEditar.GetComponentsInChildren<TMP_InputField>()[1].text = string.Empty;
            telaEditar.GetComponentsInChildren<TMP_InputField>()[1].GetComponentsInChildren<TextMeshProUGUI>()[0].text = mesaRes.senha;
            GameObject forcaSenha = forcaSenhaEditar;
            int forca = gameControler.MedindoForcaSenha(mesaRes.senha);
            forcaSenha.GetComponentsInChildren<Slider>()[0].value = forca;
            forcaSenha.GetComponentsInChildren<TextMeshProUGUI>()[0].text = gameControler.GetForcaText(forca);
            forcaSenha.GetComponentsInChildren<TextMeshProUGUI>()[0].color = gameControler.GetColorForca(forca);
        }
        else
        {
            telaEditar.GetComponentsInChildren<Toggle>()[0].isOn = false;
            telaEditar.GetComponentsInChildren<TMP_InputField>()[1].text = string.Empty;
            telaEditar.GetComponentsInChildren<TMP_InputField>()[1].GetComponentsInChildren<TextMeshProUGUI>()[0].text = string.Empty;
        }
        limitJogadoresEditar.GetComponent<Slider>().value = mesaRes.limitJogadores;
        limitJogadoresEditar.GetComponentsInChildren<TextMeshProUGUI>()[0].text = mesaRes.limitJogadores.ToString();
        telaEditar.GetComponentsInChildren<TMP_InputField>()[2].text = string.Empty;
        telaEditar.GetComponentsInChildren<TMP_InputField>()[2].GetComponentsInChildren<TextMeshProUGUI>()[0].text = mesaRes.dinheiroPorJogador.ToString();
        limitPerguntasEditar.GetComponent<Slider>().value = mesaRes.limitPerguntas;
        limitPerguntasEditar.GetComponentsInChildren<TextMeshProUGUI>()[0].text = mesaRes.limitPerguntas.ToString();
    }

    public void ListarPesquisaMinhasMesas(string pesq)
    {
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
        string valorProcurado = gameControler.RemoverAcentos(pesq.ToLowerInvariant());

        List<Mesa> resultado = new List<Mesa>();

        List<Mesa> restoListaMesas = new List<Mesa>();

        foreach (var mesa in auxPesqMesas.mesas)
        {
            string valorFormatado = gameControler.RemoverAcentos(mesa.nome?.ToString()?.ToLowerInvariant());

            if(!string.IsNullOrEmpty(valorFormatado) && valorFormatado.Contains(valorProcurado))
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
            string valorFormatado = gameControler.RemoverAcentos(mesa.codSala?.ToString()?.ToLowerInvariant());

            if (!string.IsNullOrEmpty(valorFormatado) && valorFormatado.Contains(valorProcurado))
            {
                resultado.Add(mesa);
            }
        }

        ListaMesas retorno = new ListaMesas();
        retorno.mesas = resultado.ToArray();
        return retorno;
    }

    

    public void ListarMinhasMesas()
    {
        StartCoroutine(GetListaMesasRequest());
    }
    private IEnumerator GetListaMesasRequest()
    {
        var getRequest = gameControler.CreateRequest(
            gameControler.UrlRota + "/salas/responsavel/" + PlayerPrefs.GetString("id"),
            true,
            GameControler.RequestType.GET,
            null
            );
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            telas.SetTrigger("Login");
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Usuario n„o Encontrado. \n… necess·rio estar logado para acessar esse modulo";
            Debug.LogError(getRequest.error);
        }
        else
        {
            conjuntoMesas = new ListaMesas();
            auxPesqMesas = new ListaMesas();
            telas.SetTrigger("MinhaMesa");
            conjuntoMesas = JsonUtility.FromJson<ListaMesas>(getRequest.downloadHandler.text);
            auxPesqMesas = JsonUtility.FromJson<ListaMesas>(getRequest.downloadHandler.text);
            OrganizandoMesas();
        }
    }

    private void OrganizandoMesas()
    {
        foreach(GameObject mesa in listaMesasTela)
        {
            mesa.SetActive(true);
        }
        if (conjuntoMesas.mesas.Length >= 5)
        {
            paginacaoAnteriorMesa.SetActive(false);
            paginacaoProximoMesa.SetActive(true);
            for (int i = 0; i < 4; i++)
            {
                listaMesasTela[i].GetComponentsInChildren<TextMeshProUGUI>()[0].text = conjuntoMesas.mesas[i].nome;
                listaMesasTela[i].GetComponentsInChildren<TextMeshProUGUI>()[1].text = conjuntoMesas.mesas[i].codSala;
            }
            gruposPaginacaoMesa = gameControler.DividirArray(conjuntoMesas.mesas, 4);
            pagina = 1;
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

    public void ProximoMesa()
    {
        gameControler.ProximoPaginacao(gruposPaginacaoMesa.Count, paginacaoAnteriorMesa, paginacaoProximoMesa, pagina);
        pagina++;
        gameControler.ExibirGrupoPaginacaoTela(listaMesasTela, GameControler.DadosType.MESA, gruposPaginacaoMesa[pagina - 1]);
    }

    public void AnteriorMesa()
    {
        gameControler.AnteriorPaginacao(gruposPaginacaoMesa.Count, paginacaoAnteriorMesa, paginacaoProximoMesa, pagina);
        pagina--;
        gameControler.ExibirGrupoPaginacaoTela(listaMesasTela, GameControler.DadosType.MESA, gruposPaginacaoMesa[pagina - 1]);
    }

   
}
