using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackToStageButton : MonoBehaviour
{


 

    public void OnBackButton()
    {
        // �w�肳�ꂽ�V�[���ɐ؂�ւ�
        SceneManager.LoadScene("StageSelect");
    }
}
