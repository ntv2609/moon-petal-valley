namespace MoonpetalValley.Core
{
    public interface ISaveable
    {
        string SaveKey { get; }
        string CaptureStateJson();
        void RestoreStateJson(string json);
    }
}
