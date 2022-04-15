using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCoinTextUI : MonoBehaviour
{
    static public int nCoins = 0;
    private Text cointText;

    // Start is called before the first frame update
    void Start()
    {
        nCoins = 0;
        cointText = GetComponent<Text>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        cointText.text = nCoins.ToString();
    }
}
