using System.Drawing;
using TelegramBotHub.MiniGamesModule.TicTacToe.Additionals;

namespace TelegramBotHub.MiniGamesModule.TicTacToe.Interfaces;

public interface ITicTacToeDrawer
{
    public Bitmap DrawView(bool[,] markers, bool isChoiceBlock, Point chosenBlock, Cell lastTurn);
}