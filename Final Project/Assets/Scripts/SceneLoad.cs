using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SceneLoad : MonoBehaviour
{
    private string lastSavedScene = "M1Scene";
    private static SceneLoad _instance;
    [SerializeField]
    private GameObject fadeScreen;
    private string activeSceneName = "M1Scene";
    private Vector3 lastSavedPosition;
    public static SceneLoad Instance
    {
        get{return _instance;}
    }
    public string ActiveSceneName
    {
        get{return activeSceneName;}
    }
    public string LastSavedScene
    {
        get{return lastSavedScene;}
    }
    public Vector3 LastSavedPosition
    {
        get{return lastSavedPosition;}
    }
    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
    }
    public void GameOver()
    {
        StartCoroutine(gameOverScreen());
    }  
    IEnumerator gameOverScreen()
    {
        Fade fade = Instantiate(fadeScreen).GetComponent<Fade>();
        fade.setFadeTimer(3,3,1,1);
        fade.setText("You Have Died");
        StartCoroutine(fade.fadeIn());
        yield return new WaitForSecondsRealtime(3.0f);
        Player player = Player.Instance;
        player.updateHealth(player.MaxHealth);
        SceneManager.LoadSceneAsync(lastSavedScene);
        player.transform.position = lastSavedPosition;
        StartCoroutine(fade.fadeOut());

    }
    public void load(string sceneName)
    {
        activeSceneName = sceneName;
        SceneManager.LoadSceneAsync(sceneName);
    }
    public void load(string sceneName, string sceneObjectPosition, Vector3 offset)
    {
        activeSceneName = sceneName;
        SceneManager.LoadSceneAsync(sceneName);
        StartCoroutine(waitForSceneLoad(sceneName, sceneObjectPosition, offset));

    }
    IEnumerator waitForSceneLoad(string sceneName, string sceneObjectPosition, Vector3 offset)
    {
        //waits for scene to finish loading
        while (SceneManager.GetActiveScene().name != sceneName)
        {
            yield return null;
        }
        Transform sceneTransition = GameObject.FindGameObjectWithTag("SceneTransition").transform;
        //finds the scene object position to and sets the player's position relative to the offset
        for (int i = 0; i < sceneTransition.childCount; i++)
        {
            if (sceneTransition.GetChild(i).name == sceneObjectPosition)
            {
                Transform objTrans = sceneTransition.GetChild(i).transform;
                Vector3 newOffset = new Vector3(objTrans.localScale.x*offset.x, objTrans.localScale.y*offset.y,0);
                Player.Instance.transform.position = objTrans.position + newOffset;
                SceneTransition transitionObject;
                if (objTrans.TryGetComponent<SceneTransition>(out transitionObject))
                {
                    transitionObject.HasntEntered = false;
                }
            }
        }
        Time.timeScale = 1;
    }
    public void setSaveSpot(Vector3 position)
    {
        lastSavedPosition = position;
        print(SceneManager.GetActiveScene().name);
        lastSavedScene = SceneManager.GetActiveScene().name;
    }
}
