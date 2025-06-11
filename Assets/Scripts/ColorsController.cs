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
            case Colors.Orange: _color = new Color(1f, 0.65f, 0f); break;
            case Colors.Purple: _color = new Color(0.5f, 0f, 0.5f); break;
            case Colors.Pink: _color = new Color(1f, 0.75f, 0.8f); break;
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
