using UnityEngine;


// Класс, реализующий сбор монет
public class PickUpCoins : MonoBehaviour
{
    public string coinType;
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            ChangeCoinTextUI.nCoins++;
            
            if (coinType == "Gold")
            {
                collider.gameObject.GetComponent<BasicBehaviour>().time += 5f;
            }
            else if (coinType == "Silver")
            {
                collider.gameObject.GetComponent<MoveBehaviour>().jumpHeight += 0.08f;
            }
            else if (coinType == "Copper")
            {
                if (collider.gameObject.GetComponent<MoveBehaviour>().runSpeed > 1f)
                {
                    collider.gameObject.GetComponent<MoveBehaviour>().runSpeed -= 0.03f;
                }
            }

            Destroy(gameObject);
        }
    }
}

