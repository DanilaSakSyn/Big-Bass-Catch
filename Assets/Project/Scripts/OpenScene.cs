using UnityEngine;

namespace Project.Scripts
{
    public class OpenScene : MonoBehaviour
    {
        [SerializeField] private string sceneName;
        
        public void Open()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}