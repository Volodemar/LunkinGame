using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Управление уровнем Game
/// </summary>	
public class GameLevelController : LevelController
{
	public PathManager pathManager;
	public TrainManager trainManager;

	[BoxGroup("Настройки логистики"), SerializeField] private bool _isTrainСanSkipMine;
    public bool IsTrainСanSkipMine => _isTrainСanSkipMine;

	[BoxGroup("Настройки логистики"), SerializeField] private bool _isTrainСanSkipBase;
    public bool IsTrainСanSkipBase => _isTrainСanSkipBase;

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
