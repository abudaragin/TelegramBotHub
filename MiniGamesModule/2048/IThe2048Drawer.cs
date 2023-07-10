using System.Drawing;

namespace TelegramBotHub.MiniGamesModule._2048;

public interface IThe2048Drawer
{
    public Bitmap DrawView(int[,] numbers);
}