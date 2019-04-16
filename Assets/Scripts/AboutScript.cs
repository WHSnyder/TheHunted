using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Used this youtube video for guidance on how to make buttons work: 
//https://www.youtube.com/watch?v=zc8ac_qUXQY&t=448s

public class AboutScript : MonoBehaviour
{
    public void Back2()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
