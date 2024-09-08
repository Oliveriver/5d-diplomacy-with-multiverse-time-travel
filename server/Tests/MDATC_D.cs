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
public class MDATC_D : AdjudicationTestBase
{
    [Fact(DisplayName = "D.01. Bootstrap paradox resolved")]
    public void MDATC_D_1()
    {
        // Arrange
        var world = new World();

        var presentBoard = world.AddBoard(year: 1902);
        var pastBoard1 = world.AddBoard(phase: Phase.Winter);
        var pastBoard2 = world.AddBoard(phase: Phase.Fall);

        pastBoard1.AddCentres([(Nation.France, "Mar")]);
        var units = presentBoard.AddUnits([(Nation.France, UnitType.Army, "Mar")]);

        pastBoard1.Build(Nation.France, UnitType.Army, "Mar", status: OrderStatus.Success);

        var order = units.Get("Mar", year: 1902).Move("Mar", phase: Phase.Fall);

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Success);

        presentBoard.Next().ShouldHaveUnits([]);
        pastBoard2.Next(timeline: 2).ShouldHaveUnits([(Nation.France, UnitType.Army, "Mar", false)]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "D.02. Convoy kidnapping across time")]
    public void MDATC_D_2()
    {
        // Arrange
        var world = new World();

        var presentBoard = world.AddBoard(phase: Phase.Fall);
        var pastBoard = world.AddBoard();

        List<Unit> units =
            [
                .. pastBoard.AddUnits(
                    [
                        (Nation.England, UnitType.Fleet, "NTH"),
                        (Nation.France, UnitType.Army, "Bel"),
                        (Nation.Germany, UnitType.Army, "Hol"),
                    ]),
                .. presentBoard.AddUnits(
                    [
                        (Nation.England, UnitType.Fleet, "NTH"),
                        (Nation.France, UnitType.Army, "Bel"),
                        (Nation.Germany, UnitType.Army, "Hol"),
                    ]),
            ];

        units.Get("NTH").Hold(status: OrderStatus.Success);
        units.Get("Bel").Move("Hol", status: OrderStatus.Failure);
        units.Get("Hol").Move("Bel", status: OrderStatus.Failure);

        var englishConvoy = units.Get("NTH", phase: Phase.Fall).Convoy(units.Get("Bel"), "Hol");
        var frenchHold = units.Get("Bel", phase: Phase.Fall).Hold();
        var germanHold = units.Get("Hol", phase: Phase.Fall).Hold();

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishConvoy.Status.Should().Be(OrderStatus.Success);
        frenchHold.Status.Should().Be(OrderStatus.Success);
        germanHold.Status.Should().Be(OrderStatus.Success);

        presentBoard.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH", false),
                (Nation.France, UnitType.Army, "Bel", false),
                (Nation.Germany, UnitType.Army, "Hol", false),
            ]);
        pastBoard.Next(timeline: 2).ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH", false),
                (Nation.Germany, UnitType.Army, "Bel", false),
                (Nation.France, UnitType.Army, "Hol", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "D.03. Jurassic convoy")]
    public void MDATC_D_3()
    {
        // Life, uh...finds a way.

        // Arrange
        var world = new World();

        var presentBoard = world.AddBoard(year: 1902, phase: Phase.Fall);
        var pastBoard1 = world.AddBoard(year: 1902);
        var pastBoard2 = world.AddBoard(phase: Phase.Winter);
        var pastBoard3 = world.AddBoard(phase: Phase.Fall);
        var pastBoard4 = world.AddBoard();

        List<Unit> units =
            [
                .. pastBoard4.AddUnits(
                    [
                        (Nation.England, UnitType.Army, "Lon"),
                        (Nation.England, UnitType.Fleet, "NTH"),
                    ]),
                .. pastBoard3.AddUnits(
                    [
                        (Nation.England, UnitType.Army, "Lon"),
                        (Nation.England, UnitType.Fleet, "NTH"),
                    ]),
                .. pastBoard2.AddUnits(
                    [
                        (Nation.England, UnitType.Army, "Lon"),
                        (Nation.England, UnitType.Fleet, "NTH"),
                    ]),
                .. pastBoard1.AddUnits(
                    [
                        (Nation.England, UnitType.Army, "Lon"),
                        (Nation.England, UnitType.Fleet, "NTH"),
                    ]),
                .. presentBoard.AddUnits(
                    [
                        (Nation.England, UnitType.Army, "Lon"),
                        (Nation.England, UnitType.Fleet, "NTH"),
                    ]),
            ];

        units.Get("Lon").Hold(status: OrderStatus.Success);
        units.Get("NTH").Hold(status: OrderStatus.Success);
        units.Get("Lon", phase: Phase.Fall).Hold(status: OrderStatus.Success);
        units.Get("Lon", year: 1902).Hold(status: OrderStatus.Success);
        var convoy1 = units.Get("NTH", phase: Phase.Fall).Convoy(units.Get("Lon", year: 1902, phase: Phase.Fall), "Nwy", status: OrderStatus.Invalid);
        var convoy2 = units.Get("NTH", year: 1902).Convoy(units.Get("Lon", year: 1902, phase: Phase.Fall), "Nwy", status: OrderStatus.Invalid);

        var move = units.Get("Lon", year: 1902, phase: Phase.Fall).Move("Nwy");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        move.Status.Should().Be(OrderStatus.Success);
        convoy1.Status.Should().Be(OrderStatus.Success);
        convoy2.Status.Should().Be(OrderStatus.Success);

        presentBoard.Next().ShouldHaveUnits([(Nation.England, UnitType.Fleet, "NTH", false)]);
        pastBoard4.Next(timeline: 2).ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH", false),
                (Nation.England, UnitType.Army, "Lon", false),
                (Nation.England, UnitType.Army, "Nwy", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }
}
