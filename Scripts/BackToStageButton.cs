using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackToStageButton : MonoBehaviour
{


 

    public void OnBackButton()
    {
        // 指定されたシーンに切り替え
        SceneManager.LoadScene("StageSelect");
    }
}
