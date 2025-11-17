using System.Threading.Tasks;
using UnityEngine;

public class AccountLinkAskPopup : MonoBehaviour
{
    [SerializeField] private GameObject dim;

    public void Open()
    {
        dim.SetActive(true);
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        dim.SetActive(false);
    }
}
