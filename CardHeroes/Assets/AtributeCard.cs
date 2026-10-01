using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using Systems;

public class AtributeCard : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   // public ScriptableObject dadosCarta;
    //public GameObject dadosDaCarta;
    [SerializeField] private CharacterCardData itemData; 
    public int forca;
    public int velocidade;
    public int intelecto;
    public int destreza;
    public GameObject fotoPersonagem;
    public TMP_Text vida;
    public TMP_Text descricao;
   // public int raiva;
    void Start()
    {
        //CharacterCardData dadosItem = ScriptableObject.CreateInstance<CharacterCardData>();
        fotoPersonagem.GetComponent<SpriteRenderer>().sprite =  itemData.imagem;
      //  vida = int.TryParse(dadosItem.vida);
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
