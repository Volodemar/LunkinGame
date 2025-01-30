using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Управление поездом
/// </summary>
public class Train : MonoBehaviour
{
    public float speed = 200f; 
    public float speedMine = 20f; 

    [SerializeField] private Color _pathColor;

    private PathManager _pathManager;
    private BaseNode _currentNode;
    private BaseNode _targetNode;
    private List<BaseNode> _path;

    private float _progress = 0f;
    private int _currentWaypoint = 0;
    private float _countResource = 0;

    private bool _isManing = false;

    // Коэфициент скорости поездов, не влияет на расчеты путей
    private float _speedFactor = 0.1f;

	GameLevelController _gameLevel;

    public void Init(PathManager pathManager, BaseNode startNode)
    {
        _gameLevel = LevelController.Instance as GameLevelController;

        _pathManager = pathManager;

        _currentNode = startNode;

        // Выбираем лучшую шахту
        _targetNode = pathManager.GetBestMineNode(_currentNode, speed, speedMine);

        // Стартанули в шахте, значит майним
        Mine theMine = _currentNode as Mine;
        if (theMine != null)
        {          
            StartCoroutine(WaitAtMine(theMine));
        }  

        _path = pathManager.GetCachedShortPath(_currentNode, _targetNode);
    }

    private void Update()
    {
        if (_path == null || _isManing) 
            return;

        int nextWaypoint = _currentWaypoint + 1;

        if (nextWaypoint > _path.Count-1)
        {
            // Достигли конца пути 
            _currentWaypoint = 0;
            _progress = 0;
            _currentNode = _targetNode;    

            // Если есть груз, то ищем базу, иначе ищем шахту
            if (_countResource > 0)
                _targetNode = _pathManager.GetBestBaseNode(_currentNode, speed, speedMine); 
            else
                _targetNode = _pathManager.GetBestMineNode(_currentNode, speed, speedMine);    

            _path = _pathManager.GetCachedShortPath(_currentNode, _targetNode);

            // Поворачиваем в дефолтное направление, если следующей точки нет
            this.transform.rotation = Quaternion.identity;
        }
        else
        {
            float pathLength = _pathManager.GetPathSectionLength(_path[_currentWaypoint], _path[_currentWaypoint + 1]);

            _progress += Time.deltaTime * _speedFactor * speed / pathLength;
            _progress = Mathf.Clamp01(_progress);

            Vector3 startPos = _path[_currentWaypoint].transform.position;
            Vector3 endPos = _path[nextWaypoint].transform.position;
            this.transform.position = Vector3.Lerp(startPos, endPos, _progress);

            // Поворачиваем капсулу (поезд) в сторону следующей точки
            Vector3 direction = (endPos - startPos).normalized;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            if (_progress >= 1f)
            {
                // Достигли ноды, выбираем новую цель
                _currentWaypoint++;
                _progress = 0;

                Mine theMine = _path[_currentWaypoint] as Mine;
                Base theBase = _path[_currentWaypoint] as Base;

                // Если мы прибыли в шахту
                if (theMine != null && _countResource == 0)
                {
                    // Поезду разрешено/запрещено пропускать шахту
                    if (_gameLevel.IsTrainСanSkipMine && _currentWaypoint == _path.Count-1)
                    {
                        StartCoroutine(WaitAtMine(theMine));
                    }
                    else
                    {
                        StartCoroutine(WaitAtMine(theMine));
                    }
                }
                // Если прибыли в базу
                else if(theBase != null && _countResource > 0)
                {
                    // Поезду разрешено/запрещено пропускать базу
                    if (_gameLevel.IsTrainСanSkipBase && _currentWaypoint == _path.Count-1)
                    {
                        LevelData.ModifyTotalResources(theBase.resourceMultiplier * _countResource);

                        _countResource = 0;
                    }
                    else
                    {
                        LevelData.ModifyTotalResources(theBase.resourceMultiplier * _countResource);

                        _countResource = 0;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Майнинг в шахте
    /// </summary>
    private IEnumerator WaitAtMine(Mine mine)
    {
        _isManing = true;

        // Поворачиваем в дефолтное направление
        this.transform.rotation = Quaternion.identity;

        float waitTime = mine.miningTimeMultiplier * speedMine;

        yield return new WaitForSeconds(waitTime);

        _countResource += 1f;

        _isManing = false;
    }

    public void SetSpeedFactor(float value)
    {
        _speedFactor = value;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Стиль
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black; 
        style.fontSize = 13; 
        style.alignment = TextAnchor.MiddleRight;

        // Отрисовка текста с множителем
        Handles.Label(transform.position + Vector3.left * 0.6f, $"{_countResource}", style);

        if (_path == null || _path.Count == 0)
            return;

        // Устанавливаем цвет Gizmos на зеленый
        Handles.color = _pathColor;

        // Толщина линии (например, 5 пикселей)
        float thickness = 5f;

        // Рисуем линию между всеми точками пути с использованием Handles
        for (int i = _currentWaypoint; i < _path.Count - 1; i++)
        {
            Vector3 startPos = _path[i].transform.position;
            Vector3 endPos = _path[i + 1].transform.position;
            Handles.DrawAAPolyLine(thickness, startPos, endPos);
        }
    }
#endif
}