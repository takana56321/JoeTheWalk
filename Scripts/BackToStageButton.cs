using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackToStageButton : MonoBehaviour
{


 

    public void OnBackButton()
    {
        // w’è‚³‚ê‚½ƒV[ƒ“‚ÉØ‚è‘Ö‚¦
        SceneManager.LoadScene("StageSelect");
    }
}
