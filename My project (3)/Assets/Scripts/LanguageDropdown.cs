using TMPro;
using UnityEngine;

// Script que gestiona el cambio de idioma mediante un dropdown
public class LanguageDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown; 
    public Sprite flagES;
    public Sprite flagEN;
    private string selectedLang = "es"; // Idioma seleccionado

    void Start()
    {
        // Crear opciones con banderas
        var options = new System.Collections.Generic.List<TMP_Dropdown.OptionData>
        {
            new TMP_Dropdown.OptionData { image = flagES },
            new TMP_Dropdown.OptionData { image = flagEN }
        };

        dropdown.ClearOptions();
        dropdown.AddOptions(options);

        // Leer idioma actual desde LanguageManager
        string savedLang = LanguageManager.Instance.GetSelectedLanguage();
        dropdown.value = savedLang == "es" ? 0 : 1;
        selectedLang = savedLang;

        // Actualizar la imagen del dropdown (bandera)
        dropdown.captionImage.sprite = dropdown.options[dropdown.value].image;

        // Escucha cambios
        dropdown.onValueChanged.AddListener(index =>
        {
            selectedLang = (index == 0) ? "es" : "en";

            PlayerPrefs.SetString("language", selectedLang);
            PlayerPrefs.Save(); // Opcional pero recomendable

            dropdown.captionImage.sprite = dropdown.options[index].image;

            LanguageManager.Instance.LoadLanguage(selectedLang);

            MissionManager.Instance?.LoadMissionsFromSave(
                MissionManager.Instance.activeMissions,
                MissionManager.Instance.completedMissions
            );
            FindFirstObjectByType<MissionUI>()?.RefreshMissionUI();

            FindFirstObjectByType<LanguageUpdater>()?.UpdateAllTexts();
        });
    }



    // Método público para obtener el idioma actualmente seleccionado
    public string GetSelectedLanguage()
    {
        return selectedLang;
    }

    // Método para actualizar el dropdown desde código
    public void SetDropdownLanguage(string code)
    {
        if (code == "es")
        {
            dropdown.value = 0;
            selectedLang = "es";
            dropdown.captionImage.sprite = flagES;
        }
        else if (code == "en")
        {
            dropdown.value = 1;
            selectedLang = "en";
            dropdown.captionImage.sprite = flagEN;
        }
    }

}
