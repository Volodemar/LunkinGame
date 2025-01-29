using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Менеджер игры (входная точка любой сцены)
/// </summary>	
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public string SceneName => SceneManager.GetActiveScene().name;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		LevelController.Instance?.OnLevelInit();
	}
}
