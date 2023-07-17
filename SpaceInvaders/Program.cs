using SpaceInvaders;

public static class Program
{
    public static void Main(params string[] args)
    {
        using var game = new SpaceInvadersGame();
        game.Run();        
    }
}