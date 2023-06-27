using BlueNotation.Music;

namespace BlueNotation.Pages;

public readonly record struct TableNoteData(Note Note, int TotalAttempts, int TotalCorrect, int Accuracy, float AverageTime);
public readonly record struct TableKeyData(string Key, int TotalAttempts, int TotalCorrect, int Accuracy, float AverageTime);
