using System.Threading.Tasks;
using UnityEngine;

public class AccountLinkAskPopup : MonoBehaviour
{
    
    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
