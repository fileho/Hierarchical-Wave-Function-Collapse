using System;
using hwfc;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Guides the generation from the UI
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    private Button butGenerate;

    [SerializeField]
    private HierarchicalController hierarchicalController;

    public void GeneratePressed()
    {
        hierarchicalController.SetSeed(GetSeed());
        hierarchicalController.StartGenerating();
    }

    public void Menu()
    {
        SceneManager.LoadScene(0);
    }

    private int GetSeed()
    {
        Int32.TryParse(inputField.text, out int result);
        return result;
    }
}
