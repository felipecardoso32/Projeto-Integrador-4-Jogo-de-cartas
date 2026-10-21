using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardSelectionManager : MonoBehaviour
{
    public static CardSelectionManager Instance { get; private set; }

    [Header("Configurações")]
    [SerializeField] private List<CharacterCardData> todasAsCartasDisponiveis = new List<CharacterCardData>();
    [SerializeField] private Transform containerGridCartas;
    [SerializeField] private GameObject prefabCardUI;
    [SerializeField] private TMP_Text textoInstrucaoJogador;
    [SerializeField] private Button botaoConfirmarSelecao;

    // Painel de Detalhes (Tela 3)
    [SerializeField] private GameObject painelDetalhes;
    [SerializeField] private Image imagemDetalhe;
    [SerializeField] private TMP_Text textoNomeDetalhe;
    [SerializeField] private TMP_Text textoDescricaoDetalhe;
    [SerializeField] private TMP_Text textoAtributosDetalhe;

    private int jogadorAtual = 1;
    private List<CharacterCardData> cartasJogador1 = new List<CharacterCardData>();
    private List<CharacterCardData> cartasJogador2 = new List<CharacterCardData>();
    
    private CharacterCardData cartaSelecionadaAtual;
    private List<AtributeCard> instanciasCartasUI = new List<AtributeCard>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void IniciarSelecao()
    {
        jogadorAtual = 1;
        cartasJogador1.Clear();
        cartasJogador2.Clear();
        MontarGrid();
        AtualizarInterfaceSelecao();
    }

    void MontarGrid()
    {
        foreach (Transform child in containerGridCartas) Destroy(child.gameObject);
        instanciasCartasUI.Clear();

        foreach (var carta in todasAsCartasDisponiveis)
        {
            GameObject obj = Instantiate(prefabCardUI, containerGridCartas);
            AtributeCard cardUI = obj.GetComponent<AtributeCard>();
            cardUI.ConfigurarCarta(carta, OnCardClicked);
            instanciasCartasUI.Add(cardUI);
        }
    }

    void OnCardClicked(CharacterCardData carta)
    {
        cartaSelecionadaAtual = carta;
        ExibirDetalhes(carta);
    }

    void ExibirDetalhes(CharacterCardData carta)
    {
        if (painelDetalhes) painelDetalhes.SetActive(true);
        if (imagemDetalhe) imagemDetalhe.sprite = carta.imagem;
        if (textoNomeDetalhe) textoNomeDetalhe.text = carta.nomePersonagem;
        if (textoDescricaoDetalhe) textoDescricaoDetalhe.text = carta.descricao;
        if (textoAtributosDetalhe) textoAtributosDetalhe.text = $"Vida Base: {carta.VidaBase}\nDano: {carta.DanoMinimoBase}-{carta.DanoMaximoBase}";
    }

    public void ConfirmarEscolhaDetalhes()
    {
        if (cartaSelecionadaAtual == null) return;

        List<CharacterCardData> listaAtual = (jogadorAtual == 1) ? cartasJogador1 : cartasJogador2;

        if (listaAtual.Contains(cartaSelecionadaAtual)) return; // Já escolhida

        listaAtual.Add(cartaSelecionadaAtual);
        if (painelDetalhes) painelDetalhes.SetActive(false);

        ActualizarVisualCartasEscolha();

        if (listaAtual.Count >= 2)
        {
            if (jogadorAtual == 1)
            {
                // Transição para Jogador 2
                GameFlowController.Instance.MostrarTelaHandoff("Passe o aparelho para o Jogador 2.", () => {
                    jogadorAtual = 2;
                    ActualizarVisualCartasEscolha();
                    AtualizarInterfaceSelecao();
                    GameFlowController.Instance.IniciarFluxoSelecao(); // Retorna à seleção para o J2
                });
            }
            else
            {
                // Fim da seleção, ir para Distribuição de Pontos (Tela 5)
                GameFlowController.Instance.MostrarTelaHandoff("Passando para a Distribuição de Pontos do Jogador 1.", () => {
                    GameFlowController.Instance.IniciarDistribuicaoPontos(); // Chamada segura
                });
            }
        }
    }

    void ActualizarVisualCartasEscolha()
    {
        List<CharacterCardData> listaAtual = (jogadorAtual == 1) ? cartasJogador1 : cartasJogador2;
        foreach (var cardUI in instanciasCartasUI)
        {
            bool escolhidaP1 = cartasJogador1.Contains(cardUI.ItemData);
            bool escolhidaP2 = cartasJogador2.Contains(cardUI.ItemData);

            cardUI.DefinirEscurecida(escolhidaP1 || escolhidaP2);
            cardUI.DefinirSelecionada(listaAtual.Contains(cardUI.ItemData));
        }
    }

    void AtualizarInterfaceSelecao()
    {
        if (textoInstrucaoJogador)
            textoInstrucaoJogador.text = $"Jogador {jogadorAtual} — Selecione 2 cartas";
    }

    public List<CharacterCardData> GetCartasJ1() => cartasJogador1;
    public List<CharacterCardData> GetCartasJ2() => cartasJogador2;
}