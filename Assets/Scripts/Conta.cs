using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class Conta : MonoBehaviour
{
    public Animator carregamento;
    public GameControler gameController;
    public GameObject telaErro;
    public GameObject telaLogin;
    public GameObject telaConta;
    public GameObject telaOpcoes;
    public GameObject telaCadastro;
    public Animator telas;
    public GameObject forcaTrocaSenha;
    public GameObject forcaSenha;
    private Usuario usuario;
    private UsuarioLimitado tempUsuario;
    private LoginResponse resLogin;
    private bool senhaEditar = false;
    private bool senhaEsqueceu = false;
    private bool campoEmail;
    private bool tipoCadastro;
    private bool validacaoEmail = false;
    private bool validacaoNick = false;
    private bool validacaoSenha = false;
    private bool temporizadorAtivo;
    private float delay;
    private float timer;

    private void Start()
    {
        tempUsuario = new UsuarioLimitado();
        temporizadorAtivo = false;
    }

    private void Update()
    {
        if (temporizadorAtivo)
        {
            timer -= Time.deltaTime;

            if(timer <= 0f)
            {
                timer = delay;
                temporizadorAtivo = false;
                if(campoEmail)
                {
                    StartCoroutine(VerificaEmailRequest());
                }
                else
                {
                    StartCoroutine(VerificaNickRequest());
                }
            }
        }
    }

    public void Deslogando()
    {
        StartCoroutine(DeslogarRequest());
    }

    public void Logando()
    {
        StartCoroutine(LoginRequest());
    }

    public void OnLoginValueChanged(string email)
    {
        tempUsuario.email = email;
    }

    public void OnSenhaLoginValueChanged(string senha)
    {
        tempUsuario.senha = senha;
    }

    public void ConfirmarNovoUsuario(TMP_InputField codInput)
    {
        StartCoroutine(VerificaCadastroRequest(codInput.GetComponentsInChildren<TextMeshProUGUI>()[1].text));
    }

    public void CadastrarNovoUsuario()
    {
        if(
            validacaoEmail &&
            validacaoNick &&
            validacaoSenha
          )
        {
            StartCoroutine(CadastrarRequest());
        }
        else
        {
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Edição Não Permitida: \n Corrigir campos em vermelho";
        }
    }

    public void CadastrarEdicao()
    {
        if(validacaoNick || string.IsNullOrEmpty(tempUsuario.nick))
        {
            StartCoroutine(UpdateRequest());
        }
        else
        {
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Edição Não Permitida: \n Corrigir campos em vermelho";
        }
    }

    public void OnUsuarioExistChanged(string novoUsuario)
    {
        tempUsuario.nick = novoUsuario;
        tipoCadastro = false;
        campoEmail = false;
        temporizadorAtivo = true;
        delay = 3f;
    }

    public void OnCadastroEmailExistChanged(string novoEmail)
    {
        tempUsuario.email = novoEmail;
        campoEmail = true;
        temporizadorAtivo = true;
        delay = 3f;
    }

    public void OnCadastroUsuarioExistChanged(string novoUsuario)
    {
        tempUsuario.nick = novoUsuario;
        tipoCadastro = true;
        campoEmail = false;
        temporizadorAtivo = true;
        delay = 3f;
    }

    public void OnTrocaSenhaValueChanged(string novaSenha)
    {
        tempUsuario.senha = novaSenha;
        int forca = gameController.MedindoForcaSenha(novaSenha);
        forcaTrocaSenha.GetComponentsInChildren<Slider>()[0].value = forca;
        forcaTrocaSenha.GetComponentsInChildren<TextMeshProUGUI>()[0].text = gameController.GetForcaText(forca);
        forcaTrocaSenha.GetComponentsInChildren<TextMeshProUGUI>()[0].color = gameController.GetColorForca(forca);
        validacaoSenha = forca >= 5 ? true : false;
    }

    public void OnSenhaValueChanged(string novaSenha)
    {
        tempUsuario.senha = novaSenha;
        int forca = gameController.MedindoForcaSenha(novaSenha);
        forcaSenha.GetComponentsInChildren<Slider>()[0].value = forca;
        forcaSenha.GetComponentsInChildren<TextMeshProUGUI>()[0].text = gameController.GetForcaText(forca);
        forcaSenha.GetComponentsInChildren<TextMeshProUGUI>()[0].color = gameController.GetColorForca(forca);
        validacaoSenha = forca >= 5 ? true : false;

    }

    public void OnNomeValueChanged(string novoNome)
    {
        tempUsuario.nome = novoNome;
    }

    public void LimparInputsEditing(GameObject formulario)
    {
        TMP_InputField[] listInput = formulario.GetComponentsInChildren<TMP_InputField>();
        foreach (TMP_InputField input in listInput)
        {
            input.text = string.Empty;
        }
        tempUsuario = new UsuarioLimitado();
    }

    public void EntrarConta()
    {
        string nick = PlayerPrefs.GetString("nick");
        string token = PlayerPrefs.GetString("token");
        if(token != null && nick != null)
        {
            Debug.Log("Carregando...");
            StartCoroutine(BuscarConta(nick, true));
        }
        else
        {
            telas.SetTrigger("Login");
        }
    }

    public void VerificaConexao()
    {
        StartCoroutine(Verificando());
    }

    private IEnumerator Verificando()
    {
        carregamento.SetTrigger("carregar");
        var getRequest = gameController.CreateRequest(
            gameController.UrlRota + "/usuarios/verifica-nick/PedrinCar",
            false,
            GameControler.RequestType.GET,
            null
            );
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            carregamento.SetTrigger("carregar");
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Erro ao Conectar";
            Debug.LogError(getRequest.error);
            Debug.Log(gameController.UrlRota + "/usuarios/verifica-nick/PedrinCar");
        }
        else
        {
            carregamento.SetTrigger("carregar");
            Debug.Log("conectado");
        }
    }

    public void TrocarSenhaEditar(TMP_InputField senha)
    {
        senhaEditar = true;
        tempUsuario.senha = senha.text;
        StartCoroutine(UpdateRequest());
    }

    public void TrocarSenhaEsqueceu(TMP_InputField senha)
    {
        senhaEsqueceu = true;
        tempUsuario.senha = senha.text;
        StartCoroutine(UpdateRequest());
    }

    public void BuscarEmailEsqueceuSenha(TMP_InputField email)
    {
        Debug.Log("Carregando...");
        StartCoroutine(EmailEsqueceuSenha(email.GetComponentsInChildren<TextMeshProUGUI>()[1].text));
    }

    public void CodigoRecuperacaoEsqueceuSenha(TMP_InputField codigo)
    {
        Debug.Log("Carregando...");
        StartCoroutine(CodigoRecuperacaoEsqueceuSenhaRequest(codigo.GetComponentsInChildren<TextMeshProUGUI>()[1].text));
    }

    private IEnumerator UpdateRequest()
    {
        carregamento.SetTrigger("carregar");
        tempUsuario.nome = !string.IsNullOrEmpty(tempUsuario.nome) ? tempUsuario.nome.Replace("\u200b", "") : usuario.nome;
        tempUsuario.email = !string.IsNullOrEmpty(tempUsuario.email) ? tempUsuario.email.Replace("\u200b", "") : usuario.email;
        tempUsuario.nick = !string.IsNullOrEmpty(tempUsuario.nick) ? tempUsuario.nick.Replace("\u200b", "") : usuario.nick;
        tempUsuario.senha = !string.IsNullOrEmpty(tempUsuario.senha) ? tempUsuario.senha.Replace("\u200b", "") : usuario.senha;
        Debug.Log("url: " + gameController.UrlRota + "/usuarios/" + usuario._id);
        var getRequest = gameController.CreateRequest(
            gameController.UrlRota + "/usuarios/" + usuario._id,
            true,
            GameControler.RequestType.PATCH,
            JsonUtility.ToJson(tempUsuario)
            );
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            carregamento.SetTrigger("carregar");
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Erro ao Cadastrar. \n Verifique sua conexão com a internet";
            Debug.LogError(getRequest.error);
        }
        else
        {
            carregamento.SetTrigger("carregar");
            if (senhaEditar)
            {
                telas.SetTrigger("Troca");
            }else if (senhaEsqueceu)
            {
                telas.SetTrigger("Sucess");
            }
            else
            {
                telaConta.GetComponentsInChildren<TMP_InputField>()[0].GetComponentsInChildren<TextMeshProUGUI>()[0].text = tempUsuario.nome;
                PlayerPrefs.SetString("nome", tempUsuario.nome.Replace("\u200b", ""));
                telaConta.GetComponentsInChildren<TMP_InputField>()[1].GetComponentsInChildren<TextMeshProUGUI>()[0].text = tempUsuario.email;
                PlayerPrefs.SetString("email", tempUsuario.email.Replace("\u200b", ""));
                telaConta.GetComponentsInChildren<TMP_InputField>()[2].GetComponentsInChildren<TextMeshProUGUI>()[0].text = tempUsuario.nick;
                PlayerPrefs.SetString("nick", tempUsuario.nick.Replace("\u200b", ""));
                PlayerPrefs.Save();
                LimparInputsEditing(telaConta);
                telas.SetTrigger("Editar");
            }
            senhaEditar = false;
            senhaEsqueceu = false;
        }
    }

    private IEnumerator DeslogarRequest()
    {
        carregamento.SetTrigger("carregar");
        var getRequest = gameController.CreateRequest(
            gameController.UrlRota + "/api/auth/logout",
            true,
            GameControler.RequestType.POST,
            null
            );
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            carregamento.SetTrigger("carregar");
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Erro ao Deslogar";
            Debug.LogError(getRequest.error);
        }
        else
        {
            carregamento.SetTrigger("carregar");
            usuario = new Usuario();
            telas.SetTrigger("Conta");
            PlayerPrefs.SetString("token", "");
            PlayerPrefs.SetString("nick", "");
            PlayerPrefs.Save();
        }
    }

    private IEnumerator LoginRequest()
    {
        carregamento.SetTrigger("carregar");
        tempUsuario.email.Replace("\u200b", "");
        tempUsuario.senha.Replace("\u200b", "");
        var getRequest = gameController.CreateRequest(
             gameController.UrlRota + "/api/auth/login",
             false,
             GameControler.RequestType.POST,
             "{\"email\": \"" + tempUsuario.email + "\", \"password\": \"" + tempUsuario.senha + "\"}"
             );
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            carregamento.SetTrigger("carregar");
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Erro ao Logar";
            Debug.LogError(getRequest.error);
            ErroRequest erro = JsonUtility.FromJson<ErroRequest>(getRequest.downloadHandler.text);
            Debug.Log("messagem: " + erro.message);
        }
        else
        {
            carregamento.SetTrigger("carregar");
            resLogin = JsonUtility.FromJson<LoginResponse>(getRequest.downloadHandler.text);
            if (!string.IsNullOrEmpty(resLogin.token))
            {
                telas.SetTrigger("Login");
                PlayerPrefs.SetString("token", resLogin.token);
                PlayerPrefs.SetString("nick", resLogin.nick);
                PlayerPrefs.Save();
            }
            else
            {
                telas.SetTrigger("SucessCadastro");
            }
        }
    }

    private IEnumerator CadastrarRequest()
    {
        carregamento.SetTrigger("carregar");
        tempUsuario.nome.Replace("\u200b", "");
        tempUsuario.email.Replace("\u200b", "");
        tempUsuario.nick.Replace("\u200b", "");
        tempUsuario.senha.Replace("\u200b", "");
        var getRequest = gameController.CreateRequest(
            gameController.UrlRota + "/usuarios/cadastro", 
            false, 
            GameControler.RequestType.POST, 
            JsonUtility.ToJson(tempUsuario)
            );
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            carregamento.SetTrigger("carregar");
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Erro ao Cadastrar. \n Verifique sua conexão com a internet";
            Debug.LogError(getRequest.error);
        }
        else
        {
            carregamento.SetTrigger("carregar");
            telas.SetTrigger("SucessCadastro");
        }
    }

    private IEnumerator VerificaCadastroRequest(string cod)
    {
        carregamento.SetTrigger("carregar");
        var getRequest = gameController.CreateRequest(
            gameController.UrlRota + "/usuarios/verifica-cadastro/" + cod.Replace("\u200b", ""),
            false,
            GameControler.RequestType.GET,
            null
            );
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            carregamento.SetTrigger("carregar");
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Codigo Invalido";
            Debug.LogError(getRequest.error);
        }
        else
        {
            carregamento.SetTrigger("carregar");
            telas.SetTrigger("SucessCadastro");
            PlayerPrefs.SetString("token", getRequest.downloadHandler.text);
            PlayerPrefs.SetString("nick", tempUsuario.nick.Replace("\u200b", ""));
            PlayerPrefs.Save();
        }
    }

    private IEnumerator VerificaNickRequest()
    {
        carregamento.SetTrigger("carregar");
        var getRequest = gameController.CreateRequest(gameController.UrlRota + "/usuarios/verifica-nick/" + tempUsuario.nick.Replace("\u200b", ""), false, GameControler.RequestType.GET, null);
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            carregamento.SetTrigger("carregar");
            validacaoNick = false;
            if (tipoCadastro)
            {
                telaCadastro.GetComponentsInChildren<TMP_InputField>()[1].GetComponentsInChildren<TextMeshProUGUI>()[1].color = gameController.HexToColor("#75EC51");
            }
            else
            {
                telaConta.GetComponentsInChildren<TMP_InputField>()[2].GetComponentsInChildren<TextMeshProUGUI>()[1].color = gameController.HexToColor("#FF4747");
            }
        }
        else
        {
            carregamento.SetTrigger("carregar");
            validacaoNick = true;
            if (tipoCadastro)
            {
                telaCadastro.GetComponentsInChildren<TMP_InputField>()[1].GetComponentsInChildren<TextMeshProUGUI>()[1].color = gameController.HexToColor("#75EC51");
            }
            else
            {
                telaConta.GetComponentsInChildren<TMP_InputField>()[2].GetComponentsInChildren<TextMeshProUGUI>()[1].color = gameController.HexToColor("#75EC51");
            }

        }
    }

    private IEnumerator VerificaEmailRequest()
    {
        carregamento.SetTrigger("carregar");
        var getRequest = gameController.CreateRequest(gameController.UrlRota + "/usuarios/verifica-email/" + tempUsuario.email.Replace("\u200b", ""), false, GameControler.RequestType.GET, null);
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            carregamento.SetTrigger("carregar");
            validacaoEmail = false;
            telaCadastro.GetComponentsInChildren<TMP_InputField>()[2].GetComponentsInChildren<TextMeshProUGUI>()[1].color = gameController.HexToColor("#FF4747");
        }
        else
        {
            carregamento.SetTrigger("carregar");
            validacaoEmail = true;
            telaCadastro.GetComponentsInChildren<TMP_InputField>()[2].GetComponentsInChildren<TextMeshProUGUI>()[1].color = gameController.HexToColor("#75EC51");
        }
    }

    private IEnumerator CodigoRecuperacaoEsqueceuSenhaRequest(string codigo)
    {
        carregamento.SetTrigger("carregar");
        var getRequest = gameController.CreateRequest(gameController.UrlRota + "/usuarios/esquece/" + codigo.Replace("\u200b", ""), false, GameControler.RequestType.GET, null);
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            carregamento.SetTrigger("carregar");
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Codigo Incorreto";
            Debug.LogError(getRequest.error);
        }
        else
        {
            carregamento.SetTrigger("carregar");
            telas.SetTrigger("Sucess");
            PlayerPrefs.SetString("token", getRequest.downloadHandler.text);
            PlayerPrefs.Save();
        }
    }

    private IEnumerator EmailEsqueceuSenha(string mail)
    {
        carregamento.SetTrigger("carregar");
        var getRequest = gameController.CreateRequest(this.gameController.UrlRota + "/usuarios/email/" + mail.Replace("\u200b", ""), false, GameControler.RequestType.GET, null);
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            carregamento.SetTrigger("carregar");
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Email Incorreto";
            Debug.LogError(getRequest.error.ToString());
        }
        else
        {
            carregamento.SetTrigger("carregar");
            telas.SetTrigger("Reset");
            PlayerPrefs.SetString("email", mail);
            PlayerPrefs.Save();
            usuario = new Usuario();
            usuario = JsonUtility.FromJson<Usuario>(getRequest.downloadHandler.text);
        }
    }

    private IEnumerator BuscarConta(string nick, bool conta)
    {
        carregamento.SetTrigger("carregar");
        var getRequest = gameController.CreateRequest(gameController.UrlRota + "/usuarios/" + nick, true, GameControler.RequestType.GET, null);
        yield return getRequest.SendWebRequest();
        if(getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            carregamento.SetTrigger("carregar");
            if (!conta)
            {
                telaErro.SetActive(true);
                telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Usuario não Encontrado";
                Debug.LogError(getRequest.error);
            }
            telas.SetTrigger("Login");
        }
        else
        {
            carregamento.SetTrigger("carregar");
            if (!string.IsNullOrEmpty(nick))
            {
                usuario = JsonUtility.FromJson<Usuario>(getRequest.downloadHandler.text);
                telas.SetTrigger("Conta");
                telaConta.GetComponentsInChildren<TMP_InputField>()[0].GetComponentsInChildren<TextMeshProUGUI>()[0].text = usuario.nome;
                PlayerPrefs.SetString("nome", usuario.nome);
                telaConta.GetComponentsInChildren<TMP_InputField>()[1].GetComponentsInChildren<TextMeshProUGUI>()[0].text = usuario.email;
                PlayerPrefs.SetString("email", usuario.email);
                telaConta.GetComponentsInChildren<TMP_InputField>()[2].GetComponentsInChildren<TextMeshProUGUI>()[0].text = usuario.nick;
                PlayerPrefs.SetString("nick", usuario.nick);
                PlayerPrefs.SetString("id", usuario._id);
                PlayerPrefs.Save();
            }
            else
            {
                telas.SetTrigger("Login");
                telaErro.SetActive(true);
                telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Usuario não Encontrado";
            }
        }
    }
}
