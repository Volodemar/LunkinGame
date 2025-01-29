using UnityEngine;

/// <summary>
/// Управление уровнем Game
/// </summary>	
public class GameLevelController : LevelController
{
	public PathManager pathManager;
	public TrainManager trainManager;

	/// <summary>
	/// Инициализируем уровень
	/// </summary>
	public override void OnLevelInit()
	{
		base.OnLevelInit();

		pathManager.Init();

		trainManager.Init();

	}
}
