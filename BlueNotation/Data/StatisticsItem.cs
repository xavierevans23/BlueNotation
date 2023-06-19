using BlueNotation.Music;

namespace BlueNotation.Data;

public abstract class StatisticsItem
{
    public int TotalAttempts { get; set; } = 0;
    public int TotalTimesPlayed { get; set; } = 0;
    public int TotalLatency { get; set; } = 0;
    public DateTime LastPlayed { get; set; } = DateTime.Now;

    public const int MaxHistory = 5;

    public StatisticsItemHistory[] History = new StatisticsItemHistory[MaxHistory];

    public int HistoryPointer = 0;

    public void AddData(int attempts, int timesPlayed, int latencyTotal)
    {
        LastPlayed = DateTime.Now;

        TotalAttempts += attempts;
        TotalTimesPlayed += timesPlayed;
        TotalLatency += latencyTotal;

        HistoryPointer++;
        if (HistoryPointer >= MaxHistory)
        {
            HistoryPointer = 0;
        }

        History[HistoryPointer] = new(attempts, timesPlayed, latencyTotal);
    }

    public StatisticsItemHistory GetHistory(int count)
    {
        if (count > MaxHistory)
        {
            throw new ArgumentException("Exceeded max history length.",nameof(count));
        }

        var totalAttempts = 0;
        var totalTimesPlayed = 0;
        var totalLatency = 0;

        var pointer = HistoryPointer;

        do
        {
            var item = History[pointer];
            
            totalAttempts += item.Attempts;
            totalTimesPlayed += item.TimesPlayed;
            totalLatency += item.Latency;

            pointer--;
            if (pointer < 0)
            {
                pointer = MaxHistory - 1;
            }

            count--;
        }
        while (count > 0);

        return new StatisticsItemHistory(totalAttempts, totalTimesPlayed, totalLatency);
    }
}

public class NoteStatisticsItem : StatisticsItem
{
    public int Midi { get; set; } = 0;
}

public class KeyStatisticsItem : StatisticsItem
{
    public Key Key { get; set; } = new(Letter.C, Accidental.Natural);
}