namespace TelegramBotHub.MiniGamesModule.BullsAndCows;

public class BullsAndCowsSettings
{
    private const int MinCount = 4;
    private const int MaxCount = 6;

    private const int DefaultCount = 4;
    private const bool DefaultCanRepeated = false;

    private int _count;
    public int Count
    {
        get => _count;
        set
        {
            if (value >= MinCount && value <= MaxCount)
                _count = value;
        }
    }
    public bool CanRepeated { get; set; }

    public BullsAndCowsSettings()
    {
        _count = DefaultCount;
        CanRepeated = DefaultCanRepeated;
    }
    
    public int[] GetCountOptions()
    {
        var result = new int[MaxCount - MinCount + 1];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = MinCount + i;
        }

        return result;
    }
}