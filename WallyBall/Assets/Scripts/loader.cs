using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class loader : MonoBehaviour
{
    // Ce script génère le menu principale 
    // La majorité des fonctions sont lié à des bouttons dans la scène
   
    public Slider slider; // Référence pour la glissière
    public GameObject SliderLoading;
    public Text progressText; // Référence pour le texte du chargement
    public GameObject TutoPannel;
    public GameObject OptionsPannel; 
    public Text RekiumT;
    public Image TutorielIm;
    public Sprite[] tutos;
    

    private void Start()
    {
        RekiumT.text = PlayerPrefs.GetInt("LastScore", 0).ToString();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //Rester tout les playerprefs (les sauvegardes)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("FromMenu", 1);
    }



    public void Loader(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex)); 
        // Chargement de la scène par son numéro
    }


    IEnumerator LoadAsynchronously (int sceneIndex)
     {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
       
        //Calculer le pourcentage de chargement de la scène et le lié avec la glissière
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / .9f);
            SliderLoading.SetActive(true);
                slider.value = progress;
                progressText.gameObject.SetActive(true);
                progressText.text = "CHARGEMENT "+ progress * 100f + "%";
                yield return null;

            }
     }


    private int counter = 1;        //Compteur initialisé à 0 pour signifier que Images[0] est en cours

    //Avancer l'image du tutoriel
    public void AdvanceImage()
    {
        if (counter < tutos.Length)
        {
            TutorielIm.sprite = tutos[counter];
            counter++;
        }
        else
        {
            counter = 0;
            TutoPannel.SetActive(false);
        }
             
    }
    //Réculer l'image du tutoriel
    public void BackImage()
    {
        if (counter > 0 && counter <= tutos.Length)
        {
            counter--;
            TutorielIm.sprite = tutos[counter];            
        }
        else
        {
            counter = 0;
            TutoPannel.SetActive(false);
        }
        
    }


    // Quitter l'application
    public void quit()
    {
        Application.Quit();
    }
    // Activer le tutoriel
    public void Tutoriel()
    {
        TutoPannel.SetActive(true);
    }
    // Activer le menu option
    public void Option()
    {
        OptionsPannel.SetActive(true);
    }
    // Charger le lien du rec'im
    public void Recim()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.recimapp.recim&hl=fr&gl=US"); 
    }

    // Déactiver le tutoriel
    public void TutorielClose()
    {
        TutoPannel.SetActive(false);
    }
    // Déactiver le menu option
    public void OptionClose()
    {
        OptionsPannel.SetActive(false);
    }
}
