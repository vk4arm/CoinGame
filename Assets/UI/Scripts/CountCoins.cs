using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountCoins : MonoBehaviour
{
    public GameObject coins;
    private Text cointText;

    void Start()
    {
        cointText = GetComponent<Text>();
        cointText.text = "/ " + coins.transform.childCount.ToString();
    }
}
