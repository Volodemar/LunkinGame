using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

/// <summary>
/// Менеджер поездов
/// </summary>	
public class TrainManager : MonoBehaviour
{
    [SerializeField] private GameObject[] trainPrefabs;

    [BoxGroup("Тест поезда"), SerializeField] private GameObject testTrainPrefab;
    [BoxGroup("Тест поезда"), SerializeField] private BaseNode testStartNode;

    // Замедление/ускорение движения поездов для лучшего наблюдения
    [BoxGroup("Настройки поездов"), SerializeField] private float speedFactor = 0.1f;

    private List<Train> _trains = new List<Train>();

	GameLevelController _gameLevel;

    public void Init()
    {
        _gameLevel = LevelController.Instance as GameLevelController;

        AddTrains();
    }

    private void AddTrains()
    {
        PathManager pathManager = _gameLevel.pathManager;

        if (testStartNode == null && testTrainPrefab != null)
        {
            foreach(GameObject trainPrefab in trainPrefabs)
            {
                BaseNode randomNode = pathManager.GetRandomNode();

			    Train train = Instantiate(trainPrefab, randomNode.transform.position, Quaternion.identity, this.transform).GetComponent<Train>();

			    train.Init(pathManager, randomNode);
                train.SetSpeedFactor(speedFactor);

                _trains.Add(train);
            }
        }
        else
        {
			Train train = Instantiate(testTrainPrefab, testStartNode.transform.position, Quaternion.identity, this.transform).GetComponent<Train>();

			train.Init(pathManager, testStartNode);
            train.SetSpeedFactor(speedFactor);

            _trains.Add(train);
        }
    }

    [Button("Обновить скорость поездов")]
    private void UpdateSpeedFactor()
    {
        foreach(Train train in _trains)
        {
            train.SetSpeedFactor(speedFactor);
        }
    }
}
