using UnityEngine;

[System.Serializable]
public class CharacterBattleState
{
    public CharacterCardData cardData;
    
    // Atributos distribuídos nesta partida (0 a 10 cada, soma total 20)
    public int forca;
    public int velocidade;
    public int destreza;
    public int intelecto;
    public int raiva;

    // Vida atual e máxima na partida
    public int vidaMaxima = 100;
    public int vidaAtual;
    
    public bool IsDerrotado => vidaAtual <= 0;
    public int IdJogador { get; private set; } // 1 ou 2

    public CharacterBattleState(CharacterCardData data, int idJogador)
    {
        cardData = data;
        IdJogador = idJogador;
        
        // Inicializa com os valores padrão do ScriptableObject ou zerados para distribuição
        forca = data.forca;
        velocidade = data.velocidade;
        destreza = data.destreza;
        intelecto = data.intelecto;
        raiva = data.raiva;

        vidaMaxima = data.VidaBase;
        vidaAtual = vidaMaxima;
    }

    public void AplicarDistribuicao(int f, int v, int d, int i, int r)
    {
        forca = f;
        velocidade = v;
        destreza = d;
        intelecto = i;
        raiva = r;
    }

    public int DanoMinimoEfetivo => cardData.DanoMinimoBase + forca;
    public int DanoMaximoEfetivo => cardData.DanoMaximoBase + forca;

    public int RolarDano()
    {
        return Random.Range(DanoMinimoEfetivo, DanoMaximoEfetivo + 1);
    }

    public float CalcularChanceEsquiva(CharacterBattleState atacante)
    {
        float chance = intelecto * cardData.EsquivaPorPonto;
        chance -= atacante.destreza * cardData.DestrezaPorPonto;
        return Mathf.Clamp(chance, 0f, 0.30f); // Teto de 30% recomendado
    }

    public bool Esquivou(CharacterBattleState atacante)
    {
        float chance = CalcularChanceEsquiva(atacante);
        return Random.value < chance;
    }

    public float CalcularChanceCritico()
    {
        return Mathf.Clamp01(destreza * cardData.CriticoPorPonto);
    }

    public bool CausouCritico()
    {
        return Random.value < CalcularChanceCritico();
    }

    public int CalcularDanoTotal(CharacterBattleState defensor, out bool foiCritico, out bool foiEsquiva)
    {
        foiCritico = false;
        foiEsquiva = defensor.Esquivou(this);

        if (foiEsquiva)
        {
            return 0;
        }

        int dano = RolarDano();

        foiCritico = CausouCritico();
        if (foiCritico)
        {
            dano = Mathf.RoundToInt(dano * cardData.MultiplicadorCritico);
        }

        // Aplica Raiva se a vida atual do atacante for estritamente menor que 30%
        float limiteRaiva = vidaMaxima * 0.30f;
        if (vidaAtual < limiteRaiva)
        {
            dano = Mathf.RoundToInt(dano * (1f + cardData.BonusRaiva));
        }

        return Mathf.Max(0, dano);
    }

    public void ReceberDano(int dano)
    {
        vidaAtual = Mathf.Max(0, vidaAtual - dano);
    }
}