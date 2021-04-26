using UnityEngine;
using System.Collections;

public class ColorControl : MonoBehaviour {

    // Ce script change la couleur des murs,des bloqueurs et du ballon a chaque stage
    public Material tileMaterial;
	public Material blockM;
	public Material ballM;
    public Color[] colorList;// Liste de toutes les couleurs qui seront utilisées 
    public PlayerScript playerS;
    int ranNumber;	//Génération d'un nombre aliatoire
	int selectedNum; //Décision d'un nouveau nombre
	Color ranColor; 
	bool colorChanged; 


	void Start ()
    {
		colorChanged = true;

        // Nombre aléatoire entre 0 et la longueur de la liste des couleur
        ranNumber = Random.Range (0, colorList.Length); 

		ranColor = colorList [ranNumber];
		
		if (ranNumber < 6)
        {
			selectedNum = ranNumber + 6;
		}
        else
        {
			selectedNum = ranNumber - 6;
		}
		

    }
	
    public void changecolor()
    {
            // Changement du coleur


            if (ranNumber == 6)
            {
                blockM.color = Color.Lerp(blockM.color, colorList[11], Time.time * 0.0003f);
                ballM.color = Color.Lerp(ballM.color, colorList[1], Time.time * 0.0003f);
            }
            else if (ranNumber == 5)
            {
                blockM.color = Color.Lerp(blockM.color, colorList[10], Time.time * 0.0003f);
                ballM.color = Color.Lerp(ballM.color, colorList[0], Time.time * 0.0003f);
            }
            else
            {
                blockM.color = Color.Lerp(blockM.color, colorList[selectedNum - 1], Time.time * 0.0003f);
                ballM.color = Color.Lerp(ballM.color, colorList[selectedNum + 1], Time.time * 0.0003f);
            }
            tileMaterial.color = Color.Lerp(tileMaterial.color, ranColor, Time.time * 0.0003f);

            // Si la couleur de change pas
            if (!colorChanged)
            {
                colorChanged = true;

                // Lecture audio pour le changement de couleur

                GetComponent<AudioSource>().Play();

            }
        
    }
	void Update ()
    {

        changecolor();

    }


}
