using UnityEngine;
using System.IO;

public static class CutscenePathBuilder
{
    public static string BuildStreamingAssetsPath(string relativePath)
    {
        return Path.Combine(Application.streamingAssetsPath, relativePath);
    }

    public static string BuildUrl(string relativePath)
    {
        string streamingPath = BuildStreamingAssetsPath(relativePath);

#if UNITY_ANDROID
        // Android는 StreamingAssets의 jar 경로를 VideoPlayer가 내부적으로 처리함
        // 절대 file:// 붙이지 말 것!
        return streamingPath;
#else
        // Editor, Windows, iOS는 file:// 붙여야 함
        if (!streamingPath.StartsWith("file://"))
            streamingPath = "file://" + streamingPath;

        return streamingPath;
#endif
    }
}