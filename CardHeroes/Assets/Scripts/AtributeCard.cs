using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AtributeCard : MonoBehaviour
{
    [SerializeField] private CharacterCardData itemData;
    [SerializeField] private Image fotoPersonagem;
    [SerializeField] private TMP_Text textoNome;
    [SerializeField] private GameObject indicadorSelecao;
    [SerializeField] private GameObject indicadorEscurecido;

    private System.Action<CharacterCardData> aoClicarCarta;
    private bool selecionada = false;

    public CharacterCardData ItemData => itemData;

    public void ConfigurarCarta(CharacterCardData dados, System.Action<CharacterCardData> callbackClique)
    {
        itemData = dados;
        aoClicarCarta = callbackClique;

        if (itemData != null)
        {
            if (fotoPersonagem) fotoPersonagem.sprite = itemData.imagem;
            if (textoNome) textoNome.text = itemData.nomePersonagem;
        }

        AtualizarVisual();
    }

    public void DefinirSelecionada(bool estado)
    {
        selecionada = estado;
        AtualizarVisual();
    }

    public void DefinirEscurecida(bool escurecida)
    {
        if (indicadorEscurecido) indicadorEscurecido.SetActive(escurecida);
    }

    public void OnClickCard()
    {
        aoClicarCarta?.Invoke(itemData);
    }

    private void AtualizarVisual()
    {
        if (indicadorSelecao) indicadorSelecao.SetActive(selecionada);
    }
}