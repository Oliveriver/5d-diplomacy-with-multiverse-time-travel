import java.util.*;
import java.util.stream.Stream;

public class Game {
    private ArrayList<Army> armies = new ArrayList<>();
    private ArrayList<Board> boards = new ArrayList<>();
    private ArrayList<Army> retreatingArmies = new ArrayList<>();

    public Game()
    {
        // 0 = Blue province
        // 1 = Neutral province
        // 2 = Orange province
        armies.add(new Army(0,0,0, Player.BLUE));
        armies.add(new Army(0,0,2, Player.ORANGE));
        updateBoards();
    }

    public void updateBoards()
    {
        for (Army army : armies)
        {
            if (boards.stream().noneMatch(board -> board.getPosition()[0] == army.getLocation()[0] && board.getPosition()[1] == army.getLocation()[1]))
            {
                boards.add(new Board(army.getLocation()[0], army.getLocation()[1]));
            }
        }
    }

    public ArrayList<Order> parseOrders(ArrayList<String> ordersText)
    {
        ArrayList<Order> orders = new ArrayList<>();
        for (String orderText : ordersText) {
            Order order;
            String[] words = orderText.split(" ");
            if (words.length > 1)
            {
                switch (words[1])
                {
                    case "m":
                        order = new Move();
                        order.setLocation(words[0].split(","));
                        ((Move) order).setDestination(words[2].split(","));
                        break;
                    case "s":
                        order = new Support();
                        order.setLocation(words[0].split(","));
                        ((Support) order).setSupportLocation(words[2].split(","));
                        ((Support) order).setSupportDestination(words[3].split(","));
                        break;
                    default:
                        order = new Hold();
                        order.setLocation(words[0].split(","));
                }
            }
            else
            {
                order = new Hold();
                order.setLocation(words[0].split(","));
            }

            orders.add(order);
        }
        return orders;
    }

    public boolean isRetreatNeeded()
    {
        return retreatingArmies != null && retreatingArmies.size() > 0;
    }

    public ArrayList<Retreat> parseRetreats(ArrayList<String> retreatsText)
    {
        ArrayList<Retreat> retreats = new ArrayList<>();
        for (String retreatText : retreatsText)
        {
            Retreat retreat;
            String[] words = retreatText.split(" ");
            switch (words[1])
            {
                case "m":
                    retreat = new RetreatMove();
                    retreat.setLocation(words[0].split(","));
                    ((RetreatMove) retreat).setDestination(words[2].split(","));
                    break;
                default:
                    retreat = new Disband();
                    retreat.setLocation(words[0].split(","));
                    break;
            }

            retreats.add(retreat);
        }
        return retreats;
    }

    public void resolveOrders(ArrayList<Order> orders)
    {
        retreatingArmies = new ArrayList<>();
        // LOGIC FOR PROCESSING ORDERS GOES HERE
        /**
         * 1. Find units (including units to be supported) and ensure they're on active boards
         * 2. Ensure orders are legal for current game state?
         * 3. Find supports that should be cut and cut them (be careful with cutting support of same colour!)
         * 3. For each move, find all its incoming supports and add them to its strength
         * 4. For each hold, find all its incoming supports and add them to its strength
         * 5. For each move, find the front end of its move chain
         * 6. For each front of a move chain, determine whether it's successful by comparing its destination to other fronts of move chains and holds
         * 7. Work backwards through each move chain...somehow? Conflicts along the way could get messy...
         * 8. Make an army one space in the future for all successful moves to active boards
         * 9. Make an army one space in the future for all holds on active boards, unless something's already there, in which case flag for a retreat
         * 10. For each move to an inactive board, make a new army at that position one higher/lower than the highest/lowest y position, then duplicate everything at the destination x position to that y position, unless something's already there, in which case flag for a retreat
         * 11. Make new boards (updateBoards?) and make boards not at the highest x position in a given row inactive
         */
        // Remove orders where the originating unit is on an inactive board.
        orders.removeIf(order -> {
            Board board = getBoard(order.getLocation());
            return board == null || !board.isActive();
        });

        // Remove support orders where the unit to be supported is on an inactive board.
        orders.removeIf(order -> {
            if (order instanceof Support)
            {
                Support support = (Support) order;
                Board board = getBoard(support.getSupportLocation());
                return board == null || !board.isActive();
            }
            return false;
        });

        // Remove orders which move beyond the +-1 range.
        orders.removeIf(order -> {
            if (order instanceof Move)
            {
                Move move = (Move) order;
                return Math.abs(move.getLocation()[0] - move.getDestination()[0]) > 1 ||
                        Math.abs(move.getLocation()[1] - move.getDestination()[1]) > 1;
            }
            if (order instanceof Support)
            {
                Support support = (Support) order;
                return Math.abs(support.getLocation()[0] - support.getSupportLocation()[0]) > 1 ||
                        Math.abs(support.getLocation()[1] - support.getSupportLocation()[1]) > 1 ||
                        Math.abs(support.getLocation()[0] - support.getSupportDestination()[0]) > 1 ||
                        Math.abs(support.getLocation()[1] - support.getSupportDestination()[1]) > 1;
            }
            return false;
        });

        // TODO: Ensure further legality of moves?

        // Set the player for each order (for now, assuming the requisite army exists).
        for (Order order : orders)
        {
            if (order instanceof Retreat)
            {
                // Ignore retreats here as these are handled elsewhere.
            }
            else
            {
                Optional<Army> armyStream = armies.stream().filter(army -> Arrays.equals(army.getLocation(), order.getLocation())).findFirst();
                armyStream.ifPresent(army -> order.setPlayer(army.getOwner()));
            }
        }

        // Generate lists of each order type.
        ArrayList<Support> supportOrders = new ArrayList<>();
        ArrayList<Move> moveOrders = new ArrayList<>();
        ArrayList<Hold> holdOrders = new ArrayList<>();
        for (Order order : orders)
        {
            if (order instanceof Support)
            {
                supportOrders.add((Support) order);
            }
            else if (order instanceof Move)
            {
                moveOrders.add((Move) order);
            }
            else if (order instanceof Hold)
            {
                holdOrders.add((Hold) order);
            }
        }

        // Add supports to list of holds for later.
        for (Support support : supportOrders)
        {
            Hold hold = new Hold();
            hold.setLocation(support.getLocation());
            hold.setPlayer(support.getPlayer());
            holdOrders.add(hold);
        }

        // Find supports that should be cut and cut them (but not units of the same player!).
        ArrayList<Support> filteredSupportOrders = new ArrayList<>();
        for (Support support : supportOrders)
        {
            boolean keepSupport = true;
            for (Move move : moveOrders)
            {
                if (Arrays.equals(move.getDestination(), support.getLocation()) && move.getPlayer() != support.getPlayer())
                {
                    keepSupport = false;
                }
            }
            if (keepSupport)
                filteredSupportOrders.add(support);
        }

        // For each move, find all its incoming supports and add them to its strength.
        for (Move move : moveOrders)
        {
            Stream<Support> incomingSupports = filteredSupportOrders.stream().filter(support ->
                    Arrays.equals(support.getSupportLocation(), move.getLocation()) &&
                            Arrays.equals(support.getSupportDestination(), move.getDestination()));
            move.addStrength((int) incomingSupports.count());
        }

        // TODO: Do something to give hold orders by default for units not ordered?

        // For each hold, find all its incoming supports and add them to its strength.
        for (Hold hold : holdOrders)
        {
            Stream<Support> incomingSupports = filteredSupportOrders.stream().filter(support ->
                    Arrays.equals(support.getSupportLocation(), hold.getLocation()) &&
                            Arrays.equals(support.getSupportDestination(), hold.getLocation()));
            hold.addStrength((int) incomingSupports.count());
        }

        // For each move, find the front end of its move chain (avoid duplicates!).
        ArrayList<Move> frontMoves = new ArrayList<>();
        for (Move move : moveOrders)
        {
            // I sure hope streams aren't destructive...
            Stream<Move> movesFromDestination = moveOrders.stream().filter(
                    moveOrder -> Arrays.equals(move.getDestination(), moveOrder.getLocation()));
            if (movesFromDestination.findFirst().isEmpty() && !frontMoves.contains(move))
            {
                frontMoves.add(move);
            }
        }

        // For each front of a move chain, determine whether it's successful by comparing its destination to other moves
        // and holds. If it's moving to an inactive board, compare with army locations.
        ArrayList<Move> successfulMoves = new ArrayList<>();
        for (Move move : frontMoves)
        {
            Stream<Move> competingMoves = moveOrders.stream().filter(competingMove ->
                    !move.equals(competingMove) &&
                    Arrays.equals(move.getDestination(), competingMove.getDestination()));
            Optional<Hold> competingHold = holdOrders.stream().filter(hold ->
                    Arrays.equals(move.getDestination(), hold.getLocation())).findFirst();
            Board targetBoard = getBoard(move.getDestination());
            Stream<Army> armiesAtDestination = armies.stream().filter(army -> Arrays.equals(army.getLocation(), move.getDestination()));
            if (!targetBoard.isActive() && armiesAtDestination.count() > 0 && move.getStrength() == 1) {
                Hold hold = new Hold();
                hold.setLocation(move.getLocation());
                hold.setPlayer(move.getPlayer());
                holdOrders.add(hold);
            }
            else if (competingMoves.allMatch(competingMove -> move.getStrength() > competingMove.getStrength()) &&
                    (competingHold.isEmpty() || move.getStrength() > competingHold.get().getStrength()))
            {
                move.setSuccessful(true);
                successfulMoves.add(move);
            }
            else
            {
                Hold hold = new Hold();
                hold.setLocation(move.getLocation());
                hold.setPlayer(move.getPlayer());
                holdOrders.add(hold);
            }
        }

        // Work backwards through each move chain to find successful moves...somehow? Conflicts along the way could get messy...
        for (Move frontMove : frontMoves)
        {
            backtrackMoves(frontMove, moveOrders, successfulMoves, holdOrders);
        }

        // Make an army one (relative) space in the future for all successful moves to active boards.
        for (Move move : successfulMoves)
        {
            int[] destination = move.getDestination();
            if (getBoard(destination).isActive())
            {
                armies.add(new Army(destination[0] + 1, destination[1], destination[2], move.getPlayer()));
            }
        }

        // Make an army one space in the future for all holds on active boards, unless something's already there, in which case flag for a retreat.
        for (Hold hold : holdOrders)
        {
            int[] location = hold.getLocation();
            if (getBoard(location).isActive())
            {
                Army army = new Army(location[0] + 1, location[1], location[2], hold.getPlayer());
                Stream<Army> overlapArmies = armies.stream().filter(existingArmy -> Arrays.equals(existingArmy.getLocation(), location));
                if (overlapArmies.count() > 1)
                {
                    retreatingArmies.add(army);
                }
                else
                {
                    armies.add(army);
                }
            }
        }

        // For each move to an inactive board, make a new army at that position one higher/lower than the highest/lowest
        // y position, then duplicate everything at the destination x position to that y position, unless something's
        // already there, in which case flag for a retreat.
        for (Move move : successfulMoves)
        {
            int[] destination = move.getDestination();
            if (!getBoard(destination).isActive())
            {
                switch (move.getPlayer())
                {
                    case BLUE:
                        int yPosMin = getExtremeYPosition(false) - 1;
                        armies.add(new Army(move.getDestination()[0] + 1, yPosMin, move.getDestination()[2], Player.BLUE));
                        Stream<Army> armiesToDuplicateBelow = armies.stream().filter(army -> army.getLocation()[0] == move.getDestination()[0]);
                        ArrayList<Army> duplicatedArmiesBelow = new ArrayList<>();
                        armiesToDuplicateBelow.forEach(army -> {
                            Army newArmy = new Army(army.getLocation()[0] + 1, yPosMin, army.getLocation()[2], army.getOwner());
                            Stream<Army> overlapArmies = armies.stream().filter(existingArmy -> Arrays.equals(existingArmy.getLocation(), newArmy.getLocation()));
                            if (overlapArmies.count() > 0)
                            {
                                retreatingArmies.add(newArmy);
                            }
                            else
                            {
                                duplicatedArmiesBelow.add(newArmy);
                            }
                        });
                        armies.addAll(duplicatedArmiesBelow);
                        break;
                    case ORANGE:
                        int yPosMax = getExtremeYPosition(true) + 1;
                        armies.add(new Army(move.getDestination()[0] + 1, yPosMax, move.getDestination()[2], Player.ORANGE));
                        Stream<Army> armiesToDuplicateAbove = armies.stream().filter(army -> army.getLocation()[0] == move.getDestination()[0]);
                        ArrayList<Army> duplicatedArmiesAbove = new ArrayList<>();
                        armiesToDuplicateAbove.forEach(army -> {
                            Army newArmy = new Army(army.getLocation()[0] + 1, yPosMax, army.getLocation()[2], army.getOwner());
                            Stream<Army> overlapArmies = armies.stream().filter(existingArmy -> Arrays.equals(existingArmy.getLocation(), newArmy.getLocation()));
                            if (overlapArmies.count() > 0)
                            {
                                retreatingArmies.add(newArmy);
                            }
                            else
                            {
                                duplicatedArmiesAbove.add(newArmy);
                            }
                        });
                        armies.addAll(duplicatedArmiesAbove);
                        break;
                }
            }
        }

        updateBoards();

        // Make boards not at the end of their row inactive.
        for (Board board : boards)
        {
            Stream<Board> boardRow = boards.stream().filter(b -> b.getPosition()[1] == board.getPosition()[1]);
            int endPosition = boardRow.map(b -> b.getPosition()[0]).max(Integer::compareTo).orElseThrow();
            if (board.getPosition()[0] != endPosition)
            {
                board.setActive(false);
            }
        }
    }

    public void resolveRetreats(ArrayList<Retreat> retreats)
    {
        // Do disbands, then construct a list of Move objects with properties matching RetreatMove objects, then call resolveOrders.
        ArrayList<Order> moveOrders = new ArrayList<>();
        for (Retreat reatreat : retreats)
        {
            if (reatreat instanceof Disband)
            {
                // Do nothing for now.
            }
            else
            {
                RetreatMove retreatMove = (RetreatMove) reatreat;
                Move move = new Move();
                move.setLocation(retreatMove.getLocation());
                move.setDestination(retreatMove.getDestination());
                moveOrders.add(move);
                // SET PLAYER???
            }
        }
        // STILL DOESN'T WORK!!!
        resolveOrders(moveOrders);

        retreatingArmies.clear();
    }

    private Board getBoard(int[] location)
    {
        Stream<Board> boardStream = boards.stream().filter(
                board -> board.getPosition()[0] == location[0] && board.getPosition()[1] == location[1]);
        return boardStream.findFirst().orElse(null);
    }

    private int getExtremeYPosition(boolean max)
    {
        return armies.stream().map(army -> army.getLocation()[1]).reduce(0, (acc, value) ->
                max ? Integer.max(acc, value) : Integer.min(acc, value));
    }

    public void backtrackMoves(Move frontMove, ArrayList<Move> allMoves, ArrayList<Move> successfulMoves, ArrayList<Hold> holdOrders)
    {
        Stream<Move> backMoves = allMoves.stream().filter(move -> Arrays.equals(move.getDestination(),frontMove.getLocation()));
        Move backMovesMax;
        try
        {
            backMovesMax = allMoves.stream().filter(move -> Arrays.equals(move.getDestination(), frontMove.getLocation())).max(Comparator.comparingInt(Move::getStrength)).orElseThrow();
        }
        catch (NoSuchElementException e)
        {
            // There are no moves earlier in the chain.
            return;
        }
        Stream<Move> matchingStrengthMoves = allMoves.stream().filter(move -> Arrays.equals(move.getDestination(), frontMove.getLocation())).filter(move -> move.getStrength() == backMovesMax.getStrength());
        if (matchingStrengthMoves.count() > 1)
        {
            // Back moves are at most of equal strength, so are not successful.
            backMoves.forEach(move -> {
                Hold hold = new Hold();
                hold.setLocation(move.getLocation());
                hold.setPlayer(move.getPlayer());
                holdOrders.add(hold);
                backtrackMoves(move, allMoves, successfulMoves, holdOrders);
            });
        }
        else if (frontMove.isSuccessful())
        {
            backMovesMax.setSuccessful(true);
            successfulMoves.add(backMovesMax);
            backtrackMoves(backMovesMax, allMoves, successfulMoves, holdOrders);
            backMoves.forEach(move -> {
                if (!move.equals(backMovesMax))
                {
                    Hold hold = new Hold();
                    hold.setLocation(move.getLocation());
                    hold.setPlayer(move.getPlayer());
                    holdOrders.add(hold);
                    backtrackMoves(move, allMoves, successfulMoves, holdOrders);
                }
            });
        }
        else if (backMovesMax.getStrength() > 1)
        {
            backMovesMax.setSuccessful(true);
            successfulMoves.add(backMovesMax);
            Hold frontHold = new Hold();
            frontHold.setLocation(frontMove.getLocation());
            frontHold.setPlayer(frontMove.getPlayer());
            holdOrders.add(frontHold);
            backtrackMoves(backMovesMax, allMoves, successfulMoves, holdOrders);
            backMoves.forEach(move -> {
                if (!move.equals(backMovesMax))
                {
                    Hold hold = new Hold();
                    hold.setLocation(move.getLocation());
                    hold.setPlayer(move.getPlayer());
                    holdOrders.add(hold);
                    backtrackMoves(move, allMoves, successfulMoves, holdOrders);
                }
            });
        }
        else
        {
            backMoves.forEach(move -> {
                Hold hold = new Hold();
                hold.setLocation(move.getLocation());
                hold.setPlayer(move.getPlayer());
                holdOrders.add(hold);
                backtrackMoves(move, allMoves, successfulMoves, holdOrders);
            });
        }
    }

    public void display()
    {
        for (Board board : boards)
        {
            System.out.println("\n" + board + ":");
            ArrayList<Army> armiesOnBoard = new ArrayList<>();

            for (Army army : armies)
            {
                if (army.getLocation()[0] == board.getPosition()[0] && army.getLocation()[1] == board.getPosition()[1])
                {
                    armiesOnBoard.add(new Army(army.getLocation(), army.getOwner()));
                }
            }

            for (Army army : armiesOnBoard)
            {
                System.out.println("  " + army.getOwner() + " army at " + army.getLocation()[2]);
            }
        }

        if (isRetreatNeeded())
        {
            System.out.println("\nRetreats:");
            for (Army army : retreatingArmies)
            {
                System.out.println("  " + army.getOwner() + " army at " + "(" + army.getLocation()[0] + "," + army.getLocation()[1] + "," + army.getLocation()[2] + ")");
            }
        }
    }
}
