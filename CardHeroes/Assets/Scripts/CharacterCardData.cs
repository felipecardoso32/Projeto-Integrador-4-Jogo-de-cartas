using UnityEngine;

[CreateAssetMenu(fileName = "NovaCarta", menuName = "Combate/Carta de Personagem", order = 1)]
public class CharacterCardData : ScriptableObject
{
    [Header("Informações da Carta")]
    public string nomePersonagem;
    [TextArea(2, 5)]
    public string descricao;
    public Sprite imagem;

    [Header("Valores Base")]
    [SerializeField] private int vidaBase = 100;
    [SerializeField] private int danoMinimoBase = 10;
    [SerializeField] private int danoMaximoBase = 20;

    [Header("Atributos Padrão (Asset)")]
    [Range(0, 10)] public int forca;
    [Range(0, 10)] public int velocidade;
    [Range(0, 10)] public int destreza;
    [Range(0, 10)] public int intelecto;
    [Range(0, 10)] public int raiva;

    [Header("Regras")]
    [SerializeField] private int pontosTotais = 20;
    [SerializeField] private int atributoMaximo = 10;
    [SerializeField] private float bonusRaiva = 0.60f;
    [SerializeField] private float esquivaPorPonto = 0.03f;
    [SerializeField] private float destrezaPorPonto = 0.03f;
    [SerializeField] private float criticoPorPonto = 0.03f;
    [SerializeField] private float multiplicadorCritico = 1.5f;

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

#if UNITY_EDITOR
    private void OnValidate()
    {
        vidaBase = Mathf.Max(1, vidaBase);
        danoMinimoBase = Mathf.Max(0, danoMinimoBase);
        danoMaximoBase = Mathf.Max(danoMinimoBase, danoMaximoBase);

        forca = Mathf.Clamp(forca, 0, atributoMaximo);
        velocidade = Mathf.Clamp(velocidade, 0, atributoMaximo);
        destreza = Mathf.Clamp(destreza, 0, atributoMaximo);
        intelecto = Mathf.Clamp(intelecto, 0, atributoMaximo);
        raiva = Mathf.Clamp(raiva, 0, atributoMaximo);
    }
#endif
}