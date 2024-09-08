using Adjudication;
using Entities;
using Enums;
using FluentAssertions;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Tests;

// Adapted and extended from Multiversal Diplomacy Adjudicator Test Cases, Tim Van Baak
// https://github.com/Jaculabilis/5dplomacy/blob/master/MDATC.html

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
public class MDATC_B : AdjudicationTestBase
{
    [Fact(DisplayName = "B.01. Move into own past forks timeline")]
    public void MDATC_B_1()
    {
        // Arrange
        var world = new World();

        var presentBoard = world.AddBoard(phase: Phase.Fall);
        var pastBoard = world.AddBoard();

        List<Unit> units =
            [
                .. pastBoard.AddUnits([(Nation.Germany, UnitType.Army, "Mun")]),
                .. presentBoard.AddUnits([(Nation.Germany, UnitType.Army, "Mun")]),
            ];

        units.Get("Mun").Hold(status: OrderStatus.Success);

        var order = units.Get("Mun", phase: Phase.Fall).Move("Tyr");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Success);

        presentBoard.Next().ShouldHaveUnits([]);
        pastBoard.Next(timeline: 2).ShouldHaveUnits(
            [
                (Nation.Germany, UnitType.Army, "Mun", false),
                (Nation.Germany, UnitType.Army, "Tyr", false),
            ]);
    }

    [Fact(DisplayName = "B.02. Support to past move repelled by move forks timeline")]
    public void MDATC_B_2()
    {
        // Arrange
        var world = new World();

        var presentBoard = world.AddBoard(phase: Phase.Fall);
        var pastBoard = world.AddBoard();

        List<Unit> units =
            [
                .. pastBoard.AddUnits(
                    [
                        (Nation.Germany, UnitType.Army, "Mun"),
                        (Nation.Austria, UnitType.Army, "Boh"),
                    ]),
                .. presentBoard.AddUnits(
                    [
                        (Nation.Germany, UnitType.Army, "Mun"),
                        (Nation.Austria, UnitType.Army, "Boh"),
                    ]),
            ];

        units.Get("Mun").Move("Tyr", status: OrderStatus.Failure);
        units.Get("Boh").Move("Tyr", status: OrderStatus.Failure);

        var austrianHold = units.Get("Boh", phase: Phase.Fall).Hold();
        var germanSupport = units.Get("Mun", phase: Phase.Fall).Support(units.Get("Mun"), "Tyr");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        austrianHold.Status.Should().Be(OrderStatus.Success);
        germanSupport.Status.Should().Be(OrderStatus.Success);

        presentBoard.Next().ShouldHaveUnits(
            [
                (Nation.Germany, UnitType.Army, "Mun", false),
                (Nation.Austria, UnitType.Army, "Boh", false),
            ]);
        pastBoard.Next(timeline: 2).ShouldHaveUnits(
            [
                (Nation.Germany, UnitType.Army, "Tyr", false),
                (Nation.Austria, UnitType.Army, "Boh", false),
            ]);
    }

    [Fact(DisplayName = "B.03. Support to past move repelled by hold dislodges")]
    public void MDATC_B_3()
    {
        // Arrange
        var world = new World();

        var presentBoard = world.AddBoard(phase: Phase.Fall);
        var pastBoard = world.AddBoard();

        List<Unit> units =
            [
                .. pastBoard.AddUnits(
                    [
                        (Nation.Germany, UnitType.Army, "Mun"),
                        (Nation.Austria, UnitType.Army, "Tyr"),
                    ]),
                .. presentBoard.AddUnits(
                    [
                        (Nation.Germany, UnitType.Army, "Mun"),
                        (Nation.Austria, UnitType.Army, "Tyr"),
                    ]),
            ];

        units.Get("Mun").Move("Tyr", status: OrderStatus.Failure);
        units.Get("Tyr").Hold(status: OrderStatus.Success);

        var austrianHold = units.Get("Tyr", phase: Phase.Fall).Hold();
        var germanSupport = units.Get("Mun", phase: Phase.Fall).Support(units.Get("Mun"), "Tyr");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        austrianHold.Status.Should().Be(OrderStatus.Success);
        germanSupport.Status.Should().Be(OrderStatus.Success);

        presentBoard.ShouldHaveUnits(
            [
                (Nation.Germany, UnitType.Army, "Mun", false),
                (Nation.Austria, UnitType.Army, "Tyr", false),
            ]);
        pastBoard.ShouldHaveUnits(
            [
                (Nation.Germany, UnitType.Army, "Mun", false),
                (Nation.Austria, UnitType.Army, "Tyr", true),
            ]);
        presentBoard.ShouldNotHaveNextBoard();
        pastBoard.ShouldNotHaveNextBoard(timeline: 2);
    }

    [Fact(DisplayName = "B.04. Failed move does not fork timeline")]
    public void MDATC_B_4()
    {
        // Arrange
        var world = new World();

        var presentBoard = world.AddBoard(phase: Phase.Fall);
        var pastBoard = world.AddBoard();

        List<Unit> units =
            [
                .. pastBoard.AddUnits(
                    [
                        (Nation.Germany, UnitType.Army, "Mun"),
                        (Nation.Austria, UnitType.Army, "Tyr"),
                    ]),
                .. presentBoard.AddUnits(
                    [
                        (Nation.Germany, UnitType.Army, "Mun"),
                        (Nation.Austria, UnitType.Army, "Tyr"),
                    ]),
            ];

        units.Get("Mun").Hold(status: OrderStatus.Success);
        units.Get("Tyr").Hold(status: OrderStatus.Success);

        var austrianHold = units.Get("Tyr", phase: Phase.Fall).Hold();
        var germanMove = units.Get("Mun", phase: Phase.Fall).Move("Tyr");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        austrianHold.Status.Should().Be(OrderStatus.Success);
        germanMove.Status.Should().Be(OrderStatus.Failure);

        presentBoard.Next().ShouldHaveUnits(
            [
                (Nation.Germany, UnitType.Army, "Mun", false),
                (Nation.Austria, UnitType.Army, "Tyr", false),
            ]);
        pastBoard.ShouldNotHaveNextBoard(timeline: 2);
    }

    [Fact(DisplayName = "B.05. Superfluous support does not fork timeline")]
    public void MDATC_B_5()
    {
        // Arrange
        var world = new World();

        var presentBoard = world.AddBoard(phase: Phase.Fall);
        var pastBoard = world.AddBoard();

        List<Unit> units =
            [
                .. pastBoard.AddUnits(
                    [
                        (Nation.Germany, UnitType.Army, "Mun"),
                        (Nation.Germany, UnitType.Army, "Boh"),
                    ]),
                .. presentBoard.AddUnits(
                    [
                        (Nation.Germany, UnitType.Army, "Tyr"),
                        (Nation.Germany, UnitType.Army, "Boh"),
                    ]),
            ];

        units.Get("Mun").Move("Tyr", status: OrderStatus.Success);
        units.Get("Boh").Hold(status: OrderStatus.Success);

        var hold = units.Get("Tyr", phase: Phase.Fall).Hold();
        var support = units.Get("Boh", phase: Phase.Fall).Support(units.Get("Mun"), "Tyr");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        hold.Status.Should().Be(OrderStatus.Success);
        support.Status.Should().Be(OrderStatus.Success);

        presentBoard.Next().ShouldHaveUnits(
            [
                (Nation.Germany, UnitType.Army, "Tyr", false),
                (Nation.Germany, UnitType.Army, "Boh", false),
            ]);
        pastBoard.ShouldNotHaveNextBoard(timeline: 2);
    }

    [Fact(DisplayName = "B.06. Cross-timeline support does not fork head")]
    public void MDATC_B_6()
    {
        // Arrange
        var world = new World();

        var topBoard = world.AddBoard();
        var bottomBoard = world.AddBoard(timeline: 2);

        List<Unit> units =
            [
                .. topBoard.AddUnits([(Nation.Germany, UnitType.Army, "Mun")]),
                .. bottomBoard.AddUnits([(Nation.Germany, UnitType.Army, "Mun")]),
            ];

        var move = units.Get("Mun", timeline: 2).Move("Tyr", timeline: 2);
        var support = units.Get("Mun").Support(units.Get("Mun", timeline: 2), "Tyr", timeline: 2);

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        move.Status.Should().Be(OrderStatus.Success);
        support.Status.Should().Be(OrderStatus.Success);

        topBoard.Next().ShouldHaveUnits([(Nation.Germany, UnitType.Army, "Mun", false)]);
        bottomBoard.Next().ShouldHaveUnits([(Nation.Germany, UnitType.Army, "Tyr", false)]);
        topBoard.ShouldNotHaveNextBoard(timeline: 3);
        bottomBoard.ShouldNotHaveNextBoard(timeline: 3);
    }

    [Fact(DisplayName = "B.07. Cutting cross-timeline support forks timeline")]
    public void MDATC_B_7()
    {
        // Arrange
        var world = new World();

        var presentTopBoard = world.AddBoard(phase: Phase.Fall);
        var presentBottomBoard = world.AddBoard(timeline: 2, phase: Phase.Fall);
        var pastTopBoard = world.AddBoard();
        var pastBottomBoard = world.AddBoard(timeline: 2);

        List<Unit> units =
            [
                .. presentTopBoard.AddUnits(
                    [
                        (Nation.Germany, UnitType.Army, "Mun"),
                        (Nation.Austria, UnitType.Army, "Boh"),
                    ]),
                .. presentBottomBoard.AddUnits(
                    [
                        (Nation.Germany, UnitType.Army, "Tyr"),
                        (Nation.Austria, UnitType.Army, "Boh"),
                    ]),
                .. pastTopBoard.AddUnits(
                    [
                        (Nation.Germany, UnitType.Army, "Mun"),
                        (Nation.Austria, UnitType.Army, "Boh"),
                    ]),
                .. pastBottomBoard.AddUnits(
                    [
                        (Nation.Germany, UnitType.Army, "Mun"),
                        (Nation.Austria, UnitType.Army, "Boh"),
                    ]),
            ];

        units.Get("Mun", timeline: 2).Move("Tyr", timeline: 2, status: OrderStatus.Success);
        units.Get("Mun").Support(units.Get("Mun", timeline: 2), "Tyr", timeline: 2, status: OrderStatus.Success);
        units.Get("Boh").Hold(OrderStatus.Success);
        units.Get("Boh", timeline: 2).Move("Tyr", timeline: 2, status: OrderStatus.Failure);

        var austrianMove = units.Get("Boh", phase: Phase.Fall).Move("Mun");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        austrianMove.Status.Should().Be(OrderStatus.Failure);

        presentTopBoard.Next().ShouldHaveUnits(
            [
                (Nation.Germany, UnitType.Army, "Mun", false),
                (Nation.Austria, UnitType.Army, "Boh", false),
            ]);
        presentBottomBoard.Next().ShouldHaveUnits(
            [
                (Nation.Germany, UnitType.Army, "Tyr", false),
                (Nation.Austria, UnitType.Army, "Boh", false),
            ]);
        pastBottomBoard.Next(timeline: 3).ShouldHaveUnits(
            [
                (Nation.Germany, UnitType.Army, "Mun", false),
                (Nation.Austria, UnitType.Army, "Boh", false),
            ]);
    }

    [Fact(DisplayName = "B.08. Multiple powers move to same board")]
    public void MDATC_B_8()
    {
        // Arrange
        var world = new World();

        var presentBoard = world.AddBoard(phase: Phase.Fall);
        var pastBoard = world.AddBoard();

        List<Unit> units =
            [
                .. pastBoard.AddUnits(
                    [
                        (Nation.England, UnitType.Fleet, "Lon"),
                        (Nation.Germany, UnitType.Army, "Ber"),
                        (Nation.Russia, UnitType.Army, "Mos"),
                        (Nation.Turkey, UnitType.Army, "Con"),
                        (Nation.Austria, UnitType.Army, "Vie"),
                        (Nation.Italy, UnitType.Fleet, "Rom"),
                        (Nation.France, UnitType.Army, "Par"),
                    ]),
                .. presentBoard.AddUnits(
                    [
                        (Nation.England, UnitType.Fleet, "Lon"),
                        (Nation.Germany, UnitType.Army, "Ber"),
                        (Nation.Russia, UnitType.Army, "Mos"),
                        (Nation.Turkey, UnitType.Army, "Con"),
                        (Nation.Austria, UnitType.Army, "Vie"),
                        (Nation.Italy, UnitType.Fleet, "Rom"),
                        (Nation.France, UnitType.Army, "Par"),
                    ]),
            ];

        units.Get("Lon").Hold(status: OrderStatus.Success);
        units.Get("Ber").Hold(status: OrderStatus.Success);
        units.Get("Mos").Hold(status: OrderStatus.Success);
        units.Get("Con").Hold(status: OrderStatus.Success);
        units.Get("Vie").Hold(status: OrderStatus.Success);
        units.Get("Rom").Hold(status: OrderStatus.Success);
        units.Get("Par").Hold(status: OrderStatus.Success);

        var englishMove = units.Get("Lon", phase: Phase.Fall).Move("Wal");
        var germanMove = units.Get("Ber", phase: Phase.Fall).Move("Kie");
        var russianMove = units.Get("Mos", phase: Phase.Fall).Move("Stp");
        var turkishMove = units.Get("Con", phase: Phase.Fall).Move("Bul");
        var austrianMove = units.Get("Vie", phase: Phase.Fall).Move("Bud");
        var italianMove = units.Get("Rom", phase: Phase.Fall).Move("Tus");
        var frenchMove = units.Get("Par", phase: Phase.Fall).Move("Pic");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        germanMove.Status.Should().Be(OrderStatus.Success);
        russianMove.Status.Should().Be(OrderStatus.Success);
        turkishMove.Status.Should().Be(OrderStatus.Success);
        austrianMove.Status.Should().Be(OrderStatus.Success);
        italianMove.Status.Should().Be(OrderStatus.Success);
        frenchMove.Status.Should().Be(OrderStatus.Success);

        presentBoard.Next().ShouldHaveUnits([]);
        pastBoard.Next(timeline: 2).ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "Lon", false),
                (Nation.England, UnitType.Fleet, "Wal", false),
                (Nation.Germany, UnitType.Army, "Ber", false),
                (Nation.Germany, UnitType.Army, "Kie", false),
                (Nation.Russia, UnitType.Army, "Mos", false),
                (Nation.Russia, UnitType.Army, "Stp", false),
                (Nation.Turkey, UnitType.Army, "Con",false),
                (Nation.Turkey, UnitType.Army, "Bul", false),
                (Nation.Austria, UnitType.Army, "Vie", false),
                (Nation.Austria, UnitType.Army, "Bud", false),
                (Nation.Italy, UnitType.Fleet, "Rom", false),
                (Nation.Italy, UnitType.Fleet, "Tus", false),
                (Nation.France, UnitType.Army, "Par", false),
                (Nation.France, UnitType.Army, "Pic", false),
            ]);
        pastBoard.ShouldNotHaveNextBoard(timeline: 3);
    }

    [Fact(DisplayName = "B.09. Grandfather paradox resolved")]
    public void MDATC_B_9()
    {
        // Arrange
        var world = new World();

        var presentBoard = world.AddBoard(phase: Phase.Fall);
        var pastBoard = world.AddBoard();

        List<Unit> units =
            [
                .. pastBoard.AddUnits(
                    [
                        (Nation.Russia, UnitType.Army, "Mos"),
                        (Nation.Russia, UnitType.Army, "War"),
                    ]),
                .. presentBoard.AddUnits(
                    [
                        (Nation.Russia, UnitType.Army, "Mos"),
                        (Nation.Russia, UnitType.Army, "War"),
                    ]),
            ];

        units.Get("Mos").Hold(status: OrderStatus.Success);
        units.Get("War").Hold(status: OrderStatus.Success);

        var move = units.Get("Mos", phase: Phase.Fall).Move("Mos");
        var support = units.Get("War", phase: Phase.Fall).Support(units.Get("Mos", phase: Phase.Fall), "Mos");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        move.Status.Should().Be(OrderStatus.Failure);
        support.Status.Should().Be(OrderStatus.Failure);

        presentBoard.Next().ShouldHaveUnits(
            [
                (Nation.Russia, UnitType.Army, "Mos", false),
                (Nation.Russia, UnitType.Army, "War", false),
            ]);
        pastBoard.ShouldNotHaveNextBoard(timeline: 2);
    }

    [Fact(DisplayName = "B.10. Vanishing in a puff of logic")]
    public void MDATC_B_10()
    {
        // It should never be possible for this situation to arise in an actual game. If a unit exists on a board and
        // has no past self, then it must have been built in the intervening winter board, and thus moving back in time
        // would create a different base for the resolution of that winter board and fork the timeline. However, if
        // there is indeed an edge case we've missed where a unit can travel to its own past without bouncing or
        // forking a timeline, it seems thematically appropriate for it to ouroboros itself out of existence.

        // Arrange
        var world = new World();

        var presentBoard = world.AddBoard(phase: Phase.Fall);
        var pastBoard = world.AddBoard();

        var units = presentBoard.AddUnits([(Nation.France, UnitType.Army, "Bel")]);

        var move = units.Get("Bel", phase: Phase.Fall).Move("Bel");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        move.Status.Should().Be(OrderStatus.Success);

        presentBoard.Next().ShouldHaveUnits([]);
        pastBoard.ShouldNotHaveNextBoard(timeline: 2);
    }
}
