using TMPro;
using UnityEngine;

/// <summary>
/// Application.version 값을 읽어 TextMeshProUGUI에 표시
/// AutoVersionIncrement(Edior 스크립트)와 연동되어 빌드 결과 버전을 런타임에서 확인 가능.
/// </summary>
public class VersionText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI versionTxt;

    void Awake()
    {
        versionTxt.text = $"V.{Application.version}";
    }
}