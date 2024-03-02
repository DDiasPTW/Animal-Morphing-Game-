using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoBoard : MonoBehaviour
{
    private Canvas canvas;
    [SerializeField] private GameObject infoImage;
    public List<TMP_Text> timeTexts = new List<TMP_Text>();
    private GameManager gM;
    private Animator anim;
    private const string inAnimation = "Info_In";
    private const string outAnimation = "Info_Out";

    void Awake()
    {
        gM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        anim = GetComponentInChildren<Animator>();

        canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    void Start()
    {
        StartCoroutine(SetTimeText());
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            anim.Play(inAnimation);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //infoImage.SetActive(false);
            anim.Play(outAnimation);
        }
    }

    IEnumerator SetTimeText()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < timeTexts.Count; i++)
        {
            if(i == 0)
            {
                if(gM.currentPB != Mathf.Infinity){
                    timeTexts[0].text = gM.currentPB.ToString("F3") + "s";
                }
                else {
                    timeTexts[0].text = "N/A";
                }
                
            }
            else{
                timeTexts[i].text = gM.gradeTimes[i-1].ToString("F3") + "s";
            }
        }
    }
}
