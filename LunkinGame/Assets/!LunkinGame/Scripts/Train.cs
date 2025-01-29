using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Управление поездом
/// </summary>
public class Train : MonoBehaviour
{
    public float speed = 5f; 

    private PathManager pathManager;
    private BaseNode currentNode;
    private BaseNode targetNode;
    private List<BaseNode> path;
    private float progress = 0f;

    public void Init(PathManager pathManager, BaseNode startNode)
    {
        this.pathManager = pathManager;

        this.currentNode = startNode;

        this.targetNode = pathManager.GetRandomNode();

        this.path = pathManager.GetCachedPath(currentNode, targetNode);
    }

    private void Update()
    {
        if (path == null || path.Count < 2) return;

        progress += Time.deltaTime * speed / GetTotalPathLength();
        progress = Mathf.Clamp01(progress);

        Vector3 startPos = path[0].transform.position;
        Vector3 endPos = path[1].transform.position;
        transform.position = Vector3.Lerp(startPos, endPos, progress);

        if (progress >= 1f)
        {
            // Достигли конечной ноды, выбираем новую цель
            currentNode = targetNode;
            targetNode = pathManager.GetRandomNode();
            path = pathManager.GetCachedPath(currentNode, targetNode);
            progress = 0f;
        }
    }

    private float GetTotalPathLength()
    {
        float totalLength = 0f;
        for (int i = 0; i < path.Count - 1; i++)
        {
            totalLength += pathManager.GetPathLength(path[i], path[i + 1]);
        }
        return totalLength;
    }
}