using UnityEngine;

[CreateAssetMenu(
    fileName = "NovaCarta",
    menuName = "Combate/Carta de Personagem",
    order = 1
)]
public class CharacterCardData : ScriptableObject
{
    // ============================================================
    // CONFIGURAÇÕES FIXAS
    // ============================================================

    [Header("Informações da Carta")]
    public string nomePersonagem;

    [TextArea(2, 5)]
    public string descricao;

    public Sprite imagem;

    // ============================================================
    // VIDA E DANO BASE
    // ============================================================

    [Header("Valores Base")]

    [Tooltip("Vida inicial de todos os personagens.")]
    [SerializeField]
    private int vidaBase = 100;

    [Tooltip("Dano mínimo definido pela carta.")]
    [SerializeField]
    private int danoMinimoBase = 10;

    [Tooltip("Dano máximo definido pela carta.")]
    [SerializeField]
    private int danoMaximoBase = 20;

    // ============================================================
    // ATRIBUTOS
    // ============================================================

    [Header("Atributos")]

    [Range(0, 10)]
    public int forca;

    [Range(0, 10)]
    public int velocidade;

    [Range(0, 10)]
    public int destreza;

    [Range(0, 10)]
    public int intelecto;

    [Range(0, 10)]
    public int raiva;

    // ============================================================
    // REGRAS
    // ============================================================

    [Header("Regras")]

    [Tooltip("Quantidade máxima de pontos que podem ser distribuídos.")]
    [SerializeField]
    private int pontosTotais = 20;

    [Tooltip("Valor máximo permitido para cada atributo.")]
    [SerializeField]
    private int atributoMaximo = 10;

    [Tooltip("Bônus de dano quando a vida estiver abaixo de 30%.")]
    [SerializeField]
    private float bonusRaiva = 0.60f;

    [Tooltip("Quanto cada ponto de Intelecto aumenta a esquiva.")]
    [SerializeField]
    private float esquivaPorPonto = 0.03f;

    [Tooltip("Quanto cada ponto de Destreza reduz a esquiva do defensor.")]
    [SerializeField]
    private float destrezaPorPonto = 0.03f;

    [Tooltip("Quanto cada ponto de Destreza aumenta a chance de crítico.")]
    [SerializeField]
    private float criticoPorPonto = 0.03f;

    [Tooltip("Multiplicador de dano de um ataque crítico.")]
    [SerializeField]
    private float multiplicadorCritico = 1.5f;

    // ============================================================
    // PROPRIEDADES
    // ============================================================

    public int VidaBase => vidaBase;

    public int DanoMinimoBase => danoMinimoBase;

    public int DanoMaximoBase => danoMaximoBase;

    public int PontosTotais => pontosTotais;

    public int AtributoMaximo => atributoMaximo;

    public float BonusRaiva => bonusRaiva;

    public float EsquivaPorPonto => esquivaPorPonto;

    public float DestrezaPorPonto => destrezaPorPonto;

    public float CriticoPorPonto => criticoPorPonto;

    public float MultiplicadorCritico => multiplicadorCritico;

    // ============================================================
    // FORÇA
    // ============================================================

    /*
     * Cada ponto de Força aumenta o dano mínimo e máximo.
     *
     * Exemplo:
     *
     * Carta: 10 - 20
     * Força: 3
     *
     * Resultado:
     * 13 - 23
     *
     * Ou seja, cada ponto de Força adiciona +1
     * aos dois lados do intervalo.
     */

    public int DanoMinimoComForca
    {
        get
        {
            return danoMinimoBase + forca;
        }
    }

    public int DanoMaximoComForca
    {
        get
        {
            return danoMaximoBase + forca;
        }
    }

    // ============================================================
    // VALIDAÇÃO DOS ATRIBUTOS
    // ============================================================

    public int PontosDistribuidos
    {
        get
        {
            return forca
                   + velocidade
                   + destreza
                   + intelecto
                   + raiva;
        }
    }

    public int PontosRestantes
    {
        get
        {
            return pontosTotais - PontosDistribuidos;
        }
    }

    public bool DistribuicaoValida
    {
        get
        {
            return PontosDistribuidos <= pontosTotais
                   && forca <= atributoMaximo
                   && velocidade <= atributoMaximo
                   && destreza <= atributoMaximo
                   && intelecto <= atributoMaximo
                   && raiva <= atributoMaximo;
        }
    }

    // ============================================================
    // DANO ALEATÓRIO
    // ============================================================

    public int RolarDano()
    {
        int danoMin = DanoMinimoComForca;
        int danoMax = DanoMaximoComForca;

        return Random.Range(danoMin, danoMax + 1);
    }

    // ============================================================
    // VELOCIDADE
    // ============================================================

    /*
     * Compara a velocidade de dois personagens.
     *
     * Exemplo:
     *
     * Personagem 1 = 7
     * Personagem 2 = 3
     *
     * Total = 10
     *
     * Personagem 1:
     * 70% de chance de começar.
     *
     * Personagem 2:
     * 30% de chance.
     */

    public bool TemPrioridadeSobre(CharacterCardData adversario)
    {
        int total = velocidade + adversario.velocidade;

        if (total <= 0)
        {
            return Random.value < 0.5f;
        }

        return Random.Range(0, total) < velocidade;
    }

    // ============================================================
    // ESQUIVA
    // ============================================================

    /*
     * Intelecto do defensor aumenta sua esquiva.
     *
     * Exemplo:
     *
     * Intelecto = 5
     *
     * 5 * 0.03 = 0.15
     *
     * 15% de esquiva.
     *
     * A Destreza do atacante reduz essa chance.
     */

    public float CalcularChanceEsquiva(CharacterCardData atacante)
    {
        float chance = intelecto * esquivaPorPonto;

        chance -= atacante.destreza * destrezaPorPonto;

        // Nunca pode ser negativa.
        chance = Mathf.Max(0f, chance);

        // Nunca passa de 100%.
        chance = Mathf.Min(1f, chance);

        return chance;
    }

    public bool Esquivou(CharacterCardData atacante)
    {
        float chance = CalcularChanceEsquiva(atacante);

        return Random.value < chance;
    }

    // ============================================================
    // CRÍTICO
    // ============================================================

    /*
     * Destreza determina a chance de crítico.
     *
     * Exemplo:
     *
     * Destreza = 5
     *
     * 5 * 0.03 = 15%
     */

    public float CalcularChanceCritico()
    {
        float chance = destreza * criticoPorPonto;

        return Mathf.Clamp01(chance);
    }

    public bool CausouCritico()
    {
        return Random.value < CalcularChanceCritico();
    }

    // ============================================================
    // RAIVA
    // ============================================================

    /*
     * Raiva entra em ação quando a vida estiver abaixo de 30%.
     *
     * O bônus é de +60%.
     *
     * Exemplo:
     *
     * Dano = 20
     *
     * 20 * 1.60 = 32
     */

    public int AplicarRaiva(int dano, int vidaAtual)
    {
        float limiteRaiva = vidaBase * 0.30f;

        if (vidaAtual < limiteRaiva)
        {
            dano = Mathf.RoundToInt(dano * (1f + bonusRaiva));
        }

        return dano;
    }

    // ============================================================
    // DANO COMPLETO
    // ============================================================

    /*
     * Essa função reúne:
     *
     * 1. Sorteio do dano
     * 2. Crítico
     * 3. Raiva
     */

    public int CalcularDano(CharacterCardData defensor, int vidaAtual)
    {
        // Sorteia o dano base.
        int dano = RolarDano();

        // Verifica crítico.
        bool critico = CausouCritico();

        if (critico)
        {
            dano = Mathf.RoundToInt(
                dano * multiplicadorCritico
            );
        }

        // Aplica Raiva depois do crítico.
        dano = AplicarRaiva(dano, vidaAtual);

        // Garante que o dano nunca seja negativo.
        dano = Mathf.Max(0, dano);

        return dano;
    }

    // ============================================================
    // VALIDAÇÃO NO INSPECTOR
    // ============================================================

#if UNITY_EDITOR

    private void OnValidate()
    {
        // Impede valores negativos.

        vidaBase = Mathf.Max(1, vidaBase);

        danoMinimoBase = Mathf.Max(0, danoMinimoBase);

        danoMaximoBase = Mathf.Max(
            danoMinimoBase,
            danoMaximoBase
        );

        // Limita os atributos.

        forca = Mathf.Clamp(
            forca,
            0,
            atributoMaximo
        );

        velocidade = Mathf.Clamp(
            velocidade,
            0,
            atributoMaximo
        );

        destreza = Mathf.Clamp(
            destreza,
            0,
            atributoMaximo
        );

        intelecto = Mathf.Clamp(
            intelecto,
            0,
            atributoMaximo
        );

        raiva = Mathf.Clamp(
            raiva,
            0,
            atributoMaximo
        );

        // Impede ultrapassar os 20 pontos.
        //
        // A correção é feita automaticamente
        // caso você coloque pontos demais no Inspector.

        int excesso = PontosDistribuidos - pontosTotais;

        if (excesso > 0)
        {
            if (raiva > 0)
            {
                int reduzir = Mathf.Min(raiva, excesso);
                raiva -= reduzir;
                excesso -= reduzir;
            }

            if (intelecto > 0 && excesso > 0)
            {
                int reduzir = Mathf.Min(intelecto, excesso);
                intelecto -= reduzir;
                excesso -= reduzir;
            }

            if (destreza > 0 && excesso > 0)
            {
                int reduzir = Mathf.Min(destreza, excesso);
                destreza -= reduzir;
                excesso -= reduzir;
            }

            if (velocidade > 0 && excesso > 0)
            {
                int reduzir = Mathf.Min(velocidade, excesso);
                velocidade -= reduzir;
                excesso -= reduzir;
            }

            if (forca > 0 && excesso > 0)
            {
                int reduzir = Mathf.Min(forca, excesso);
                forca -= reduzir;
                excesso -= reduzir;
            }
        }
    }

#endif
}