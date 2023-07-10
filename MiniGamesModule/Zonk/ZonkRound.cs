using System.Text;

namespace TelegramBotHub.MiniGamesModule.Zonk;

public class ZonkRound
{
    public int Result { get; private set; }
    public List<int> SavedBones { get; private set; }
    public List<int> RolledComboBones { get; private set; }
    public List<int> RolledWithoutComboBones { get; private set; }

    public ZonkRound()
    {
        Result = 0;
        SavedBones = new List<int>();
        RolledComboBones = new List<int>();
        RolledWithoutComboBones = new List<int>();
        Roll();
    }

    #region OPTIONS

    public bool CanStop()
    {
        return Result >= 300;
    }

    public bool CanContinue()
    {
        return RolledComboBones.Count > 0;
    }

    public void Continue()
    {
        Roll();
    }

    #endregion

    #region ROLL

    private void Roll()
    {
        SavedBones.AddRange(RolledComboBones);
        RolledComboBones = new List<int>();
        if (SavedBones.Count == 6) SavedBones = new List<int>();

        int bonesToRoll = 6 - SavedBones.Count;
        RolledWithoutComboBones = new List<int>();
        var random = new Random();
        for (int i = 0; i < bonesToRoll; i++)
        {
            RolledWithoutComboBones.Add(random.Next(6) + 1);
        }

        CheckSixDifferent();
        CheckThreePairs();
        CheckTripleOrMore();
        CheckSolos();
    }

    private void CheckSixDifferent()
    {
        if(RolledWithoutComboBones.Count < 6) return;
        if (RolledWithoutComboBones.Contains(1) &&
            RolledWithoutComboBones.Contains(2) &&
            RolledWithoutComboBones.Contains(3) &&
            RolledWithoutComboBones.Contains(4) &&
            RolledWithoutComboBones.Contains(5) &&
            RolledWithoutComboBones.Contains(6))
        {
            RolledComboBones.AddRange(RolledWithoutComboBones);
            RolledWithoutComboBones.Clear();
            Result += 1500;
        }
    }
    
    private void CheckThreePairs()
    {
        if(RolledWithoutComboBones.Count < 6) return;
        var copy = new List<int>(RolledWithoutComboBones);
        copy.Sort();
        if (copy[0] == copy[1] &&
            copy[1] != copy[2] &&
            copy[2] == copy[3] &&
            copy[3] != copy[4] &&
            copy[4] == copy[5])
        {
            RolledComboBones.AddRange(RolledWithoutComboBones);
            RolledWithoutComboBones.Clear();
            Result += 750;
        }
        
    }
    
    private void CheckTripleOrMore()
    {
        if(RolledWithoutComboBones.Count < 3) return;
        for (int num = 1; num <= 6; num++)
        {
            int count = 0;
            foreach (var number in RolledWithoutComboBones)
            {
                if (num == number) count++;
            }

            if (count >= 3)
            {
                int resultPlus = num * 100;
                if (num == 1) resultPlus = 1000;
                if (resultPlus > 3) resultPlus *= count - 2;
                Result += resultPlus;

                for (int i = 0; i < count; i++)
                {
                    RolledWithoutComboBones.Remove(num);
                    RolledComboBones.Add(num);
                }
            }
        }
    }
    
    private void CheckSolos()
    {
        for (int i = 0; i < RolledWithoutComboBones.Count; i++)
        {
            if (RolledWithoutComboBones[i] == 1)
            {
                RolledComboBones.Add(RolledWithoutComboBones[i]);
                RolledWithoutComboBones.RemoveAt(i);
                i--;
                Result += 100;
                continue;
            }

            if (RolledWithoutComboBones[i] == 5)
            {
                RolledComboBones.Add(RolledWithoutComboBones[i]);
                RolledWithoutComboBones.RemoveAt(i);
                i--;
                Result += 50;
            }
        }
    }

    #endregion

    #region TEXT

    public String GetReport()
    {
        var builder = new StringBuilder();
        builder.Append("\n");
        builder.Append("Брошенно: ").Append(GetLineReport(RolledWithoutComboBones));
        builder.Append("\n");
        builder.Append("Комбо: ").Append(GetLineReport(RolledComboBones));
        builder.Append("\n");
        builder.Append("\n");
        builder.Append("Сохранено: ").Append(GetLineReport(SavedBones));
        builder.Append("\n");
        builder.Append(GetLastString());

        return builder.ToString();
    }

    private String GetLastString()
    {
        if (CanContinue())
            return "Накопленно за раунд: " + Result;
        return "\"Зонк\"";
    }

    private String GetLineReport(List<int> line)
    {
        var builder = new StringBuilder();
        foreach (var num in line)
        {
            builder.Append(GetDice(num) + " ");
        }

        return builder.ToString();
    }

    private String GetDice(int number)
    {
        return number switch
        {
            1 => "1️⃣",
            2 => "2️⃣",
            3 => "3️⃣",
            4 => "4️⃣",
            5 => "5️⃣",
            6 => "6️⃣",
        };
    }

    #endregion
}