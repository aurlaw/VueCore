namespace VueCore
{
    public  class Utility
    {
    public string GetAssemblyVersion()
    {
        return GetType().Assembly.GetName().Version.ToString();
    }        
    }
}