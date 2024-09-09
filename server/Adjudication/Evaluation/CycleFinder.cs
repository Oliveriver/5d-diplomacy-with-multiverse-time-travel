using Entities;
using Enums;

namespace Adjudication;

public class CycleFinder(AdjacencyValidator adjacencyValidator)
{
    private readonly AdjacencyValidator adjacencyValidator = adjacencyValidator;

    public List<Move> GetMoveCycle(List<Order> orders)
    {
        var moves = orders.OfType<Move>().Where(m => m.Status is OrderStatus.New or OrderStatus.Success && !m.IsSzykmanHold).ToList();

        var visitedMoves = new List<Move>();
        var cycle = new List<Move>();

        var move = moves.FirstOrDefault(m => !visitedMoves.Contains(m));
        while (move != null)
        {
            cycle.Add(move);
            visitedMoves.Add(move);

            var nextMove = moves.FirstOrDefault(m => adjacencyValidator.EqualsOrIsRelated(move.Destination, m.Location));

            if (nextMove == null)
            {
                cycle = [];
                move = moves.FirstOrDefault(m => !visitedMoves.Contains(m));
            }
            else
            {
                if (cycle.Contains(nextMove))
                {
                    return cycle;
                }

                move = nextMove;
            }
        }

        return [];
    }
}
