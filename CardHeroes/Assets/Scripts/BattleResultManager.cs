using UnityEngine;
using TMPro;

public class BattleResultManager : MonoBehaviour
{
    public static BattleResultManager Instance { get; private set; }

    [SerializeField] private TMP_Text textoVencedorTitulo;
    [SerializeField] private TMP_Text textoEstatisticasDetalhes;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ExibirResultado(string vencedor, string estatisticas)
    {
        if (textoVencedorTitulo) textoVencedorTitulo.text = vencedor;
        if (textoEstatisticasDetalhes) textoEstatisticasDetalhes.text = estatisticas;
    }

    public void BotaoJogarNovamente()
    {
        GameFlowController.Instance.IniciarFluxoSelecao();
    }

    public void BotaoVoltarAoMenu()
    {
        GameFlowController.Instance.MostrarMenuInicial();
    }
}