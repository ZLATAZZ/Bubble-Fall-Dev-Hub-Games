using System;
using UnityEngine;

/// <summary>
/// ��������� ������� � ���������� ������ �� enum.
/// </summary>
public class ColorsController : MonoBehaviour
{
    private Color _currentColor;
    private Colors _currentColorEnum;

    /// <summary>
    /// ���������� ��������� ���� � ��������� ��� � �����������.
    /// </summary>
    public Color GetRandomColor()
    {
        Array values = Enum.GetValues(typeof(Colors));
        _currentColorEnum = (Colors)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        _currentColor = GetColorFromEnum(_currentColorEnum);
        return _currentColor;
    }

    /// <summary>
    /// ����������� �������� ������������ Colors � Unity Color.
    /// </summary>
    private Color GetColorFromEnum(Colors colorEnum)
    {
        return colorEnum switch
        {
            Colors.Red => Color.red,
            Colors.Green => Color.green,
            Colors.Blue => Color.blue,
            Colors.Yellow => Color.yellow,
            _ => Color.white,
        };
    }

    public Color CurrentColor => _currentColor;
    public Colors CurrentColorEnum => _currentColorEnum;
}

/// <summary>
/// ��������� ������� �����.
/// </summary>
public enum Colors
{
    Red,
    Green,
    Blue,
    Yellow
}
