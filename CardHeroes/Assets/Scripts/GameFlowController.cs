using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameFlowController : MonoBehaviour
{
    public static GameFlowController Instance { get; private set; }

    [Header("Paineis (Telas 1 a 7)")]
    [SerializeField] private GameObject painelMenuInicial;         // Tela 1
    [SerializeField] private GameObject painelSelecaoCartas;       // Tela 2
    [SerializeField] private GameObject painelDetalhesCarta;       // Tela 3
    [SerializeField] private GameObject painelPassagemJogador;     // Tela 4
    [SerializeField] private GameObject painelDistribuicaoPontos;  // Tela 5
    [SerializeField] private GameObject painelBatalha;             // Tela 6
    [SerializeField] private GameObject painelResultado;           // Tela 7

    [Header("Elementos Auxiliares")]
    [SerializeField] private TMP_Text textoPassagemHandoff;
    [SerializeField] private Button botaoContinuarHandoff;

    private System.Action acaoPosHandoff;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        MostrarMenuInicial();
    }

    public void MostrarMenuInicial()
    {
        OcultarTodosPaineis();
        AtivarPainel(painelMenuInicial, true);
    }

    public void IniciarFluxoSelecao()
    {
        OcultarTodosPaineis();
        AtivarPainel(painelSelecaoCartas, true);
        CardSelectionManager.Instance.IniciarSelecao();
    }

    public void MostrarDetalhesCarta(CharacterCardData carta, System.Action onConfirmar = null)
    {
        AtivarPainel(painelDetalhesCarta, true);
        // Opcional: Atualizar UI de detalhes aqui ou via CardSelectionManager
    }

    public void FecharDetalhesCarta()
    {
        AtivarPainel(painelDetalhesCarta, false);
    }

    public void MostrarTelaHandoff(string mensagem, System.Action aoContinuar)
    {
        OcultarTodosPaineis();
        AtivarPainel(painelPassagemJogador, true);
        if (textoPassagemHandoff) textoPassagemHandoff.text = mensagem;
        
        acaoPosHandoff = aoContinuar;
        botaoContinuarHandoff.onClick.RemoveAllListeners();
        botaoContinuarHandoff.onClick.AddListener(() => {
            acaoPosHandoff?.Invoke();
        });
    }

    public void IniciarDistribuicaoPontos()
    {
        OcultarTodosPaineis();
        AtivarPainel(painelDistribuicaoPontos, true);
        PointDistributionManager.Instance.IniciarDistribuicao();
    }

    public void IniciarBatalhaReal()
    {
        OcultarTodosPaineis();
        AtivarPainel(painelBatalha, true);
        CombatManager.Instance.IniciarPartida();
    }

    public void MostrarResultado(string textoVencedor, string estatisticas)
    {
        OcultarTodosPaineis();
        AtivarPainel(painelResultado, true);
        BattleResultManager.Instance.ExibirResultado(textoVencedor, estatisticas);
    }

    private void OcultarTodosPaineis()
    {
        AtivarPainel(painelMenuInicial, false);
        AtivarPainel(painelSelecaoCartas, false);
        AtivarPainel(painelDetalhesCarta, false);
        AtivarPainel(painelPassagemJogador, false);
        AtivarPainel(painelDistribuicaoPontos, false);
        AtivarPainel(painelBatalha, false);
        AtivarPainel(painelResultado, false);
    }

    private void AtivarPainel(GameObject painel, bool ativo)
    {
        if (painel == null) return;
        CanvasGroup cg = painel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = ativo ? 1f : 0f;
            cg.interactable = ativo;
            cg.blocksRaycasts = ativo;
        }
        else
        {
            painel.SetActive(ativo);
        }
    }
}