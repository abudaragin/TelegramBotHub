namespace TelegramBotHub.MiniGamesModule.TicTacToe.OpponentAI;

public enum Potential
{
    Hopeless = 0,
    Empty = 1,
    BlockFutureLine = 2,
    BlockFutureTrap = 3,
    PutLine = 4,
    PutTrap = 5,
    BlockLine = 6,
    CloseZone = 7,
    WinGame = 8
}