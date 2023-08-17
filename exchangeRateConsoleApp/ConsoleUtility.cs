namespace ExchangeRateConsoleApp
{
    public class ConsoleUtility
    {
        public void PrintHeader( string text )
        {
            Console.WriteLine("======================================");
            Console.WriteLine($"        {text}");
            Console.WriteLine("======================================");
        }

        public void PrintSubHeader( string text )
        {
            Console.WriteLine(text);
            Console.WriteLine("--------------------------------------");
        }

        public void PrintFooter( string text )
        {
            Console.WriteLine("======================================");
            Console.WriteLine(text);
        }
    }
}