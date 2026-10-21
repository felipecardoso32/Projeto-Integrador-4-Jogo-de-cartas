using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PointDistributionManager : MonoBehaviour
{
    public static PointDistributionManager Instance { get; private set; }

    [Header("UI Elementos")]
    [SerializeField] private TMP_Text textoTituloConfig;
    [SerializeField] private TMP_Text textoPontosRestantes;
    [SerializeField] private TMP_Text textoForcaVal, textoVelVal, textoDestVal, textoIntelVal, textoRaivaVal;
    [SerializeField] private TMP_Text textoPreviaDano;
    [SerializeField] private Button botaoConfirmar;

    private List<CharacterBattleState> personagensJ1 = new List<CharacterBattleState>();
    private List<CharacterBattleState> personagensJ2 = new List<CharacterBattleState>();

    private int jogadorIndex = 1;
    private int personagemIndex = 0;

    private int f, v, d, i, r;
    private int pontosTotais = 20;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void IniciarDistribuicao()
    {
        personagensJ1.Clear();
        personagensJ2.Clear();

        foreach (var carta in CardSelectionManager.Instance.GetCartasJ1())
            personagensJ1.Add(new CharacterBattleState(carta, 1));

        foreach (var carta in CardSelectionManager.Instance.GetCartasJ2())
            personagensJ2.Add(new CharacterBattleState(carta, 2));

        jogadorIndex = 1;
        personagemIndex = 0;
        CarregarPersonagemAtual();
    }

    void CarregarPersonagemAtual()
    {
        var atual = GetPersonagemAtual();
        if (atual == null) return;

        f = 0; v = 0; d = 0; i = 0; r = 0;
        AtualizarUIValores();
    }

    CharacterBattleState GetPersonagemAtual()
    {
        List<CharacterBattleState> lista = (jogadorIndex == 1) ? personagensJ1 : personagensJ2;
        if (personagemIndex < lista.Count) return lista[personagemIndex];
        return null;
    }

    public void ModificarAtributo(string atributo, int valor)
    {
        int pontosUsados = f + v + d + i + r;
        int restante = pontosTotais - pontosUsados;

        if (valor > 0 && restante <= 0) return;

        switch (atributo)
        {
            case "forca":
                if (f + valor >= 0 && f + valor <= 10 && (f + valor + v + d + i + r <= pontosTotais)) f += valor;
                break;
            case "velocidade":
                if (v + valor >= 0 && v + valor <= 10 && (f + v + valor + d + i + r <= pontosTotais)) v += valor;
                break;
            case "destreza":
                if (d + valor >= 0 && d + valor <= 10 && (f + v + d + valor + i + r <= pontosTotais)) d += valor;
                break;
            case "intelecto":
                if (i + valor >= 0 && i + valor <= 10 && (f + v + d + i + valor + r <= pontosTotais)) i += valor;
                break;
            case "raiva":
                if (r + valor >= 0 && r + valor <= 10 && (f + v + d + i + r + valor <= pontosTotais)) r += valor;
                break;
        }
        AtualizarUIValores();
    }

    void AtualizarUIValores()
    {
        var atual = GetPersonagemAtual();
        int usados = f + v + d + i + r;
        int restantes = pontosTotais - usados;

        if (textoTituloConfig) textoTituloConfig.text = $"Jogador {jogadorIndex} — Configurar: {atual.cardData.nomePersonagem}";
        if (textoPontosRestantes) textoPontosRestantes.text = $"Pontos Restantes: {restantes}";

        if (textoForcaVal) textoForcaVal.text = f.ToString();
        if (textoVelVal) textoVelVal.text = v.ToString();
        if (textoDestVal) textoDestVal.text = d.ToString();
        if (textoIntelVal) textoIntelVal.text = i.ToString();
        if (textoRaivaVal) textoRaivaVal.text = r.ToString();

        int danoMin = atual.cardData.DanoMinimoBase + f;
        int danoMax = atual.cardData.DanoMaximoBase + f;
        if (textoPreviaDano) textoPreviaDano.text = $"Prévia Dano: {danoMin} - {danoMax} | Crítico: {d * 3}% | Esquiva: {i * 3}%";

        if (botaoConfirmar) botaoConfirmar.interactable = (restantes == 0);
    }

    public void ConfirmarAtributosPersonagem()
    {
        var atual = GetPersonagemAtual();
        atual.AplicarDistribuicao(f, v, d, i, r);

        personagemIndex++;
        if (personagemIndex >= 2)
        {
            if (jogadorIndex == 1)
            {
                GameFlowController.Instance.MostrarTelaHandoff("Passe o aparelho para o Jogador 2 configurar seus personagens.", () => {
                    jogadorIndex = 2;
                    personagemIndex = 0;
                    CarregarPersonagemAtual();
                    GameFlowController.Instance.IniciarDistribuicaoPontos();
                });
            }
            else
            {
                // Fim da distribuição, inicia Batalha (Tela 6)
                GameFlowController.Instance.MostrarTelaHandoff("Distribuição concluída! A batalha vai começar.", () => {
                    GameFlowController.Instance.IniciarBatalhaReal();
                });
            }
        }
        else
        {
            CarregarPersonagemAtual();
        }
    }

    public List<CharacterBattleState> GetExercitoJ1() => personagensJ1;
    public List<CharacterBattleState> GetExercitoJ2() => personagensJ2;
}