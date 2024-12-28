using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public static class PlayFromSceneMenu
{
    private const string WHITE_SPACE = "     ";
    private const string PLAY_TITLE = "Play";
    private const string OPEN_TITLE = "Open";
    private static Vector2 EditorWindowCenter => EditorGUIUtility.GetMainWindowPosition().center;

    [MenuItem("Tools/Scene Switcher/Open Scene %SPACE")] // CTRL+SPACE
    private static void OpenSceneMenu()
    {
        if (Application.isPlaying)
        {
            Debug.LogWarning("Game is already running");
            return;
        }

        SearchEntriesProvider searchEntriesProvider = GetOpenProvider();
        var searchWindowContext = new SearchWindowContext(screenMousePosition: EditorWindowCenter);
        SearchWindow.Open(searchWindowContext, searchEntriesProvider);
    }

    [MenuItem("Tools/Scene Switcher/Play Scene %#SPACE")] // CTRL+SHIFT+SPACE
    private static void PlaySceneMenu()
    {
        if (Application.isPlaying)
        {
            Debug.LogWarning("Game is already running");
            return;
        }

        SearchEntriesProvider searchEntriesProvider = GetPlayProvider();
        var searchWindowContext = new SearchWindowContext(screenMousePosition: EditorWindowCenter);
        SearchWindow.Open(searchWindowContext, searchEntriesProvider);
    }

    private static SearchEntriesProvider GetPlayProvider()
    {
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

        SearchTreeEntry[] options = GetEntriesFor(buildScenes);

        var provider = ScriptableObject.CreateInstance<SearchEntriesProvider>();

        provider.Initialize(PLAY_TITLE, options, PlayScene);

        return provider;
    }

    private static SearchEntriesProvider GetOpenProvider()
    {
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

        SearchTreeEntry[] options = GetEntriesFor(buildScenes);

        var provider = ScriptableObject.CreateInstance<SearchEntriesProvider>();

        provider.Initialize(OPEN_TITLE, options, result => TryOpenScene(result));

        return provider;
    }

    private static void PlayScene(string scenePath)
    {
        if (!TryOpenScene(scenePath)) return;

        EditorApplication.isPlaying = true;
    }

    private static bool TryOpenScene(string scenePath)
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return false;

        if (SceneManager.GetActiveScene().path == scenePath)
            return true;

        EditorSceneManager.OpenScene(scenePath);
        return true;
    }

    private static SearchTreeEntry[] GetEntriesFor(EditorBuildSettingsScene[] buildScenes)
    {
        var entries = new SearchTreeEntry[buildScenes.Length];

        for (int i = 0; i < buildScenes.Length; i++)
        {
            entries[i] = new SearchTreeEntry(new GUIContent(WHITE_SPACE + GetSceneName(buildScenes[i].path)))
            {
                level = 1,
                userData = buildScenes[i].path
            };
        }

        return entries;
    }

    private static string GetSceneName(string scenePath)
    {
        return scenePath[(scenePath.LastIndexOf('/') + 1)..].Replace(".unity", "");
    }
}