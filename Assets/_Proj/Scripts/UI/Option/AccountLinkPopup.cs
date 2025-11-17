using System.Threading.Tasks;
using UnityEngine;
using Google;

public class AccountLinkPopup : MonoBehaviour
{
    [SerializeField] OptionPanelController controller;
    [SerializeField] private GameObject dim;

    // 구글 연동 시도. 해당 메서드는 실제 구현 시 변경될 수 있음.
    public async Task OnClick_LinkGoogle()
    {
        // TODO: Firebase / Google 연동 시도
        bool success = await TryGoogleLinkAsync();
        Debug.Log("Google 연동 시도");

        if (success)
        {
            controller.accountLinkSuccessPopup.SetActive(true);
            dim.SetActive(true);
        }
        if (!success)
        {
            controller.accountLinkFailPopup.SetActive(true);
            dim.SetActive(true);
        }
    }

    private async Task<bool> TryGoogleLinkAsync()
    {
        // TODO : 구글 연동
        return false;
    }
}
