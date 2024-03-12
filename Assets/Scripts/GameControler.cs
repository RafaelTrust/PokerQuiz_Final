using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Reflection;
using System.Globalization;

public class GameControler : MonoBehaviour
{
    public JogoSolitario jogoSolitario;
    public GameObject menu;
    public GameObject telaFichas;
    public GameObject telaErro;
    public string UrlRota = "http://localhost:3003";

    public AudioControler audioSource;
    private bool fecharJogo;
    private ListaUsuarios usuarios;


    // Start is called before the first frame update
    void Start()
    {
        FecharJogo = true;
    }

    public List<List<T>> DividirArray<T>(T[] array, int tamanhoDoGrupo)
    {
        List<List<T>> matriz = new List<List<T>>();

        for (int i = 0; i < array.Length; i += tamanhoDoGrupo)
        {
            List<T> grupo = new List<T>();

            for (int j = i; j < i + tamanhoDoGrupo && j < array.Length; j++)
            {
                grupo.Add(array[j]);
            }

            matriz.Add(grupo);
        }

        return matriz;
    }

    public void FecharTelaErro()
    {
        telaErro.SetActive(false);
    }

    public void IniciarJogoSolitario()
    {
        FecharJogo = false;
        jogoSolitario.RestartGame();
    }

    public bool FecharJogo 
    {
        get
        {
            return this.fecharJogo;
        }

        set
        {
            this.fecharJogo = value;
        }
    }

    public void SairGame()
    {
        Debug.Log("bool: " + FecharJogo);
        if (FecharJogo)
        {
            Application.Quit();
        }
        else
        {
            Debug.Log("estou aqui GameOver");
            menu.GetComponent<Animator>().SetTrigger("Fechar");
            telaFichas.GetComponent<Animator>().SetTrigger("Aparecer");
            FecharJogo = true;
            jogoSolitario.GameOver();
        }
    }

    public int MedindoForcaSenha(string senha)
    {
        int complexity = CalculaComplexidade(senha);

        return complexity;
    }

    private int CalculaComplexidade(string senha)
    {
        int complexity = 0;

        if (senha.Length >= 8) complexity++;
        if (ContainsUppercase(senha)) complexity += 3;
        if (ContainsLowercase(senha)) complexity++;
        if (ContainsDigit(senha)) complexity++;
        if (ContainsSpecialCharacter(senha)) complexity += 4;

        return complexity;
    }

    private bool ContainsUppercase(string input)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(input, "[A-Z]");
    }

    private bool ContainsLowercase(string input)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(input, "[a-z]");
    }

    private bool ContainsDigit(string input)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(input, "[0-9]");
    }

    private bool ContainsSpecialCharacter(string input)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(input, "[^a-zA-Z0-9]");
    }

    public string GetForcaText(int forca)
    {
        if (forca >= 8) return "Muito Forte";
        if (forca >= 6) return "Forte";
        if (forca >= 4) return "Média";
        if (forca >= 2) return "Fraca";
        return "Muito Fraca";
    }

    public Color GetColorForca(int forca)
    {
        if (forca >= 8) return HexToColor("#75EC51");
        if (forca >= 6) return HexToColor("#CAEC51");
        if (forca >= 4) return HexToColor("#ECE851");
        if (forca >= 2) return HexToColor("#EC7B52");
        return HexToColor("#FF4747");
    }

    public Color HexToColor(string hex)
    {
        Color color = new Color();
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }

    public UnityWebRequest CreateRequest(string path, bool token, GameControler.RequestType type, string jsonBody)
    {
        var request = new UnityWebRequest(path, type.ToString());

        if (jsonBody != null)
        {
            Debug.Log(jsonBody);
            var bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            Debug.Log(bodyRaw);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }

        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        if (token)
        {
            request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("token"));
        }

        return request;
    }

    public void ProximoPaginacao(int tamanhoGrupo, GameObject paginacaoAnterior, GameObject paginacaoProximo, int pagina)
    {
        if (tamanhoGrupo > pagina)
        {
            if (pagina == 1)
            {
                if (tamanhoGrupo == pagina + 1)
                {
                    paginacaoAnterior.SetActive(true);
                    paginacaoProximo.SetActive(false);
                }
                else
                {
                    paginacaoAnterior.SetActive(true);
                }
            }
        }
        else
        {
            paginacaoProximo.SetActive(false);
        }
    }

    public void AnteriorPaginacao(int tamanhoGrupo, GameObject paginacaoAnterior, GameObject paginacaoProximo, int pagina)
    {
        if (tamanhoGrupo < pagina)
        {
            if (pagina == 2)
            {
                paginacaoAnterior.SetActive(false);
            }
        }
        else if (pagina == 2)
        {
            paginacaoAnterior.SetActive(false);
            paginacaoProximo.SetActive(true);
        }
        else
        {
            paginacaoProximo.SetActive(true);
        }
    }

    public void ExibirGrupoPaginacaoTela<T>(GameObject[] listaObjTela, DadosType tipo, List<T> listaObj)
    {
        foreach (GameObject obj in listaObjTela)
        {
            obj.gameObject.SetActive(true);
        }
        Debug.Log("numero de itens na lista: " + listaObj.Count);
        Debug.Log("numero de itens na lista tela: " + listaObjTela.Length);
        if (listaObj.Count < 6)
        {
            for (int i = 0; i < listaObjTela.Length; i++)
            {
                if (listaObj.Count > i)
                {
                    atualizarDadosPaginacao(DadosType.PERGUNTA, listaObjTela[i], listaObj[i]);
                }
                else
                {
                    listaObjTela[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < listaObjTela.Length; i++)
            {
                atualizarDadosPaginacao(DadosType.PERGUNTA, listaObjTela[i], listaObj[i]);
            }
        }
    }

    private void atualizarDadosPaginacao(DadosType tipoDado, GameObject objetoListadoTela, object dado)
    {
        if (tipoDado == DadosType.MESA)
        {
            Mesa objDado = (Mesa)dado;
            objetoListadoTela.GetComponentsInChildren<TextMeshProUGUI>()[0].text = objDado.nome;
            objetoListadoTela.GetComponentsInChildren<TextMeshProUGUI>()[1].text = objDado.codSala;
        }
        else if (tipoDado == DadosType.PERGUNTA)
        {
            Debug.Log("dado que entrou: " + dado.ToString());
            PerguntaOnline objDado = (PerguntaOnline)dado;
            Debug.Log("dado dele: " + objDado.enun);
            objetoListadoTela.GetComponentsInChildren<TextMeshProUGUI>()[0].text = objDado.enun;
            objetoListadoTela.GetComponentsInChildren<TextMeshProUGUI>()[1].text = objDado._id;
        }
        else
        {
            Debug.Log("Tipo inexistente");
        }
    }

    public string RemoverAcentos(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        string normalizedString = input.Normalize(NormalizationForm.FormD);
        StringBuilder stringBuilder = new StringBuilder();

        foreach (char c in normalizedString)
        {
            UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if(unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString();
    }

    public enum DadosType
    {
        MESA = 0,
        PERGUNTA = 1,
    }

    public enum RequestType
    {
        GET = 0,
        POST = 1,
        PUT = 2,
        PATCH = 3,
        DELETE = 4
    }
}
