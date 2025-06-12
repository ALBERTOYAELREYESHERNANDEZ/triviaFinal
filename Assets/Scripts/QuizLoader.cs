using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class QuizLoader : MonoBehaviour
{
    public static System.Action OnQuizDataLoaded;
    public QuizData quizData;

    void Awake()
    {
        StartCoroutine(LoadQuizData());
    }

    IEnumerator LoadQuizData()
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "quizdata.json");

#if UNITY_ANDROID && !UNITY_EDITOR
        UnityWebRequest www = UnityWebRequest.Get(path);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            quizData = JsonUtility.FromJson<QuizData>(www.downloadHandler.text);
            OnQuizDataLoaded?.Invoke();
        }
        else
        {
            Debug.LogError("Error cargando quizdata.json desde Android StreamingAssets: " + www.error);
        }
#else
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            quizData = JsonUtility.FromJson<QuizData>(json);
            OnQuizDataLoaded?.Invoke();
        }
        else
        {
            Debug.LogError("No se encontró el archivo quizdata.json en: " + path);
        }
        yield return null;
#endif
    }
}
