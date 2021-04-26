using UnityEngine;
using System.Collections;

public class ParticleSystemDestroyer : MonoBehaviour {

    // ce script détruit le système de particules de pièces après 4 secondes
    void Start () {
		StartCoroutine (Destroy ());
	}
	
	IEnumerator Destroy(){
		yield return new WaitForSeconds (4);
		Destroy (gameObject);
	}
}
