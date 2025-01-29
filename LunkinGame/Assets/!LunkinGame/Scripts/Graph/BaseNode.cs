using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Базовый класс всех Node
/// </summary>	
public class BaseNode : MonoBehaviour
{
    public BaseNode[] neighborNodes;

	public void Init(PathManager pathManager)
	{
        List<BaseNode> neighbors = new List<BaseNode>();

        foreach (var path in pathManager.paths)
        {
            if (path.nodeA == this)
            {
                neighbors.Add(path.nodeB);
            }
            else if (path.nodeB == this)
            {
                neighbors.Add(path.nodeA);
            }
        }

        neighborNodes = neighbors.ToArray();
	}
}