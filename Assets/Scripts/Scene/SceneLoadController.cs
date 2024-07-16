using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneCheckMethod
{
    Distance,
    Trigger
}

public class SceneLoadController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private SceneCheckMethod checkMethod;
    [SerializeField] private float loadRange;

    private float enterTime;
    private bool isLoaded; // 2번 로드 방지
    private bool shouldLoad = false; // trigger메소드에서 사용

    void Awake()
    {
        if (SceneManager.sceneCount > 0)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == gameObject.name) isLoaded = true;
            }
        }
    }

    void Update()
    {
        if (player == null) player = PlayerManager.Instance.GetActivePlayer().transform;
        if (checkMethod == SceneCheckMethod.Distance && player != null) DistanceCheck();

        else if (checkMethod == SceneCheckMethod.Trigger) TriggerCheck();
    }

    void DistanceCheck()
    {
        if (Vector3.Distance(player.position, transform.position) < loadRange) LoadScene();
        else UnLoadScene();
    }

    private void TriggerCheck()
    {
        if (shouldLoad) LoadScene();
        else UnLoadScene();
    }

    private void LoadScene()
    {
        if (!isLoaded)
        {
            var scenes = SceneManager.GetSceneByName(gameObject.name);
            if (scenes.isLoaded) isLoaded = true;
            else
            {
                SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
                isLoaded = true;
            }
        }
    }

    private void UnLoadScene()
    {
        if (isLoaded)
        {
            SceneManager.UnloadSceneAsync(gameObject.name);
            isLoaded = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enterTime = Time.time;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!shouldLoad && Time.time > enterTime + 1f) shouldLoad = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shouldLoad = false;
        }
    }


}
