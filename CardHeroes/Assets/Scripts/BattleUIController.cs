using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUIController : MonoBehaviour
{
    public static BattleUIController Instance { get; private set; }

    [Header("UI Turno e Infos")]
    [SerializeField] private TMP_Text textoTurnoInfo;
    [SerializeField] private Button botaoAtacar;

    [Header("Slots Personagens J1 (2 slots)")]
    [SerializeField] private List<BattleCardSlotUI> slotsJ1;

    [Header("Slots Personagens J2 (2 slots)")]
    [SerializeField] private List<BattleCardSlotUI> slotsJ2;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AtualizarTelaBatalha()
    {
        var j1 = CombatManager.Instance.GetExercitoJ1();
        var j2 = CombatManager.Instance.GetExercitoJ2();
        int jAtivo = CombatManager.Instance.GetJogadorAtivo();

        if (textoTurnoInfo) textoTurnoInfo.text = $"Turno {CombatManager.Instance.GetNumeroTurno()} — Vez do Jogador {jAtivo}";

        for (int i = 0; i < slotsJ1.Count; i++)
        {
            if (i < j1.Count) slotsJ1[i].ConfigurarSlot(j1[i], OnClickSlot);
            else slotsJ1[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < slotsJ2.Count; i++)
        {
            if (i < j2.Count) slotsJ2[i].ConfigurarSlot(j2[i], OnClickSlot);
            else slotsJ2[i].gameObject.SetActive(false);
        }

        AtualizarSelecaoVisual();
    }

    void OnClickSlot(CharacterBattleState personagem)
    {
        int jAtivo = CombatManager.Instance.GetJogadorAtivo();
        if (personagem.IdJogador == jAtivo)
        {
            CombatManager.Instance.SelecionarAtacante(personagem);
        }
        else
        {
            CombatManager.Instance.SelecionarAlvo(personagem);
        }
    }

    public void AtualizarSelecaoVisual()
    {
        var atacante = CombatManager.Instance.GetAtacanteSelecionado();
        var alvo = CombatManager.Instance.GetAlvoSelecionado();

        foreach (var slot in slotsJ1) slot.AtualizarDestaque(atacante, alvo);
        foreach (var slot in slotsJ2) slot.AtualizarDestaque(atacante, alvo);

        if (botaoAtacar) botaoAtacar.interactable = (atacante != null && alvo != null);
    }

    public void OnClickBotaoAtacar()
    {
        CombatManager.Instance.ExecutarAtoDeAtaque();
    }

    public IEnumerator AnimarAtaqueVisual(CharacterBattleState atacante, CharacterBattleState alvo, int dano, bool critico, bool esquiva)
    {
        // Atualiza UI para refletir dano
        AtualizarTelaBatalha();
        yield return new WaitForSeconds(1.0f); // Pausa dramática para animação/feedback
    }
}