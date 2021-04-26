using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour {

    /// Ce script contrôle la plupart des fonctions du jeu
    /// Génère et ferme les murs
    /// Calcule le score
    /// Décide quand le joueur meurt


    public GameObject xTilePrefab;
	public GameObject yTilePrefab;
	public LayerMask whatIsGround;
	public GameObject ps;   // Système de particules pour les collectes
    public GameObject cam;  // Référence pour la caméra pricipale
    public float speed  ;	// Vitesse du ballon
    public bool onXTile;    // le ballon est sur quel mur
    public bool onYTile ;
	public bool generateTile;   

    public Text scoreText;	// Refrence pour UI 
    // Pour stocker les murs X et Y actives
    public List<GameObject> currentTileX;
	public List<GameObject> currentTileY;

    // Stockage des murs X et Y inactives
    public List<GameObject> recycledTileX ;
	public 	List<GameObject> recycledTileY ;

    // Liste de tous les éléments audio
    public AudioClip [] audios;

    Vector3 dir ;
	bool camMoved;
    // Variable contenant le score
	public int score ;
    // Variable contenant si le joueur a revivre ou non
	bool restart=false;
	bool gmStarted;
	float tileGenerateSpeed;
	List<GameObject> closeAnimX;
	List <GameObject> closeAnimY;
    public GameObject TTP;
    public GameObject pannel_lose;
    public bool stopmove=false;
    bool stop = true;


    void Awake()
    {
		CreateTiles (30);

        // Pour ne pas metter le téléphone en veille
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        speed = PlayerPrefs.GetInt("speed");
    }

    void Start()
    {
        gmStarted = false;
        CreateAtStart(10);
        generateTile = true;

        ///////////////////
        ///recim-revive///
        ///////////////////    

        closeAnimX = new List<GameObject>();
        closeAnimY = new List<GameObject>();

    }
   

    void FixedUpdate()
    {		
		if (currentTileY.Count != 0 && currentTileX.Count != 0 && IsGrounded()&& !camMoved)
        {
			CamFollow ();
		}
        if (stopmove == false)
        {
            transform.position = transform.position + (dir * speed * Time.deltaTime);
            transform.Rotate(-dir, 100);
        }
    }

    void Update ()
    {
        // Gérer le clic de souris et vérifier que le ballon est sur un mur ou pas
            OnClickMouse ();
			IsGrounded ();
        if(IsGrounded()==false)
        {
            stop = false;
        }

        //Si La boule n'est sur aucun mur ou le score est nul
        if (!IsGrounded ()  || score < 0  )
        {if(restart==false)
            {
                lose();
                PlayerDead();
                restart = true;
            }
            ///////////////
            ///recim-end///
            ///////////////


        }
   
    }
    
    public void lose()
    {
        pannel_lose.SetActive(true);
        stopmove = true;
    }

    public void PlayerDead()
    {
		//Gérer le son
		GetComponent<AudioSource> ().volume = 1;
		GetComponent<AudioSource> ().pitch = 1;
		GetComponent<AudioSource>().clip = audios[5];
		GetComponent<AudioSource>().Play();
    }


    /* Change la vitesse de ballon quand elle touche le bloqueur sur le mur
     réduit le score à chaque fois qu'il entre en collision avec le bloqueur */

    void OnTriggerEnter(Collider other)
    {
		if (other.CompareTag ("Block"))
        {
            // Si le ballon déclenche le bloqueur, elle change la direction et génère un certain nombre de nouvelles murs
                        dir = -dir;
			int randomTileNumber = Random.Range (5, 9);
			ActivateTiles (randomTileNumber);

			//Reduire le score par 1
			score--;

			if(score >= 0)
            {
			    scoreText.text  = "SCORE : " + score;
				GetComponent<AudioSource> ().volume = 1;
				GetComponent<AudioSource> ().pitch = 1;

				GetComponent<AudioSource>().clip = audios[1];
				GetComponent<AudioSource>().Play();
			}

            // Calculer le ballon est sur quel mur


        }
        else if (other.CompareTag ("XTile"))
        {
			onYTile = false;
			onXTile = true;
		}
        else if (other.CompareTag ("YTile"))
        {
			onXTile = false;
			onYTile = true;
		}
        else if (other.CompareTag ("Coin"))
        {

			score += 2;
			Instantiate(ps,other.transform.position,other.transform.rotation);
			other.gameObject.SetActive (false);
			scoreText.text = "SCORE : " + score;
			GetComponent<AudioSource> ().pitch = 1;

			GetComponent<AudioSource> ().volume = .5f;
			GetComponent<AudioSource>().clip = audios[3];
			GetComponent<AudioSource>().Play();
		}
	}


    // Gère le clic de la souris, augmente le score de 5 à chaque clic, change la direction de la boule au clic de la souris

    void OnClickMouse()
    {

		if (Input.GetMouseButtonDown (0) && IsGrounded() && score >= 0&&stop)
        {
           
			if (!gmStarted)
            {

                // Gestion du premier clic du jeu
                GetComponent<Rigidbody> ().isKinematic = false;		
                TTP.SetActive(false);
                gmStarted = true;

                /////////////////
                ///recim-start///
                //////////////////
                
                GetComponent<AudioSource> ().pitch = 1;
				GetComponent<AudioSource> ().volume = 1;
				GetComponent<AudioSource> ().clip = audios [0];
				GetComponent<AudioSource> ().Play ();
               
             }
            else
            {
				GetComponent<AudioSource> ().pitch = 1.35f;
				GetComponent<AudioSource> ().volume = 1;
				GetComponent<AudioSource>().clip = audios[1];
				GetComponent<AudioSource>().Play();
			}


            if (stopmove == false)
            {
                score += 5;
                scoreText.text = "SCORE : " + score;
                generateTile = true;
            }

            // Changer la direction du ballon à chaque clic

            if (dir == Vector3.right || dir == -Vector3.right)
            {
				dir = Vector3.forward;

            }
            else if (dir == Vector3.forward || -dir == Vector3.forward)
            {
				dir = -Vector3.right;
            }
            else {
				dir = -Vector3.right;
            }
		}
	}

    // Caméra hadels pour suivre le ballon en douceur

    void CamFollow()
    {
		GameObject tmp = null;
		if (onYTile && currentTileX.Count != 0)
        {
            // Si le ballon est sur la tuile Y, donnant une cible à la caméra à suivre
            tmp = currentTileX[currentTileX.Count -1];
		}
        else if (onXTile && currentTileY.Count != 0)
        {
			tmp = currentTileY[currentTileY.Count - 1];
		}
		Vector3 point = cam.GetComponent<Camera>().WorldToViewportPoint(tmp.transform.position);
		Vector3 delta = tmp.transform.position - cam.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); 
		Vector3 destination = cam.transform.position + delta;
		Vector3 velocity = Vector3.zero;
		cam.transform.position =  Vector3.SmoothDamp(cam.transform.position, destination,ref velocity, .35f);

	}
		
	private bool IsGrounded()
    {

        // Vérifier la collision de ballon avec le mur

        Collider [] colliders = Physics.OverlapSphere (this.transform.position, 1f, whatIsGround);
		for(int i = 0; i<colliders.Length; i++)
        {
			if(colliders[i].gameObject != gameObject)
            {
				return true;
			}
		}
		return false;
	}

		
	public void CreateTiles(int amount)
    {
        // Renommer toutes les nouveaux murs que nous allons générer
        for (int i = 0; i < amount; i++)
        {
			recycledTileX.Add (Instantiate (xTilePrefab));
            // Renommer le dernier élément du tableau
            recycledTileX [recycledTileX.Count - 1].name = ("xTile");
			recycledTileX [recycledTileX.Count - 1].SetActive (false);
			recycledTileY.Add (Instantiate (yTilePrefab));
			recycledTileY [recycledTileY.Count - 1].name = ("yTile");		
			recycledTileY [recycledTileY.Count - 1].SetActive (false);
		}
	}


	void CreateAtStart(int amount)
    {
        // Cette fonction crée une quantité donnée de murs au démarrage du jeu
        for (int i = 0; i < amount ; i++)
        {
			GameObject tmp = recycledTileX[recycledTileX.Count-1];
			recycledTileX.Remove(tmp );
			tmp.transform.position = currentTileX[currentTileX.Count - 1].transform.GetChild(0).transform.GetChild (1).transform.position;
			currentTileX .Add( tmp);
			int spawnPickUp = Random.Range (0, 10);

			if (spawnPickUp == 0)
            {
				currentTileX[currentTileX.Count - 1]. transform.GetChild(2).gameObject.SetActive(true);
			}

			if(i == amount -1)
            {
				tmp.transform.GetChild (1).gameObject.SetActive(true);
			}
		}
		tileGenerateSpeed = .3f;
		StartCoroutine(TileWakeUp());
	}

    /* Cette fonction génère de nouveaux murs lorsque le ballon est sur le mur X ou sur le mur Y
     lit les animations pour l'ouverture et la fermeture des murs
     manipule séparément, quand il est sur un mur X ou Y
     décide où garder le nouveau bloqueur sur les bout de mur*/

    public void ActivateTiles(int amount)
    {
        // Si le nombre de murs que nous avons généré n'est pas suffisant
        if (recycledTileX.Count < 15 || recycledTileY.Count < 15 )
        {
			CreateTiles(10);
		}

        // Lorsque le ballon est sur le mur Y et qu'elle entre en collision avec le bloqueur des murs Y pour la première fois
        if (onYTile && generateTile)
        {
            // Générer une nouvelle vitesse pour la boule
			if(currentTileX.Count != 0)
            {
				if(currentTileX.Count != 0)
                {
				for(int i = 0; i < currentTileX.Count; i++)
                    {

                        // Désactiver les murs précédentes (mur X car la balle est maintenant sur le mur Y)

                        if (currentTileX[currentTileX.Count - 1-i].transform.GetChild(1).gameObject.activeSelf){
						currentTileX[currentTileX.Count - 1-i].transform.GetChild(1).gameObject.SetActive(false);
					}

                        // Exécution de l'animation de fermeture de mur
                        closeAnimX.Add (currentTileX [currentTileX.Count - i - 1]);
				}

			}
				StartCoroutine (DeactivateXTile ());

                // Effacement de mur X actuel pour que nous puissions y conserver de nouvelles murs X
                currentTileX.Clear();
            }
            // Choisir un mur aléatoire pour activer le bloqueur sur le mur

            int randomBlockNumber = Random.Range(0, currentTileY.Count -3);
			int randomTileNumber = Random.Range(randomBlockNumber + 1, currentTileY.Count -1);

            // Mise en place d'un bloqueur aléatoire actif sur le mur
            currentTileY[randomBlockNumber].transform.GetChild (1).gameObject.SetActive (true);
			GameObject tmp = recycledTileX[recycledTileX.Count - 1];
			recycledTileX.Remove (tmp);

            // Définition d'une position aléatoire de mur Y sur une nouveau mur X

            tmp.transform.position = currentTileY[randomTileNumber].transform.GetChild (0).transform.GetChild (1).transform.position;

            // Ajout de ce mur X au mur actuel X
            currentTileX.Add(tmp);

			for(int i = 0; i < amount; i++)
            {
				tmp = recycledTileX[recycledTileX.Count - 1];
				recycledTileX.Remove (tmp);

                // Génération de nouveaux murs X à côté de la dernière position de mur
                tmp.transform.position = currentTileX[currentTileX.Count - 1].transform.GetChild (0).transform.GetChild (1).transform.position;		
				currentTileX.Add(tmp);
				
				if(i == amount-1)
                {
					tmp.transform.GetChild (1).gameObject.SetActive (true);
				}
				int spawnPickUp = Random.Range (0, 10);
                // définition d'une pièce active si le nombre aléatoire correspond à zéro pour chaque mur

                if (spawnPickUp == 0)
                {
					currentTileX[currentTileX.Count - 1]. transform.GetChild(2).gameObject.SetActive(true);
				}
			}

            // Exécution de l'animation d'ouverture de murs

            StartCoroutine(WakeUpWOnY());
			generateTile = false;
			camMoved = false;
		}


        // Comme ci-dessus, juste à la place du ballon de mur Y est sur la mur X

        if (onXTile && generateTile)
        {
            // Quand le ballon est sur le mur X et si elle entre en collision avec le bloqueur de mur Y pour la première fois

            if (currentTileY.Count != 0)
            {
                // Générer une nouvelle vitesse pour le ballon
               // ManageSpeed();

				for(int i = 0; i < currentTileY.Count; i++)
                {
					if(currentTileY.Count != 0)
                    {
					if(currentTileY[currentTileY.Count - 1-i].transform.GetChild(1).gameObject.activeSelf)
                        {
                            // Désactiver les murs précédentes (murs Y car le ballon est sur la mur X maintenant)
                            currentTileY[currentTileY.Count - 1-i].transform.GetChild(1).gameObject.SetActive(false);
				        }
				
						closeAnimY.Add (currentTileY [currentTileY.Count - i - 1]);
					}
				}
				StartCoroutine (DeactivateYTile ());

                // Effacement de mur actuel pour que nous puissions y conserver de nouveaux murs Y
                currentTileY.Clear();
			}

            // Choisir un mur aléatoire pour activer le bloqueur 
            int randomBlockNumber = Random.Range(0, currentTileX.Count -3);
			int randomTileNumber = Random.Range(randomBlockNumber +1 , currentTileX.Count -1);

            // Mise en place d'un bloqueur aléatoire actif sur le mur
            currentTileX[randomBlockNumber].transform.GetChild (1).gameObject.SetActive (true);
			GameObject tmp = recycledTileY[recycledTileY.Count - 1];
			recycledTileY.Remove (tmp);

            // Définition d'une position aléatoire de mur X sur un nouveau mur Y

            tmp.transform.position = currentTileX[randomTileNumber].transform.GetChild (0).transform.GetChild (0).transform.position;

            // Ajout de ce mur Y sur le mur Y actuel
            currentTileY.Add(tmp);

			for(int i = 0; i < amount; i++)
            {
				tmp = recycledTileY[recycledTileY.Count - 1];
				recycledTileY.Remove (tmp);
                // Génération de nouveau mur X à côté de la dernière position de mur
                tmp.transform.position = currentTileY[currentTileY.Count - 1].transform.GetChild (0).transform.GetChild (0).transform.position;			
				currentTileY.Add(tmp);

				if(i == amount-1)
                {
					tmp.transform.GetChild (1).gameObject.SetActive (true);
				}
				int spawnPickUp = Random.Range (0, 10);
				
				if (spawnPickUp == 0)
                {
					currentTileY[currentTileY.Count - 1]. transform.GetChild(2).gameObject.SetActive(true);
				}
			}
			tileGenerateSpeed = .3f;
			StartCoroutine (WakeUpWOnX());
			generateTile = false;
			camMoved = false;
		}
	}
	
	IEnumerator DeactivateXTile(){
		tileGenerateSpeed = .3f;

        // Fermeture de chaque mur en jouant l'animation fermeture mur

        if (closeAnimX.Count != 0) {
			foreach(GameObject tile in closeAnimX) {

				tile .GetComponent<Animator> ().SetTrigger ("CloseTile");

				if (tileGenerateSpeed > .15f) {
					tileGenerateSpeed -= .1f;
					yield return new WaitForSeconds (tileGenerateSpeed);

				} else{
					yield return new WaitForSeconds (tileGenerateSpeed);
				}
				recycledTileX.Add (tile);
			}


			closeAnimX.Clear ();
		}
	}
	IEnumerator DeactivateYTile()
    {
		tileGenerateSpeed = .3f;
		if (closeAnimY.Count != 0)
        {
            // Fermeture de chaque mur en jouant l'animation fermer mur

            foreach (GameObject tile in closeAnimY)
            {

				tile .GetComponent<Animator> ().SetTrigger ("CloseTile");

				if (tileGenerateSpeed > .15f)
                {
					tileGenerateSpeed -= .1f;
					yield return new WaitForSeconds (tileGenerateSpeed);

				}
                else
                {
					yield return new WaitForSeconds (tileGenerateSpeed);
				}
				recycledTileY.Add (tile);
			}
			closeAnimY.Clear ();
		}
	}

	IEnumerator WakeUpWOnX(){

        // lecture de l'animation OpenTile (ouvrir tuile / mur) pour le mur Y

        foreach (GameObject tile in currentTileY) {
			tile.SetActive (true);
				tile.GetComponent<Animator>().SetTrigger ("OpenTile");
	
		
		if (tileGenerateSpeed > .15f) {
			tileGenerateSpeed -= .1f;
			yield return new WaitForSeconds (tileGenerateSpeed);
		} else{
			yield return new WaitForSeconds (tileGenerateSpeed);
		}

		}
	}
	IEnumerator WakeUpWOnY(){

        // Lecture de l'animation ouvrir le mur X

        foreach (GameObject tile in currentTileX)
        {
			tile.SetActive (true);
			tile.GetComponent<Animator>().SetTrigger ("OpenTile");
		
			if (tileGenerateSpeed > .15f)
            {
				tileGenerateSpeed -= .1f;
				yield return new WaitForSeconds (tileGenerateSpeed);
			}
            else
            {
				yield return new WaitForSeconds (tileGenerateSpeed);
			}
			
		}
	}
	IEnumerator TileWakeUp()
    {

        // Gère l'animation d'ouverture des murs au démarrage du jeu

        foreach (GameObject tile in currentTileX)
        {
			tile.SetActive (true);		
			tile.GetComponent<Animator>().SetTrigger ("OpenTile");

			if (tileGenerateSpeed > .15f)
            {
				tileGenerateSpeed -= .1f;
				yield return new WaitForSeconds (tileGenerateSpeed);
			}
            else
            {
				yield return new WaitForSeconds (tileGenerateSpeed);
			}
            
		}
	}

   

 
	
}
