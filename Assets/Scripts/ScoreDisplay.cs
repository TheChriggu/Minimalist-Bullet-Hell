using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Text textDisplay = GetComponent<Text>();
        if(textDisplay)
        {
            var score = GameObject.FindObjectOfType<Score>().gameObject;
            textDisplay.text = score.GetComponent<Score>().score.ToString();
            Destroy(score);
        }
    }
}
