using System;
using hwfc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private InputField inputField;
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
