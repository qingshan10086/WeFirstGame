using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Door2_BOSS1 : MonoBehaviour
{
    

    public void LoadTargetScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    IEnumerator LoadSceneCoroutine(string sceneName)
    {
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);
        asyncOp.allowSceneActivation = false; // 暂停激活，等加载完成再显示

        while (!asyncOp.isDone)
        {
            // 计算加载进度（0.9为加载完成阈值，剩下0.1是激活场景）
            float progress = Mathf.Clamp01(asyncOp.progress / 0.9f);
           

            // 加载完成后激活场景
            if (asyncOp.progress >= 0.9f)
            {
                asyncOp.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
