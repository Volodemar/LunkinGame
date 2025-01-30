using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Менеджер поездов
/// </summary>	
public class TrainManager : MonoBehaviour
{
    [SerializeField] private GameObject[] trainPrefabs;

    [SerializeField] private GameObject testTrainPrefab;
    [SerializeField] private BaseNode testStartNode;

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

                _trains.Add(train);
            }
        }
        else
        {
			Train train = Instantiate(testTrainPrefab, testStartNode.transform.position, Quaternion.identity, this.transform).GetComponent<Train>();

			train.Init(pathManager, testStartNode);

            _trains.Add(train);
        }
    }
}
