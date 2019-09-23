using UnityEngine;
using UnityEngine.SceneManagement;

//Used this youtube video for guidance on how to make buttons work: 
//https://www.youtube.com/watch?v=zc8ac_qUXQY&t=448s

public class MainMenu : MonoBehaviour{
    public void Play(){
        SceneManager.LoadScene("Main");
    }
    public void About(){
        SceneManager.LoadScene("About");
    }
    public void QuitTheGame(){
        Application.Quit();
    }
}