namespace KeySplash.Data;

public class SplashOption
{
    public string Name { get; set; }
    public string IdleResource { get; set; }
    public string[] TapResources { get; set; }
    public int Width;
    public int Height;

    public SplashOption(string name,int width, int height, string idleResource, string[] tapResources)
    {
        Name = name;
        Width = width;
        Height = height;
        IdleResource = idleResource;
        TapResources = tapResources;
    } 
    
    public override string ToString()
    {
        return Name;
    }
}