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

        foreach (var section in pathManager.sections)
        {
            if (section.nodeA == this)
            {
                neighbors.Add(section.nodeB);
            }
            else if (section.nodeB == this)
            {
                neighbors.Add(section.nodeA);
            }
        }

        neighborNodes = neighbors.ToArray();
	}
}