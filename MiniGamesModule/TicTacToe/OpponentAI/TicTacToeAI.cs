using TelegramBotHub.MiniGamesModule.TicTacToe.Additionals;
using TelegramBotHub.MiniGamesModule.TicTacToe.Interfaces;

namespace TelegramBotHub.MiniGamesModule.TicTacToe.OpponentAI;

public class TicTacToeAI : IOpponentAI
{
    private Field _field;

    public TicTacToeAI(Field field)
    {
        _field = field;
    }

    public void ReactToTurn(Cell turn)
    {
    }

    public Cell ChoiceTurn(List<Cell> possibleTurns)
    {
        var maxPotential = Potential.Hopeless;
        var maxPotentialTurns = new List<Cell>();

        foreach (var turn in possibleTurns)
        {
            var potential = CountPotential(turn);
            if (potential > maxPotential)
            {
                maxPotentialTurns.Clear();
                maxPotentialTurns.Add(turn);
                maxPotential = potential;
            }
            else if (potential == maxPotential)
            {
                maxPotentialTurns.Add(turn);
            }
        }

        var random = new Random();
        return maxPotentialTurns[random.Next(maxPotentialTurns.Count)];
    }

    private Potential CountPotential(Cell turn)
    {
        var block = _field.Blocks[turn.BlockPosition.X, turn.BlockPosition.Y];
        var lines = PossibleLineFinder.FindLines(turn.CellInBlockPosition);

        var maxPotential = Potential.Hopeless;
        var secondPotential = Potential.Hopeless;
        foreach (var line in lines)
        {
            var value1 = block.GetValue(line.Item1);
            if (value1 == "") value1 = " ";
            var value2 = block.GetValue(line.Item2);
            if (value2 == "") value2 = " ";

            var potential = CountLinePotential(value1, value2);

            if (potential > maxPotential)
            {
                secondPotential = maxPotential;
                maxPotential = potential;
            }
            else if (potential == maxPotential ||
                     potential > secondPotential)
            {
                secondPotential = potential;
            }
        }

        if (maxPotential == Potential.BlockFutureLine && secondPotential == Potential.BlockFutureLine)
            return Potential.BlockFutureTrap;
        if (maxPotential == Potential.PutLine && secondPotential == Potential.PutLine)
            return Potential.PutTrap;
        return maxPotential;
    }

    private Potential CountLinePotential(String value1, String value2)
    {
        switch (value1 + value2)
        {
            case "XO":
            case "OX":
                return Potential.Hopeless;
            case "  ":
                return Potential.Empty;
            case " X":
            case "X ":
                return Potential.BlockFutureLine;
            case " O":
            case "O ":
                return Potential.PutLine;
            case "XX":
                return Potential.BlockLine;
            case "OO":
                return Potential.CloseZone;
            default: throw new Exception();
        }
    }
}