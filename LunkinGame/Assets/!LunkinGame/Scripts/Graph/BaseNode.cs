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
        neighborNodes = pathManager.GetNeighbors(this).ToArray();
	}
}