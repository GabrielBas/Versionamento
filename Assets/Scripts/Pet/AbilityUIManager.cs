//using UnityEngine;
//using UnityEngine.UI;

//public class AbilityUIManager : MonoBehaviour
//{
//    public static AbilityUIManager instance; // Instância singleton para fácil acesso

//    public GameObject abilityChoicePanel; // Painel de escolha de habilidade
//    public Button abilityButton1; // Botão para a primeira habilidade
//    public Button abilityButton2; // Botão para a segunda habilidade

//    private PetAbility[] currentAbilities; // Habilidades que serão exibidas na UI

//    private void Awake()
//    {
//        if (instance == null)
//        {
//            instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }

//        // Inicializa os botões
//        abilityButton1.onClick.AddListener(() => OnAbilityButtonClicked(0));
//        abilityButton2.onClick.AddListener(() => OnAbilityButtonClicked(1));

//        // Esconde o painel inicialmente
//        abilityChoicePanel.SetActive(false);
//    }

//    // Configura as habilidades para escolha na UI
//    public void ShowAbilityChoices(PetAbility[] abilities)
//    {
//        currentAbilities = abilities;
//        if (abilities.Length >= 2)
//        {
//            // Define os ícones e textos dos botões
//            abilityButton1.GetComponentInChildren<Text>().text = abilities[0].abilityName;
//            abilityButton1.GetComponent<Image>().sprite = abilities[0].icon;

//            abilityButton2.GetComponentInChildren<Text>().text = abilities[1].abilityName;
//            abilityButton2.GetComponent<Image>().sprite = abilities[1].icon;

//            // Exibe o painel de escolha de habilidade
//            abilityChoicePanel.SetActive(true);
//        }
//    }

//    // Quando um botão de habilidade é clicado
//    private void OnAbilityButtonClicked(int index)
//    {
//        if (currentAbilities != null && index < currentAbilities.Length)
//        {
//            PetAbilityManager.instance.ChooseAbility(currentAbilities[index]);
//        }

//        abilityChoicePanel.SetActive(false); // Esconde a UI após a escolha
//    }
//}
