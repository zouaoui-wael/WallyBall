using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class sound_script : MonoBehaviour {
    
    /// Ce script contrôle la musique et garde les modifications pour tous les stages
    
    private Music music;
    public Button MusicToggleButton;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    public AudioMixer mixer;

    // changer le volume
    public void SetLevel(float SliderValue)
    {
        mixer.SetFloat("MusicVol", Mathf.Log10(SliderValue) * 20);
     
    }


    // Au départ on cherche l'element qui contient le son et on fait appel à la fonction de mise à jour de l'image
    void Start () {
        music = GameObject.FindObjectOfType<Music>();
        UpdateMusicIcon();

    }
	// Fonction liée au boutton de de pause de son 
	public void PauseMusic()
    {
        music.ToggleSound();
        UpdateMusicIcon();
    }

    // Changer l'icone du boutton
    void UpdateMusicIcon()
    {
        if (PlayerPrefs.GetInt("Muted" ,0) ==0)
        {
            AudioListener.volume = 1;
            MusicToggleButton.GetComponent<Image>().sprite = musicOnSprite;
        }
        else
        {
            AudioListener.volume = 0;
            MusicToggleButton.GetComponent<Image>().sprite = musicOffSprite;
        }
    }

   
}
