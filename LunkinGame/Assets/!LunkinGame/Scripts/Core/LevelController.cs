using UnityEngine;

/// <summary>
/// Управление уровнем
/// </summary>	
public class LevelController : MonoBehaviour
{
    public static LevelController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

	/// <summary>
	/// Инициализируем уровень
	/// </summary>
	public virtual void OnLevelInit()
	{
	}
}
