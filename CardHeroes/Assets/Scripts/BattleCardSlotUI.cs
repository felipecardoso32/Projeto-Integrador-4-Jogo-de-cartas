using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleCardSlotUI : MonoBehaviour
{
    [SerializeField] private Image imagemPersonagem;
    [SerializeField] private TMP_Text textoNome;
    [SerializeField] private TMP_Text textoVida;
    [SerializeField] private Slider barraVida;
    [SerializeField] private GameObject indicadorAtacante;
    [SerializeField] private GameObject indicadorAlvo;
    [SerializeField] private GameObject indicadorDerrotado;
    [SerializeField] private GameObject indicadorRaiva;

    private CharacterBattleState estadoPersonagem;
    private System.Action<CharacterBattleState> callbackClique;

    public void ConfigurarSlot(CharacterBattleState estado, System.Action<CharacterBattleState> aoClicar)
    {
        estadoPersonagem = estado;
        callbackClique = aoClicar;

        if (imagemPersonagem) imagemPersonagem.sprite = estado.cardData.imagem;
        if (textoNome) textoNome.text = estado.cardData.nomePersonagem;

        AtualizarDadosDinamicos();
    }

    void AtualizarDadosDinamicos()
    {
        if (estadoPersonagem == null) return;

        if (textoVida) textoVida.text = $"{estadoPersonagem.vidaAtual} / {estadoPersonagem.vidaMaxima}";
        if (barraVida) barraVida.value = (float)estadoPersonagem.vidaAtual / estadoPersonagem.vidaMaxima;

        if (indicadorDerrotado) indicadorDerrotado.SetActive(estadoPersonagem.IsDerrotado);

        // Ativa indicador de Raiva se vida < 30%
        float limiteRaiva = estadoPersonagem.vidaMaxima * 0.30f;
        if (indicadorRaiva) indicadorRaiva.SetActive(estadoPersonagem.vidaAtual < limiteRaiva && !estadoPersonagem.IsDerrotado);
    }

    public void AtualizarDestaque(CharacterBattleState atacante, CharacterBattleState alvo)
    {
        AtualizarDadosDinamicos();

        if (indicadorAtacante) indicadorAtacante.SetActive(atacante == estadoPersonagem);
        if (indicadorAlvo) indicadorAlvo.SetActive(alvo == estadoPersonagem);
    }

    public void OnClickSlotButton()
    {
        if (estadoPersonagem != null && !estadoPersonagem.IsDerrotado)
        {
            callbackClique?.Invoke(estadoPersonagem);
        }
    }
}