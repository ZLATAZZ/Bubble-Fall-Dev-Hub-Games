using UnityEngine;

public class ColorsController : MonoBehaviour
{
    private Color _color;
    private Colors currentColorEnum;

    public Color GetRandomColor()
    {
        int enumLength = System.Enum.GetNames(typeof(Colors)).Length;
        currentColorEnum = (Colors)Random.Range(0, enumLength);
        UpdateColor();
        return _color;
    }

    private void UpdateColor()
    {
        switch (currentColorEnum)
        {
            case Colors.Red: _color = Color.red; break;
            case Colors.Green: _color = Color.green; break;
            case Colors.Blue: _color = Color.blue; break;
            case Colors.Yellow: _color = Color.yellow; break;
            default: _color = Color.white; break;
        }
    }
}


public enum Colors
{
    Red,
    Green,
    Blue,
    Yellow,
    Orange,
    Purple,
    Pink
}
