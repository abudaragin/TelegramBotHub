using System.Drawing;

namespace TelegramBotHub.MiniGamesModule.TicTacToe.Additionals;

public class PossibleLineFinder
{
    private static Tuple<Point, Point>[] _shifts = new Tuple<Point, Point>[]
    {
        Tuple.Create(new Point(-2, -2), new Point(-1, -1)),
        Tuple.Create(new Point(-1, -1), new Point(1, 1)),
        Tuple.Create(new Point(1, 1), new Point(2, 2)),

        Tuple.Create(new Point(-2, 2), new Point(-1, 1)),
        Tuple.Create(new Point(-1, 1), new Point(1, -1)),
        Tuple.Create(new Point(1, -1), new Point(2, -2)),

        Tuple.Create(new Point(-2, 0), new Point(-1, 0)),
        Tuple.Create(new Point(-1, 0), new Point(1, 0)),
        Tuple.Create(new Point(1, 0), new Point(2, 0)),

        Tuple.Create(new Point(0, -2), new Point(0, -1)),
        Tuple.Create(new Point(0, -1), new Point(0, 1)),
        Tuple.Create(new Point(0, 1), new Point(0, 2)),
    };

    public static List<Tuple<Point, Point>> FindLines(Point coordinate)
    {
        var result = new List<Tuple<Point, Point>>();
        foreach (var shift in _shifts)
        {
            var newPoint1 = new Point(
                coordinate.X + shift.Item1.X,
                coordinate.Y + shift.Item1.Y);
            var newPoint2 = new Point(
                coordinate.X + shift.Item2.X,
                coordinate.Y + shift.Item2.Y);
            if (IsPointExist(newPoint1) && IsPointExist(newPoint2))
            {
                result.Add(Tuple.Create<Point, Point>(newPoint1, newPoint2));
            }
        }

        return result;
    }

    private static bool IsPointExist(Point position)
    {
        return position.X >= 0
               && position.X <= 2
               && position.Y >= 0
               && position.Y <= 2;
    }
}