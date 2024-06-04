using NcnnDotNet.OpenCV;

namespace FacePhys.Models;

public sealed class Object
{
    public Object()
    {
        this.Rect = new Rect<float>();
    }

    public Rect<float> Rect
    {
        get;
        set;
    }

    public int Label
    {
        get;
        set;
    }

    public float Prob
    {
        get;
        set;
    }
}