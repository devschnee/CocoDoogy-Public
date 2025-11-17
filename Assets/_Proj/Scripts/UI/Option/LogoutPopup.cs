using Firebase.Auth;
using UnityEngine;
using UnityEngine.SceneManagement;
using Google;

public class LogoutPopup : MonoBehaviour
{
    public void Open() => gameObject.SetActive(true);

    public void Close() => gameObject.SetActive(false);

    // 로그아웃 '확인' 시, 타이틀 화면으로
    public void LogoutAndGotoTitle()
    {
        // TODO : 실제 로그아웃 시키는 기능 필요.
        // FirebaseAuth.DefaultInstance.SignOut();
        // GoogleSignIn.DefualtInstacne.SignOut();
        gameObject.SetActive(false);
        SceneManager.LoadScene("Title");
    }
}
