using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class MesasOnline : MonoBehaviour
{
    public GameControler gameController;
    public GameObject telaErro;

    public void CriarMesa()
    {
        StartCoroutine(CreateMesaRequest());
    }

    private IEnumerator CreateMesaRequest()
    {
        var getRequest = gameController.CreateRequest(
            gameController.UrlRota + "/salas/online",
            false,
            GameControler.RequestType.GET,
            null
            );
        yield return getRequest.SendWebRequest();
        if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Mesa não encontrada";
            Debug.LogError(getRequest.error);
        }
        else
        {
            telaErro.SetActive(true);
            telaErro.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Mesa encontrada";
            ///Start Game
        }
    }
}
