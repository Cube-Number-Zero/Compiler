public class Utils
{
    public static void Error(string msg) {
        Console.Error.WriteLine(msg);
        Environment.Exit(1);
    }
}