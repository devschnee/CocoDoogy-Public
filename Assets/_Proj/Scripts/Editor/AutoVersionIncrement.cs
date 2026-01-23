using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Unity 빌드 전 처리 단계에서 실행되는 '자동 버전 증가' 유틸리티.
/// 빌드가 시작되면 PlayerSettings.bundleVersion을 [Major.Minor.Patch] 형식으로 파싱한 뒤 Patch를 증가시킴.
/// Patch/Minor 한계 도달 시 상위 버전을 자동으로 올림.
/// Editor 전용 스크립트로 런타임 코드와 분리됨.
/// </summary>
public class AutoVersionIncrement : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;
    private const int PatchLimit = 10;
    private const int MinorLimit = 10;

    public void OnPreprocessBuild(BuildReport report)
    {
        string version = PlayerSettings.bundleVersion;
        string[] parts = version.Split('.');

        if (parts.Length != 3)
        {
            Debug.LogError($"[AtuoVersionIncrement]Invalid version format: {version}");
            return;
        }

        int major = int.Parse(parts[0]);
        int minor = int.Parse(parts[1]);
        int patch = int.Parse(parts[2]);

        patch++;

        // Patch 버전을 기본 증가 단위로 사용
        // Patch 한계 도달 시 Minor 증가
        if (patch >= PatchLimit)
        {
            patch = 0;
            minor++;
        }

        // Minor 한계 도달 시 Major 증가
        if (minor >= MinorLimit)
        {
            minor = 0;
            major++;
        }

        // PlayerSettings에 계산된 새 버전 저장
        string newVersion = $"{major}.{minor}.{patch}";
        PlayerSettings.bundleVersion = newVersion;

        AssetDatabase.SaveAssets();
    }
}