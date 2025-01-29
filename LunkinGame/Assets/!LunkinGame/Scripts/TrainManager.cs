using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Менеджер поездов
/// </summary>	
public class TrainManager : MonoBehaviour
{
    [SerializeField] private GameObject trainPrefab;

    private List<Train> _trains = new List<Train>();

	GameLevelController _gameLevel;

    public void Init()
    {
        _gameLevel = LevelController.Instance as GameLevelController;

        AddTrain();
    }

    private void AddTrain()
    {
        PathManager pathManager = _gameLevel.pathManager;

        // Создаем поезд
        BaseNode randomNode = pathManager.GetRandomNode();

		if (randomNode != null)
		{
			Train train = Instantiate(trainPrefab, randomNode.transform.position, Quaternion.identity, this.transform).GetComponent<Train>();

			train.Init(pathManager, randomNode);

            _trains.Add(train);
		}
    }
}
