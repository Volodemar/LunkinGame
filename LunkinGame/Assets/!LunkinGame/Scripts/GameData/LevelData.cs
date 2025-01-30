using UnityEngine;
/// <summary>
/// Хранение данных игры (выбран такой вариант т.к для реализации ТЗ больше функционала не нужно)
/// </summary>	
public static class LevelData
{
    private static float _totalResources;

    public static float TotalResources
    {
        get { return _totalResources; }
        private set { _totalResources = value; }
    }

    public static void ModifyTotalResources(float value)
    {
        _totalResources = Mathf.Clamp(_totalResources + value, 0, float.MaxValue);
    }
}
