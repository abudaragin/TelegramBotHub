using TelegramBotHub.MiniGamesModule.TicTacToe.Additionals;

namespace TelegramBotHub.MiniGamesModule.TicTacToe.Interfaces;

public interface IOpponentAI
{
    public void ReactToTurn(Cell turn);
    public Cell ChoiceTurn(List<Cell> possibleTurns);
}