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
public class MDATC_A : AdjudicationTestBase
{
    [Fact(DisplayName = "A.01. Move to same region with loose adjacencies")]
    public void MDATC_A_1()
    {
        // Arrange
        var world = new World();

        var topBoard = world.AddBoard();
        var bottomBoard = world.AddBoard(timeline: 2);

        var units = topBoard.AddUnits([(Nation.England, UnitType.Army, "Lon")]);

        var order = units.Get("Lon").Move("Lon", timeline: 2);

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Success);

        topBoard.Next().ShouldHaveUnits([]);
        bottomBoard.Next().ShouldHaveUnits([(Nation.England, UnitType.Army, "Lon", false)]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "A.02. Move to same region with strict adjacencies")]
    public void MDATC_A_2()
    {
        // Arrange
        var world = new World();

        var topBoard = world.AddBoard();
        var bottomBoard = world.AddBoard(timeline: 2);

        var units = topBoard.AddUnits([(Nation.Turkey, UnitType.Fleet, "Smy")]);

        var order = units.Get("Smy").Move("Smy", timeline: 2);

        // Act
        new Adjudicator(world, true, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Success);

        topBoard.Next().ShouldHaveUnits([]);
        bottomBoard.Next().ShouldHaveUnits([(Nation.Turkey, UnitType.Fleet, "Smy", false)]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "A.03. Move to neighbouring region with loose adjacencies")]
    public void MDATC_A_3()
    {
        // Arrange
        var world = new World();

        var topBoard = world.AddBoard();
        var bottomBoard = world.AddBoard(timeline: 2);

        var units = bottomBoard.AddUnits([(Nation.Austria, UnitType.Army, "Vie")]);

        var order = units.Get("Vie", timeline: 2).Move("Bud");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Success);

        topBoard.Next().ShouldHaveUnits([(Nation.Austria, UnitType.Army, "Bud", false)]);
        bottomBoard.Next().ShouldHaveUnits([]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "A.04. Move to neighbouring region with strict adjacencies")]
    public void MDATC_A_4()
    {
        // Arrange
        var world = new World();

        var topBoard = world.AddBoard();
        var bottomBoard = world.AddBoard(timeline: 2);

        var units = bottomBoard.AddUnits([(Nation.Italy, UnitType.Fleet, "Tun")]);

        var order = units.Get("Tun", timeline: 2).Move("WES");

        // Act
        new Adjudicator(world, true, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Invalid);

        topBoard.Next().ShouldHaveUnits([]);
        bottomBoard.Next().ShouldHaveUnits([(Nation.Italy, UnitType.Fleet, "Tun", false)]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "A.05. Move to non-neighbouring region")]
    public void MDATC_A_5()
    {
        // Arrange
        var world = new World();

        var topBoard = world.AddBoard();
        var bottomBoard = world.AddBoard(timeline: 2);

        var units = topBoard.AddUnits([(Nation.England, UnitType.Fleet, "Lvp")]);

        var order = units.Get("Lvp").Move("Edi", timeline: 2);

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Invalid);

        topBoard.Next().ShouldHaveUnits([(Nation.England, UnitType.Fleet, "Lvp", false)]);
        bottomBoard.Next().ShouldHaveUnits([]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "A.06. Can't cross coasts across time")]
    public void MDATC_A_6()
    {
        // Arrange
        var world = new World();

        var topBoard = world.AddBoard();
        var bottomBoard = world.AddBoard(timeline: 2);

        var units = bottomBoard.AddUnits([(Nation.France, UnitType.Fleet, "Spa_S")]);

        var order = units.Get("Spa_S", timeline: 2).Move("Spa_N");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Invalid);

        topBoard.Next().ShouldHaveUnits([]);
        bottomBoard.Next().ShouldHaveUnits([(Nation.France, UnitType.Fleet, "Spa_S", false)]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "A.07. No diagonal board movement")]
    public void MDATC_A_7()
    {
        // Arrange
        var world = new World();

        var topBoard = world.AddBoard(phase: Phase.Fall);
        var bottomBoard = world.AddBoard(timeline: 2);

        var units = topBoard.AddUnits([(Nation.Germany, UnitType.Army, "Mun")]);

        var order = units.Get("Mun", phase: Phase.Fall).Move("Mun", timeline: 2);

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Invalid);

        topBoard.Next().ShouldHaveUnits([(Nation.Germany, UnitType.Army, "Mun", false)]);
        bottomBoard.Next().ShouldHaveUnits([]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "A.08. Must be adjacent timeline without convoy")]
    public void MDATC_A_8()
    {
        // Arrange
        var world = new World();

        var topBoard = world.AddBoard();
        var middleBoard = world.AddBoard(timeline: 2);
        var bottomBoard = world.AddBoard(timeline: 3);

        var units = topBoard.AddUnits([(Nation.England, UnitType.Army, "Lon")]);

        var order = units.Get("Lon").Move("Lon", timeline: 3);

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Invalid);

        topBoard.Next().ShouldHaveUnits([(Nation.England, UnitType.Army, "Lon", false)]);
        middleBoard.Next().ShouldHaveUnits([]);
        bottomBoard.Next().ShouldHaveUnits([]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "A.09. Must be immediate past major board without convoy")]
    public void MDATC_A_9()
    {
        // Arrange
        var world = new World();

        var presentBoard = world.AddBoard(year: 1902);
        var pastBoard1 = world.AddBoard(phase: Phase.Winter);
        var pastBoard2 = world.AddBoard(phase: Phase.Fall);
        var pastBoard3 = world.AddBoard();

        var units = presentBoard.AddUnits(
            [
                (Nation.England, UnitType.Army, "Lon"),
                (Nation.France, UnitType.Army, "Par"),
            ]);

        var englishMove = units.Get("Lon", year: 1902).Move("Lon");
        var frenchMove = units.Get("Par", year: 1902).Move("Gas", phase: Phase.Fall);

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Invalid);
        frenchMove.Status.Should().Be(OrderStatus.Success);

        presentBoard.Next().ShouldHaveUnits([(Nation.England, UnitType.Army, "Lon", false)]);
        pastBoard1.ShouldNotHaveNextBoard(timeline: 2);
        pastBoard2.Next(timeline: 2).ShouldHaveUnits([(Nation.France, UnitType.Army, "Gas", false)]);
        pastBoard3.ShouldNotHaveNextBoard(timeline: 2);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "A.10. No move to winter board")]
    public void MDATC_A_10()
    {
        // Arrange
        var world = new World();

        var presentBoard = world.AddBoard(year: 1902);
        var pastBoard = world.AddBoard(phase: Phase.Winter);

        presentBoard.AddCentres([(Nation.Russia, "Sev")]);
        pastBoard.AddCentres([(Nation.Russia, "Sev")]);

        List<Unit> units =
            [
                .. pastBoard.AddUnits([(Nation.Russia, UnitType.Fleet, "Sev")]),
                .. presentBoard.AddUnits([(Nation.Russia, UnitType.Fleet, "Sev")]
            )];

        var order = units.Get("Sev", year: 1902).Move("Sev", phase: Phase.Winter);

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Invalid);

        presentBoard.Next().ShouldHaveUnits([(Nation.Russia, UnitType.Fleet, "Sev", false)]);
        pastBoard.ShouldNotHaveNextBoard(timeline: 2);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "A.11. Simultaneous adjustment and movement phases advance together")]
    public void MDATC_A_11()
    {
        // Arrange
        var world = new World();

        var topBoard = world.AddBoard(year: 1902);
        var bottomBoard = world.AddBoard(timeline: 2, phase: Phase.Winter);

        bottomBoard.AddCentres([(Nation.Germany, "Ber")]);

        var units = topBoard.AddUnits([(Nation.England, UnitType.Fleet, "Edi")]);

        var englishMove = units.Get("Edi", year: 1902).Move("Cly", year: 1902);
        var germanBuild = bottomBoard.Build(Nation.Germany, UnitType.Army, "Ber");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        germanBuild.Status.Should().Be(OrderStatus.Success);

        topBoard.Next().ShouldHaveUnits([(Nation.England, UnitType.Fleet, "Cly", false)]);
        bottomBoard.Next().ShouldHaveUnits([(Nation.Germany, UnitType.Army, "Ber", false)]);

        world.ShouldHaveAllOrdersResolved();
    }
}
