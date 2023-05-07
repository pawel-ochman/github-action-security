namespace GithubActionsDotnet.Common.Service;

public class ActionLogger : IActionLogger
{
    public void Info(string message) => Display(ConsoleColor.White, $"INFO - {message}");

    public void Error(string message) => Display(ConsoleColor.Red, $"ERRO - {message}");

    public void Warning(string message) => Display(ConsoleColor.Yellow, $"WARN - {message}");

    public void Debug(string message) => Display(ConsoleColor.Gray, $"DEBG - {message}");

    private void Display(ConsoleColor color, string message)
    {
        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine($"[{DateTime.UtcNow}] {message}");
        Console.ForegroundColor = previousColor;
    }
}