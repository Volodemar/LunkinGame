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

    [SerializeField] private Color pathColor;

    private PathManager _pathManager;
    private BaseNode _currentNode;
    private BaseNode _targetNode;
    private List<BaseNode> _path;

    private float _progress = 0f;
    private int _currentWaypoint = 0;
    private float _countResource = 0;

    private bool _isManing = false;

    // Замедление движения поездов для лучшего наблюдения
    private const float _movementSpeedFactor = 0.1f;

    public void Init(PathManager pathManager, BaseNode startNode)
    {
        _pathManager = pathManager;

        _currentNode = startNode;

        // Выбираем лучшую шахту
        _targetNode = pathManager.GetBestMineNode(_currentNode, speed, speedMine);

        Mine theMine = _currentNode as Mine;
        if (theMine != null)
        {
            // Стартанули в шахте, значит майним
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
            transform.rotation = Quaternion.identity;
        }
        else
        {
            float pathLength = _pathManager.GetPathSectionLength(_path[_currentWaypoint], _path[_currentWaypoint + 1]);

            _progress += Time.deltaTime * _movementSpeedFactor * speed / pathLength;
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
                if (theMine != null)
                {
                    // Запускаем майнинг в шахте
                    StartCoroutine(WaitAtMine(theMine));
                }
                // Если прибыли в базу
                else if(theBase != null)
                {
                    LevelData.ModifyTotalResources(theBase.resourceMultiplier * _countResource);

                    _countResource = 0;
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
        transform.rotation = Quaternion.identity;

        float waitTime = mine.miningTimeMultiplier * speedMine;

        yield return new WaitForSeconds(waitTime);

        _countResource += 1f;

        _isManing = false;
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
        Handles.color = pathColor;

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