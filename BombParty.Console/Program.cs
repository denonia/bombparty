namespace BombParty.ConsoleClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var gameService = new GameClient("https://localhost:7173/game-hub");
            gameService.ConnectAsync().Wait();
            gameService.SendChatMessage("Hello, world").Wait();

            while (true)
            {
                var input = Console.ReadLine();
                gameService.SubmitInput(input).Wait();
            }
        }
    }
}
