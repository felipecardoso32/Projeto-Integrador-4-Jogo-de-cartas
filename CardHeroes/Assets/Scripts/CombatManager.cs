using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    private List<CharacterBattleState> exercitoJ1;
    private List<CharacterBattleState> exercitoJ2;

    private int jogadorAtivo = 1; // 1 ou 2
    private CharacterBattleState atacanteSelecionado;
    private CharacterBattleState alvoSelecionado;

    private int numeroTurno = 1;
    private int danoTotalJ1 = 0;
    private int danoTotalJ2 = 0;
    private int ataquesRealizados = 0;
    private int criticosRealizados = 0;
    private int esquivasRealizadas = 0;

    private bool turnoEmAndamento = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void IniciarPartida()
    {
        exercitoJ1 = PointDistributionManager.Instance.GetExercitoJ1();
        exercitoJ2 = PointDistributionManager.Instance.GetExercitoJ2();

        numeroTurno = 1;
        danoTotalJ1 = 0;
        danoTotalJ2 = 0;
        ataquesRealizados = 0;
        criticosRealizados = 0;
        esquivasRealizadas = 0;

        DefinirIniciativaInicial();
        BattleUIController.Instance.AtualizarTelaBatalha();
    }

    void DefinirIniciativaInicial()
    {
        // Define quem começa baseado na velocidade média ou aleatória
        CharacterBattleState p1 = exercitoJ1.Find(p => !p.IsDerrotado);
        CharacterBattleState p2 = exercitoJ2.Find(p => !p.IsDerrotado);

        if (p1 != null && p2 != null)
        {
            int totalVel = p1.velocidade + p2.velocidade;
            if (totalVel <= 0) jogadorAtivo = (Random.value < 0.5f) ? 1 : 2;
            else
            {
                bool p1Comeca = Random.Range(0, totalVel) < p1.velocidade;
                jogadorAtivo = p1Comeca ? 1 : 2;
            }
        }
    }

    public int GetJogadorAtivo() => jogadorAtivo;
    public int GetNumeroTurno() => numeroTurno;

    public void SelecionarAtacante(CharacterBattleState personagem)
    {
        if (turnoEmAndamento) return;
        if (personagem.IdJogador != jogadorAtivo || personagem.IsDerrotado) return;
        atacanteSelecionado = personagem;
        BattleUIController.Instance.AtualizarSelecaoVisual();
    }

    public void SelecionarAlvo(CharacterBattleState personagem)
    {
        if (turnoEmAndamento) return;
        if (atacanteSelecionado == null) return;
        if (personagem.IdJogador == jogadorAtivo || personagem.IsDerrotado) return;
        alvoSelecionado = personagem;
        BattleUIController.Instance.AtualizarSelecaoVisual();
    }

    public CharacterBattleState GetAtacanteSelecionado() => atacanteSelecionado;
    public CharacterBattleState GetAlvoSelecionado() => alvoSelecionado;

    public void ExecutarAtoDeAtaque()
    {
        if (atacanteSelecionado == null || alvoSelecionado == null || turnoEmAndamento) return;
        StartCoroutine(RotinaExecutarAtaque());
    }

    IEnumerator RotinaExecutarAtaque()
    {
        turnoEmAndamento = true;
        ataquesRealizados++;

        bool critico, esquiva;
        int dano = atacanteSelecionado.CalcularDanoTotal(alvoSelecionado, out critico, out esquiva);

        if (esquiva) esquivasRealizadas++;
        if (critico) criticosRealizados++;

        if (jogadorAtivo == 1) danoTotalJ1 += dano;
        else danoTotalJ2 += dano;

        if (!esquiva)
        {
            alvoSelecionado.ReceberDano(dano);
        }

        // Feedback Visual
        yield return(BattleUIController.Instance.AnimarAtaqueVisual(atacanteSelecionado, alvoSelecionado, dano, critico, esquiva));

        // Verificar Vitória
        if (VerificarFimDeJogo())
        {
            yield break;
        }

        // Limpar seleções e alternar turno
        atacanteSelecionado = null;
        alvoSelecionado = null;
        jogadorAtivo = (jogadorAtivo == 1) ? 2 : 1;
        numeroTurno++;

        turnoEmAndamento = false;
        BattleUIController.Instance.AtualizarTelaBatalha();
    }

    bool VerificarFimDeJogo()
    {
        bool p1Derrotados = exercitoJ1.TrueForAll(p => p.IsDerrotado);
        bool p2Derrotados = exercitoJ2.TrueForAll(p => p.IsDerrotado);

        if (p1Derrotados || p2Derrotados)
        {
            string vencedor = p1Derrotados ? "Jogador 2 Venceu!" : "Jogador 1 Venceu!";
            string stats = $"Turnos: {numeroTurno}\nDano Total J1: {danoTotalJ1} | Dano Total J2: {danoTotalJ2}\nAtaques: {ataquesRealizados} | Críticos: {criticosRealizados} | Esquivas: {esquivasRealizadas}";
            GameFlowController.Instance.MostrarResultado(vencedor, stats);
            return true;
        }
        return false;
    }

    public List<CharacterBattleState> GetExercitoJ1() => exercitoJ1;
    public List<CharacterBattleState> GetExercitoJ2() => exercitoJ2;
}