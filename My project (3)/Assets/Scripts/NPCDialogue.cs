using UnityEngine;
using TMPro;

public class NPCDialogue : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel; // Panel de diálogo
    public TextMeshProUGUI dialogueText; // Texto donde se muestran las líneas del diálogo

    [Header("Tooltip")]
    public GameObject tooltipPanel;  // Panel de mensaje "Pulsa E"
    public TextMeshProUGUI tooltipText; // Texto del tooltip
    public string tooltipMessage = "interact_soldier_npc";
    // Mensaje traducido que aparece al acercarse

    [Header("Diálogo")]
    [TextArea(3, 5)]
    public string[] dialogueKeys;  // Array de líneas del diálogo a mostrar

    [System.NonSerialized]
    private string[] translatedLines;             

    [Header("Misión que activa este NPC")]
    public string missionToGiveId; // ID de misión que este NPC puede activar

    private int currentLine = 0; // Índice actual del diálogo
    private bool isPlayerInRange = false; // Si el jugador está dentro del área de interacción
    private bool isDialogueActive = false; // Si el diálogo está actualmente activo

    void Start()
    {
        tooltipMessage = LanguageManager.Instance.GetText(tooltipMessage);

        // Oculta paneles al comenzar
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }

    void Update()
    {
        // Si el jugador está cerca y presiona E
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isDialogueActive)
            {
                StartDialogue(); // Inicia el diálogo
            }
            else
            {
                NextLine(); // Muestra la siguiente línea
            }
        }
    }

    // Inicia el diálogo desde la primera línea
    void StartDialogue()
    {
        TranslateDialogue(); // Traducir antes de mostrar

        currentLine = 0;
        isDialogueActive = true;

        dialoguePanel.SetActive(true);
        dialogueText.text = translatedLines[currentLine];

        if (tooltipPanel != null)
            tooltipPanel.SetActive(false); // Oculta el mensaje de interacción
    }

    // Muestra la siguiente línea del diálogo o finaliza si se acabaron
    void NextLine()
    {
        currentLine++;

        if (currentLine < translatedLines.Length)
        {
            dialogueText.text = translatedLines[currentLine];
        }
        else
        {
            EndDialogue(); // Finaliza el diálogo
        }
    }

    // Finaliza el diálogo y activa una misión si está configurada
    void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);

        // Si tiene una misión que dar y no está ya activa/completada
        if (!string.IsNullOrEmpty(missionToGiveId))
        {
            Mission existing = MissionManager.Instance.GetMissionById(missionToGiveId);
            if (existing == null)
            {
                MissionManager.Instance.AddMissionById(missionToGiveId);
            }
        }

        Debug.Log("Diálogo terminado.");
    }

    // Detecta si el jugador entra al área del NPC
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;

            // Muestra el tooltip si no hay diálogo activo
            if (!isDialogueActive && tooltipPanel != null && tooltipText != null)
            {
                tooltipPanel.SetActive(true);
                tooltipText.text = tooltipMessage;
            }
        }
    }

    // Detecta si el jugador se aleja del NPC
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;

            if (tooltipPanel != null)
                tooltipPanel.SetActive(false);

            if (isDialogueActive)
                EndDialogue(); // Cierra el diálogo automáticamente si se aleja
        }
    }

    // Método para traducir el diálogo
    void TranslateDialogue()
    {
        translatedLines = new string[dialogueKeys.Length];

        for (int i = 0; i < dialogueKeys.Length; i++)
        {
            translatedLines[i] = LanguageManager.Instance.GetText(dialogueKeys[i]);
        }
    }

}
