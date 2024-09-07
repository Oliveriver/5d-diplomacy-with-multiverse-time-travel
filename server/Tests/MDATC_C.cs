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
public class MDATC_C : AdjudicationTestBase
{
    [Fact(DisplayName = "C.01. Retreats to the past forbidden")]
    public void MDATC_C_1()
    {
        // Arrange
        var world = new World();

        var presentBoard = world.AddBoard(phase: Phase.Fall);
        var pastBoard = world.AddBoard();

        var units = presentBoard.AddUnits([(Nation.England, UnitType.Army, "Lon")]);

        units.Get("Lon", phase: Phase.Fall).MustRetreat = true;

        var order = units.Get("Lon", phase: Phase.Fall).Move("Lon");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Invalid);

        presentBoard.Next().ShouldHaveUnits([]);
        pastBoard.ShouldNotHaveNextBoard(timeline: 2);
    }

    [Fact(DisplayName = "C.02. Retreats to parallel timelines forbidden")]
    public void MDATC_C_2()
    {
        // Arrange
        var world = new World();

        var topBoard = world.AddBoard();
        var bottomBoard = world.AddBoard(timeline: 2);

        var units = topBoard.AddUnits([(Nation.England, UnitType.Army, "Lon")]);

        units.Get("Lon").MustRetreat = true;

        var order = units.Get("Lon").Move("Lon", timeline: 2);

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Invalid);

        topBoard.Next().ShouldHaveUnits([]);
        bottomBoard.Next().ShouldHaveUnits([]);
    }

    [Fact(DisplayName = "C.03. No orders elsewhere in the multiverse during retreats")]
    public void MDATC_C_3()
    {
        // Arrange
        var world = new World();

        var topBoard = world.AddBoard();
        var bottomBoard = world.AddBoard(timeline: 2);

        List<Unit> units =
            [
                .. topBoard.AddUnits([(Nation.England, UnitType.Army, "Lon")]),
                .. bottomBoard.AddUnits([(Nation.Italy, UnitType.Fleet, "ADR")]),
            ];

        units.Get("Lon").MustRetreat = true;

        var englishMove = units.Get("Lon").Move("Wal");
        var italianMove = units.Get("ADR", timeline: 2).Move("Alb");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Retreat);
        italianMove.Status.Should().Be(OrderStatus.Invalid);

        topBoard.Next().ShouldHaveUnits([(Nation.England, UnitType.Army, "Wal", false)]);
        bottomBoard.Next().ShouldHaveUnits([(Nation.Italy, UnitType.Fleet, "ADR", false)]);
    }

    [Fact(DisplayName = "C0.4. Retreat in past to new location forks timeline")]
    public void MDATC_C_4()
    {
        // A continuation of MDATC B 3 to match the original MDATC's scenario.

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

        units.Get("Mun").Move("Tyr", status: OrderStatus.Success);
        units.Get("Tyr").Hold(status: OrderStatus.Failure);
        units.Get("Tyr", phase: Phase.Fall).Hold(status: OrderStatus.Success);
        units.Get("Mun", phase: Phase.Fall).Support(units.Get("Mun"), "Tyr", status: OrderStatus.Success);
        units.Get("Tyr").MustRetreat = true;

        var order = units.Get("Tyr").Move("Boh");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Retreat);

        presentBoard.Next().ShouldHaveUnits(
            [
                (Nation.Germany, UnitType.Army, "Mun", false),
                (Nation.Austria, UnitType.Army, "Tyr", false),
            ]);
        pastBoard.Next(timeline: 2).ShouldHaveUnits(
            [
                (Nation.Germany, UnitType.Army, "Tyr", false),
                (Nation.Austria, UnitType.Army, "Boh", false),
            ]);
    }
}
