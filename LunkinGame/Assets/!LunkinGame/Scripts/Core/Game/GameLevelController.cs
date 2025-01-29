using UnityEngine;

/// <summary>
/// Управление уровнем Game
/// </summary>	
public class GameLevelController : LevelController
{
	[SerializeField] private PathManager pathManager;

	/// <summary>
	/// Инициализируем уровень
	/// </summary>
	public override void OnLevelInit()
	{
		base.OnLevelInit();
	}
}
