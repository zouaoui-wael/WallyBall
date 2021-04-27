using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ScoreManager : MonoBehaviour {

    //Ce script enregistre le dernier et le meilleur score utilisant PlayerPrefs
    // Calcule le temps joué chaque partie
    // Décide on est on quel niveau
    // génère le score maximal de chaque partie et change la vitesse et la couleur à chaque niveau
    // active et déactive les boutons de chaque niveau 

    int score;
    int highScore;
    // Variable contenant le temps que le joueur a joué pour sa partie
    float PlayedTime;
    public Text lastScoreT;
	public Text highScoreT;
    public Text TimePlayedT;
    public Text ScoreT;
    public Text LoseText;
    public Text MaxlvlT;

    public GameObject pannel_pub;
    public GameObject pannel_lose;
    public GameObject pannel_levels;

    public Button[] buttons;
    public Sprite[] OnButton;
    public Sprite OffButton;

    int Seconds;
    int Minutes = 0;
    int MaxScoreLevel =10 ;
    int LevelReached =1 ;
    bool next = false;

    void Awake()
    {
        PlayedTime = 1;
        //  
        if (PlayerPrefs.GetInt("LevelReached") != 0) { 
        LevelReached= PlayerPrefs.GetInt("LevelReached", LevelReached);
        }
        else
        {
            PlayerPrefs.SetInt("LevelReached", 1);
            //LevelReached = 1;

        }
        //PlayerPrefs.SetInt("LevelReached", 0);

        MaxScoreLevel = PlayerPrefs.GetInt("MaxScoreLevel",MaxScoreLevel);
        if (MaxScoreLevel == 0)
            MaxScoreLevel = 10;
        if (PlayerPrefs.GetInt("FromMenu") == 1)
        {
            pannel_levels.SetActive(true);
            PlayerPrefs.SetInt("FromMenu", 0);
        }
    }


    void Update ()
    {
        if (PlayerPrefs.GetInt("LastScore", 0) < 0)
        {
            lastScoreT.text = "DERNIER SCORE : 0 ";
        }
        else {
            lastScoreT.text = "DERNIER SCORE : " + PlayerPrefs.GetInt("LastScore", 0).ToString();
        }
        highScoreT.text = "MEILLEUR SCORE : " + PlayerPrefs.GetInt("HighScore", 0).ToString();
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        TimePlayedT.text = "vous avez jouer " + Minutes + " minutes et " + Seconds + " secondes";
      //  MaxlvlT.text = "But: " + PlayerPrefs.GetInt("MaxScoreLevel", 30).ToString();
        MaxlvlT.text = "But: " +MaxScoreLevel;
        UpdateScore();
        UpdateColor();
        UpdateMaxScore();
        CalculTime();
        UpdateButtonLevel();

    }
    void UpdateMaxScore()
    {
        if (score >= MaxScoreLevel)
        { if (next != true)
            {
                next = true;
                this.gameObject.GetComponent<PlayerScript>().stopmove = true;
                Invoke("Win",0f);

               if (LevelReached >= 9)
                {
                    pannel_lose.SetActive(true);
                    LoseText.text = "You win to be continued....";
                    this.gameObject.GetComponent<PlayerScript>().stopmove = true;
                }
            }
        }
    }
    void Win()
    {
        
        Debug.Log(PlayerPrefs.GetInt("currentlvl")+ " currentlvl"  );
        Debug.Log(PlayerPrefs.GetInt("LevelReached")+ "LevelReached");

        if (PlayerPrefs.GetInt("currentlvl") == PlayerPrefs.GetInt("LevelReached"))
        {

            PlayerPrefs.SetInt("LevelReached", LevelReached + 1);
            LevelReached += 1;

        }
        this.gameObject.GetComponent<PlayerScript>().stopmove = true;
        pannel_levels.SetActive(true);
    }


    void UpdateButtonLevel()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i +1 > LevelReached)
            {
                buttons[i].interactable = false;
                buttons[i].image.sprite = OffButton;
            }
            else
            {
                buttons[i].interactable = true;
                buttons[i].image.sprite = OnButton[i];
            }
        }
    }

    

    void UpdateScore()
    {
        if (score != GetComponent<PlayerScript>().score)
        {
            // Enregistrement du score actuel et le meilleur score 

            score = GetComponent<PlayerScript>().score;
            PlayerPrefs.SetInt("LastScore", score);

            if (score > highScore)
            {
                highScore = score;
                PlayerPrefs.SetInt("HighScore", highScore);
            }

        }
       
    }

    void UpdateColor()
    {
        // Changer la couleur du texte du score s'il est supérieur ou égale au meiller score
        if (PlayerPrefs.GetInt("LastScore", 0) >= PlayerPrefs.GetInt("HighScore", 0))
        {
            ScoreT.color = new Color32(198, 44, 78, 255);
        }
        else
        {
            ScoreT.color = new Color32(27, 1, 99, 255);
        }
    }


    public void CalculTime()
    {if (this.gameObject.GetComponent<PlayerScript>().stopmove == false)
        {
            PlayedTime += Time.deltaTime;
            Minutes = Mathf.RoundToInt(PlayedTime) / 60;
            Seconds = Mathf.RoundToInt(PlayedTime) % 60;
        }
    }

    // Fonction du boutton rejouer 
    public void revive()
    {
        pannel_lose.SetActive(false);
        pannel_pub.SetActive(true);
    }

    // Fonction du boutton quitter
    public void quit()
    {
        Application.Quit();
    }

    public void Home()
    {
        SceneManager.LoadScene("MainMenu");
    }


    public void lanchlvl1()
    {

        PlayerPrefs.SetInt("MaxScoreLevel", 30);
        PlayerPrefs.SetInt("currentlvl", 1);
        PlayerPrefs.SetInt("speed", 4);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        


    }
    public void lanchlvl2()
    {
        PlayerPrefs.SetInt("speed", 5);
        PlayerPrefs.SetInt("MaxScoreLevel", 50);
        PlayerPrefs.SetInt("currentlvl", 2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
      
    }
    public void lanchlvl3()
    {
        PlayerPrefs.SetInt("MaxScoreLevel", 70);
        PlayerPrefs.SetInt("currentlvl", 3);
        PlayerPrefs.SetInt("speed", 6);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void lanchlvl4()
    {
        PlayerPrefs.SetInt("MaxScoreLevel", 100);
        PlayerPrefs.SetInt("currentlvl", 3);
        PlayerPrefs.SetInt("speed", 7);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void lanchlvl5()
    {
        PlayerPrefs.SetInt("MaxScoreLevel", 130);
        PlayerPrefs.SetInt("currentlvl", 3);
        PlayerPrefs.SetInt("speed", 8);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void lanchlvl6()
    {
        PlayerPrefs.SetInt("MaxScoreLevel", 150);
        PlayerPrefs.SetInt("currentlvl", 3);
        PlayerPrefs.SetInt("speed", 9);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void lanchlvl7()
    {
        PlayerPrefs.SetInt("MaxScoreLevel", 180);
        PlayerPrefs.SetInt("currentlvl", 3);
        PlayerPrefs.SetInt("speed", 10);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void lanchlvl8()
    {
        PlayerPrefs.SetInt("MaxScoreLevel", 200);
        PlayerPrefs.SetInt("currentlvl", 3);
        PlayerPrefs.SetInt("speed", 11);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void lanchlvl9()
    {
        PlayerPrefs.SetInt("MaxScoreLevel", 230);
        PlayerPrefs.SetInt("currentlvl", 3);
        PlayerPrefs.SetInt("speed", 12);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    // Fonction liée au boutton de fermerture de publicité
    public void ClosePub()
    {
        pannel_pub.SetActive(false);
        Time.timeScale = 1;
        pannel_levels.SetActive(true);
    }

}
