using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalSwapper : MonoBehaviour
{
    [SerializeField] private List<Animal> newAnimals = new List<Animal>();
    [SerializeField] private List<GameObject> newAnimalGO = new List<GameObject>();

    private bool canSwap = true;
    private Canvas canvas;
    [SerializeField] private Image[] infoImage;

    void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    void Start()
    {
        for (int i = 0; i < infoImage.Length; i++)
        {
            infoImage[i].sprite = newAnimals[i].animalSprite;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (canSwap)
        {
            if (other.CompareTag("Player"))
            {
                Player_Def playerScript = other.GetComponent<Player_Def>();
                int currentActive = playerScript.activeAnimalIndex;


                for (int i = 0; i < playerScript.animalGameObjects.Count; i++)
                {
                    playerScript.animalGameObjects[i].SetActive(false);
                }
                
                playerScript.forceSwapped = true;

                // Update animals and their GameObjects
                for (int i = 0; i < newAnimals.Count && i < newAnimalGO.Count; i++)
                {
                    if (i < playerScript.animals.Count)
                    {
                        playerScript.animals[i] = newAnimals[i];
                        playerScript.animalGameObjects[i] = newAnimalGO[i];
                    }
                }

                playerScript.SwitchActiveAnimal(currentActive);
            
                //update images
                for (int i = 0; i < playerScript.animals.Count && i < playerScript.animalSprites.Count; i++)
                {
                    playerScript.animalSprites[i].sprite = playerScript.animals[i].animalSprite;
                }
                canSwap = false;
                gameObject.SetActive(false);
            }
        }

    }
}

