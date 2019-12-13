using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TodoItem
{
    public long Id { get; set; }
    public string Name { get; set; }
    public bool IsComplete { get; set; }
}

public class HTTPClient : MonoSingleton<HTTPClient>
{
    public string gameID = "test";
    string uri = "https://dogfightazurewebapp20190904112015.azurewebsites.net/api/todo";

    private void Start()
    {
        //StartCoroutine(PostCrt());
        StartCoroutine(GetText());
    }

    IEnumerator GetText()
    {
        // 웹 서버에 요청을 생성한다.
        UnityWebRequest www = UnityWebRequest.Get(String.Format(uri, 1));
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
            string gameJSON = Encoding.Default.GetString(results);
            Debug.Log(gameJSON);
            //TodoItem item = JsonUtility.FromJson<TodoItem>(gameJSON);


            //Debug.Log(item.Id + ", " + item.Name + ", " + item.IsComplete);
            //yield return item;
        }
        
    }
    
    IEnumerator PostCrt()
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Post(uri, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Post Request Complete!");
            }
        }
    }


}
