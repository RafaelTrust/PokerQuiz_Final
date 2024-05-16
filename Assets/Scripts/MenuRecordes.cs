using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
public class MenuRecordes : MonoBehaviour
{
    public GameControler gameControler;
    public GameObject perguntaTitulo;
    public JogoSolitario jogoSolitario;
    public GameObject telaEscolha;
    public GameObject[] listaEstatisticas;
    public GameObject[] listaMesas;
    public GameObject telaPergunta;
    public GameObject paginacaoProximoMesa;
    public GameObject paginacaoAnteriorMesa;
    public GameObject paginacaoAnteriorEstatisticas;
    public GameObject paginacaoProximoEstatisticas;
    public GameObject telaErroPrincipal;
    public Animator telasAnimator;
    public Animator estatisticaAnimator;
    public Animator carregamento;
    private List<List<Mesa>> gruposPaginacaoMesa;
    private List<List<EstatisticaPorcentagem>> gruposPaginacaoEstatistica;
    private ListaEstatisticaPorcentagem conjuntoEstatistica;
    private ListaEstatisticaPorcentagem completoEstatistica;
    private ListaMesas conjuntoMesas;
    private ListaMesas auxPesqMesas;
    private int paginaMesa;
    private int paginaEstatistica;
    private string codBusca;
    private string codSala;

    public void EntrarEscolha(TextMeshProUGUI cod)
    {
        codSala = cod.text;
        telaEscolha.SetActive(true);
    }

    public void SairEscolha()
    {
        telaEscolha.SetActive(false);
    }

    public void MostrarPergunta(int index)
    {
        perguntaTitulo.GetComponentsInChildren<TextMeshProUGUI>()[1].text = listaEstatisticas[index].GetComponentsInChildren<TextMeshProUGUI>()[1].text;
        perguntaTitulo.GetComponentsInChildren<TextMeshProUGUI>()[2].text = listaEstatisticas[index].GetComponentsInChildren<TextMeshProUGUI>()[2].text;
        perguntaTitulo.GetComponentsInChildren<TextMeshProUGUI>()[3].text = listaEstatisticas[index].GetComponentsInChildren<TextMeshProUGUI>()[3].text;
        perguntaTitulo.GetComponentsInChildren<TextMeshProUGUI>()[4].text = listaEstatisticas[index].GetComponentsInChildren<TextMeshProUGUI>()[4].text;
        perguntaTitulo.GetComponentsInChildren<TextMeshProUGUI>()[5].text = listaEstatisticas[index].GetComponentsInChildren<TextMeshProUGUI>()[5].text;
        StartCoroutine(GetPergunta(listaEstatisticas[index].GetComponentsInChildren<TextMeshProUGUI>()[0].text, index));
    }

    private IEnumerator GetPergunta(string pergunta_fk, int index)
    {
        carregamento.SetTrigger("carregar");
        var getRequest = gameControler.CreateRequest(
            gameControler.UrlRota + "/perguntas/" + pergunta_fk,
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
        }
        else
        {
            carregamento.SetTrigger("carregar");
            PerguntaOnline pergunta = JsonUtility.FromJson<PerguntaOnline>(getRequest.downloadHandler.text);
            OrganizandoPergunta(pergunta);
            estatisticaAnimator.SetTrigger("estatistica");
        }
    }

    public void OrganizandoPergunta(PerguntaOnline pergunta)
    {
        telaPergunta.GetComponentsInChildren<TextMeshProUGUI>()[0].text = pergunta.enun;
        telaPergunta.GetComponentsInChildren<TextMeshProUGUI>()[1].text = pergunta.alternativa1;
        telaPergunta.GetComponentsInChildren<TextMeshProUGUI>()[2].text = pergunta.alternativa2;
        telaPergunta.GetComponentsInChildren<TextMeshProUGUI>()[3].text = pergunta.alternativa3;
        telaPergunta.GetComponentsInChildren<TextMeshProUGUI>()[4].text = pergunta.alternativa4;
    }

    public void BuscarEstatisticasBySala()
    {
        StartCoroutine(GetListaEstatisticasBySala(codSala));
        telaEscolha.SetActive(false);
    }

    private IEnumerator GetListaEstatisticasBySala(string cod)
    {
        carregamento.SetTrigger("carregar");
        var getRequest = gameControler.CreateRequest(
            gameControler.UrlRota + "/recorde/estatistica/" + cod,
            false,
            GameControler.RequestType.GET,
            null
            );
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            carregamento.SetTrigger("carregar");
            telaErroPrincipal.SetActive(true);
            telaErroPrincipal.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Estatisticas não enconntradas. \nVerifique sua conexão.";
            Debug.LogError(getRequest.error);
        }
        else
        {
            carregamento.SetTrigger("carregar");
            ListaEstatistica auxListaEstatisticas = JsonUtility.FromJson<ListaEstatistica>(getRequest.downloadHandler.text);
            List<EstatisticaPorcentagem> auxConjunto = new List<EstatisticaPorcentagem>();
            conjuntoEstatistica = new ListaEstatisticaPorcentagem();
            completoEstatistica = new ListaEstatisticaPorcentagem();
            List<EstatisticaOnline> listaCompleta = new List<EstatisticaOnline>();
            if(auxListaEstatisticas.listaEstatistica.Length > 0)
            {
                for (int i = 0; i < auxListaEstatisticas.listaEstatistica.Length; i++)
                {
                    listaCompleta.Add(auxListaEstatisticas.listaEstatistica[i]);
                }
                listaCompleta.Sort((pergunta1, pergunta2) => pergunta1.pergunta_fk.CompareTo(pergunta2.pergunta_fk));

                int[] alternativas = { 0, 0, 0, 0 };
                int correto = 0;
                int total = 0;

                string pergunta_fk = listaCompleta[0].pergunta_fk;
                for (int i = 0; i < listaCompleta.Count + 1; i++)
                {
                    if (i < listaCompleta.Count && pergunta_fk == listaCompleta[i].pergunta_fk)
                    {
                        correto = listaCompleta[i].correto;
                        total++;
                        alternativas[listaCompleta[i].alternativa - 1]++;
                    }
                    else
                    {
                        int index = 0;
                        if (i < listaCompleta.Count)
                        {
                            index = i;
                        }
                        else
                        {
                            index = i - 1;
                        }
                        EstatisticaPorcentagem auxEstatistica = new EstatisticaPorcentagem();
                        auxEstatistica.pergunta_fk = pergunta_fk;
                        float acertos = (float)alternativas[correto - 1] / total * 100f;
                        auxEstatistica.acertos = "% " + acertos.ToString("F2");
                        float alternativa1 = (float)alternativas[0] / total * 100f;
                        auxEstatistica.alternativa1 = "% " + alternativa1.ToString("F2");
                        float alternativa2 = (float)alternativas[1] / total * 100f;
                        auxEstatistica.alternativa2 = "% " + alternativa2.ToString("F2");
                        float alternativa3 = (float)alternativas[2] / total * 100f;
                        auxEstatistica.alternativa3 = "% " + alternativa3.ToString("F2");
                        float alternativa4 = (float)alternativas[3] / total * 100f;
                        auxEstatistica.alternativa4 = "% " + alternativa4.ToString("F2");
                        auxEstatistica.correto = correto;
                        auxConjunto.Add(auxEstatistica);
                        pergunta_fk = listaCompleta[index].pergunta_fk;
                        for (int j = 0; j < 4; j++)
                        {
                            alternativas[j] = 0;
                        }
                        alternativas[listaCompleta[index].alternativa - 1] = 1;
                        correto = listaCompleta[index].correto;
                        total = 1;
                    }
                }
                conjuntoEstatistica.listaPorcenntagem = auxConjunto.ToArray();
                completoEstatistica.listaPorcenntagem = auxConjunto.ToArray();
                OrganizandoEstatisticas();
                estatisticaAnimator.SetTrigger("estatistica");
            }
            else
            {
                telaErroPrincipal.SetActive(true);
                telaErroPrincipal.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Estatisticas inexistentes.";
            }
            
        }
    }

    public void BuscarMinhaMesaRecordeButton()
    {
        codSala = codBusca;
        telaEscolha.SetActive(true);
    }

    public void BuscarMinhaMesaRecorde()
    {
        jogoSolitario.logadoToggle.isOn = false;
        jogoSolitario.TudoDataToggle(true);
        jogoSolitario.NickEscolhidoDropdown(0);
        jogoSolitario.NomesRepetidosDataToggle(true);
        telaEscolha.SetActive(false);
        StartCoroutine(jogoSolitario.ListaRecordeSala(codSala, true));
    }

    public void ListarPesquisaMinhasMesasRecorde(string pesq)
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
        string valorProcurado = gameControler.RemoverAcentos(pesq.ToLowerInvariant());

        List<Mesa> resultado = new List<Mesa>();

        List<Mesa> restoListaMesas = new List<Mesa>();

        foreach (var mesa in auxPesqMesas.mesas)
        {
            string valorFormatado = gameControler.RemoverAcentos(mesa.nome?.ToString()?.ToLowerInvariant());

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

    public void ListarMinhasMesasRecorde()
    {
        StartCoroutine(GetListaMesasRecordeRequest());
    }
    private IEnumerator GetListaMesasRecordeRequest()
    {
        carregamento.SetTrigger("carregar");
        var getRequest = gameControler.CreateRequest(
            gameControler.UrlRota + "/salas/publicas",
            false,
            GameControler.RequestType.GET,
            null
            );
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            carregamento.SetTrigger("carregar");
            telaErroPrincipal.SetActive(true);
            telaErroPrincipal.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Recordes não encontrados. \nVerifique sua conexão.";
            Debug.LogError(getRequest.error);
        }
        else
        {
            conjuntoMesas = new ListaMesas();
            auxPesqMesas = new ListaMesas();
            ListaMesas auxListaPublicaOnline = JsonUtility.FromJson<ListaMesas>(getRequest.downloadHandler.text);
            List<Mesa> listaMesas = new List<Mesa>();
            foreach(Mesa mesa in auxListaPublicaOnline.mesas)
            {
                listaMesas.Add(mesa);
            }
            ListaJogoOffline auxSalaPerguntas = JsonUtility.FromJson<ListaJogoOffline>(PlayerPrefs.GetString("salaPerguntasOffiline"));
            if(auxSalaPerguntas != null)
            {
                foreach (SalaPerguntas salaPergunta in auxSalaPerguntas.listaOffline)
                {
                    bool adicionar = true;
                    foreach (Mesa mesa in listaMesas)
                    {
                        if (salaPergunta.sala._id == mesa._id)
                        {
                            adicionar = false;
                        }
                    }
                    if (adicionar)
                    {
                        listaMesas.Add(salaPergunta.sala);
                    }
                }
            }
            conjuntoMesas.mesas = listaMesas.ToArray();
            auxPesqMesas.mesas = listaMesas.ToArray();
            StartCoroutine(GetListaMesasRequest());
        }
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
            carregamento.SetTrigger("carregar");
            telasAnimator.SetTrigger("Recorde");
            telaErroPrincipal.SetActive(true);
            telaErroPrincipal.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Usuario não logado. \n para acessar suas mesas precisa estar logado";
            OrganizandoMesas();
            Debug.LogError(getRequest.error);
        }
        else
        {
            carregamento.SetTrigger("carregar");
            telasAnimator.SetTrigger("Recorde");
            ListaMesas auxListaMesa = JsonUtility.FromJson<ListaMesas>(getRequest.downloadHandler.text);
            List<Mesa> listaMesas = new List<Mesa>();
            foreach(Mesa mesa in conjuntoMesas.mesas)
            {
                listaMesas.Add(mesa);
            }
            if(auxListaMesa != null)
            {
                foreach (Mesa mesa in auxListaMesa.mesas)
                {
                    bool adicionar = true;
                    foreach (Mesa mesaPublica in listaMesas)
                    {
                        if (mesa.codSala == mesaPublica.codSala)
                        {
                            adicionar = false;
                        }
                    }
                    if (adicionar)
                    {
                        listaMesas.Add(mesa);
                    }
                }
            }
            conjuntoMesas = new ListaMesas();
            auxPesqMesas = new ListaMesas();
            conjuntoMesas.mesas = listaMesas.ToArray();
            auxPesqMesas.mesas = listaMesas.ToArray();
            OrganizandoMesas();
        }
    }

    private void OrganizandoEstatisticas()
    {
        foreach (GameObject estatisticas in listaEstatisticas)
        {
            estatisticas.SetActive(true);
        }
        if (conjuntoMesas.mesas.Length >= 20)
        {
            paginacaoAnteriorEstatisticas.SetActive(false);
            paginacaoProximoEstatisticas.SetActive(true);
            for (int i = 0; i < 20; i++)
            {
                listaEstatisticas[i].GetComponentsInChildren<TextMeshProUGUI>()[0].text = conjuntoEstatistica.listaPorcenntagem[i].pergunta_fk;
                listaEstatisticas[i].GetComponentsInChildren<TextMeshProUGUI>()[1].text = conjuntoEstatistica.listaPorcenntagem[i].acertos;
                listaEstatisticas[i].GetComponentsInChildren<TextMeshProUGUI>()[2].text = conjuntoEstatistica.listaPorcenntagem[i].alternativa1;
                listaEstatisticas[i].GetComponentsInChildren<TextMeshProUGUI>()[3].text = conjuntoEstatistica.listaPorcenntagem[i].alternativa2;
                listaEstatisticas[i].GetComponentsInChildren<TextMeshProUGUI>()[4].text = conjuntoEstatistica.listaPorcenntagem[i].alternativa3;
                listaEstatisticas[i].GetComponentsInChildren<TextMeshProUGUI>()[5].text = conjuntoEstatistica.listaPorcenntagem[i].alternativa4;
            }
            gruposPaginacaoEstatistica = gameControler.DividirArray(conjuntoEstatistica.listaPorcenntagem, 20);
            paginaEstatistica = 1;
        }
        else
        {
            paginacaoAnteriorEstatisticas.SetActive(false);
            paginacaoProximoEstatisticas.SetActive(false);
            for (int i = 0; i < 20; i++)
            {
                if (i <= conjuntoEstatistica.listaPorcenntagem.Length - 1)
                {
                    listaEstatisticas[i].GetComponentsInChildren<TextMeshProUGUI>()[0].text = conjuntoEstatistica.listaPorcenntagem[i].pergunta_fk;
                    listaEstatisticas[i].GetComponentsInChildren<TextMeshProUGUI>()[1].text = conjuntoEstatistica.listaPorcenntagem[i].acertos;
                    listaEstatisticas[i].GetComponentsInChildren<TextMeshProUGUI>()[2].text = conjuntoEstatistica.listaPorcenntagem[i].alternativa1;
                    listaEstatisticas[i].GetComponentsInChildren<TextMeshProUGUI>()[3].text = conjuntoEstatistica.listaPorcenntagem[i].alternativa2;
                    listaEstatisticas[i].GetComponentsInChildren<TextMeshProUGUI>()[4].text = conjuntoEstatistica.listaPorcenntagem[i].alternativa3;
                    listaEstatisticas[i].GetComponentsInChildren<TextMeshProUGUI>()[5].text = conjuntoEstatistica.listaPorcenntagem[i].alternativa4;
                }
                else
                {
                    Debug.Log("index: " + i);
                    listaEstatisticas[i].gameObject.SetActive(false);
                }
            }
        }
    }

    private void OrganizandoMesas()
    {
        foreach (GameObject mesa in listaMesas)
        {
            mesa.SetActive(true);
        }
        if (conjuntoMesas.mesas.Length > 4)
        {
            paginacaoAnteriorMesa.SetActive(false);
            paginacaoProximoMesa.SetActive(true);
            for (int i = 0; i < 4; i++)
            {
                listaMesas[i].GetComponentsInChildren<TextMeshProUGUI>()[0].text = conjuntoMesas.mesas[i].nome;
                listaMesas[i].GetComponentsInChildren<TextMeshProUGUI>()[1].text = conjuntoMesas.mesas[i].codSala;
            }
            gruposPaginacaoMesa = gameControler.DividirArray(conjuntoMesas.mesas, 4);
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
                    listaMesas[i].GetComponentsInChildren<TextMeshProUGUI>()[0].text = conjuntoMesas.mesas[i].nome;
                    listaMesas[i].GetComponentsInChildren<TextMeshProUGUI>()[1].text = conjuntoMesas.mesas[i].codSala;
                }
                else
                {
                    listaMesas[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void ProximoEstatistica()
    {
        gameControler.ProximoPaginacao(gruposPaginacaoEstatistica.Count, paginacaoAnteriorEstatisticas, paginacaoProximoEstatisticas, paginaEstatistica);
        paginaEstatistica++;
        gameControler.ExibirGrupoPaginacaoTela(listaEstatisticas, GameControler.DadosType.ESTATISTICA, gruposPaginacaoEstatistica[paginaEstatistica - 1], 20);
    }

    public void AnteriorEstatistica()
    {
        gameControler.AnteriorPaginacao(gruposPaginacaoEstatistica.Count, paginacaoAnteriorEstatisticas, paginacaoProximoEstatisticas, paginaEstatistica);
        paginaEstatistica--;
        gameControler.ExibirGrupoPaginacaoTela(listaEstatisticas, GameControler.DadosType.ESTATISTICA, gruposPaginacaoEstatistica[paginaEstatistica - 1], 20);
    }

    public void ProximoMesa()
    {
        gameControler.ProximoPaginacao(gruposPaginacaoMesa.Count, paginacaoAnteriorMesa, paginacaoProximoMesa, paginaMesa);
        paginaMesa++;
        gameControler.ExibirGrupoPaginacaoTela(listaMesas, GameControler.DadosType.MESA, gruposPaginacaoMesa[paginaMesa - 1], 4);
    }

    public void AnteriorMesa()
    {
        gameControler.AnteriorPaginacao(gruposPaginacaoMesa.Count, paginacaoAnteriorMesa, paginacaoProximoMesa, paginaMesa);
        paginaMesa--;
        gameControler.ExibirGrupoPaginacaoTela(listaMesas, GameControler.DadosType.MESA, gruposPaginacaoMesa[paginaMesa - 1], 4);
    }
}
