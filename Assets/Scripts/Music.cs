
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Music : MonoBehaviour {
    
    // Ce script controle la musique et garde les modifications pour tous les stages
    
    static Music instance = null;

    private void Awake()
    {
     if(instance !=null)
        {
            Destroy(gameObject);
        }
     else
        {
            instance = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }
    }
    // Vérifie si le son est muet 
    public void ToggleSound()
    {
        if(PlayerPrefs.GetInt("Muted" ,0)==0)
        {
            PlayerPrefs.SetInt("Muted", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Muted", 0);
        }
    }
}
