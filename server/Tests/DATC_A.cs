using Adjudication;
using Entities;
using Enums;
using FluentAssertions;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Tests;

// Diplomacy Adjudicator Test Cases, Lucas B. Kruijswijk
// https://boardgamegeek.com/filepage/274846/datc-diplomacy-adjudicator-test-cases

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
public class DATC_A : AdjudicationTestBase
{
    [Fact(DisplayName = "DATC A.01. Moving to an area that is not a neighbour")]
    public void DATC_A_1()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();
        var units = board.AddUnits([(Nation.England, UnitType.Fleet, "NTH")]);

        var order = units.Get("NTH").Move("Pic");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits([(Nation.England, UnitType.Fleet, "NTH", false)]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "DATC A.02. Move army to sea")]
    public void DATC_A_2()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();
        var units = board.AddUnits([(Nation.England, UnitType.Army, "Lvp")]);

        var order = units.Get("Lvp").Move("IRI");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits([(Nation.England, UnitType.Army, "Lvp", false)]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "DATC A.03. Move fleet to land")]
    public void DATC_A_3()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();
        var units = board.AddUnits([(Nation.Germany, UnitType.Fleet, "Kie")]);

        var order = units.Get("Kie").Move("Mun");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits([(Nation.Germany, UnitType.Fleet, "Kie", false)]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "DATC A.04. Move to own sector")]
    public void DATC_A_4()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();
        var units = board.AddUnits([(Nation.Germany, UnitType.Fleet, "Kie")]);

        var order = units.Get("Kie").Move("Kie");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits([(Nation.Germany, UnitType.Fleet, "Kie", false)]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "DATC A.05. Move to own sector with convoy")]
    public void DATC_A_5()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH"),
                (Nation.England, UnitType.Army, "Yor"),
                (Nation.England, UnitType.Army, "Lvp"),
                (Nation.Germany, UnitType.Fleet, "Lon"),
                (Nation.Germany, UnitType.Army, "Wal"),
            ]);

        var englishMove = units.Get("Yor").Move("Yor");
        var englishConvoy = units.Get("NTH").Convoy(units.Get("Yor"), "Yor");
        var englishSupport = units.Get("Lvp").Support(units.Get("Yor"), "Yor");
        var germanMove = units.Get("Lon").Move("Yor");
        var germanSupport = units.Get("Wal").Support(units.Get("Lon"), "Yor");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Invalid);
        englishConvoy.Status.Should().Be(OrderStatus.Invalid);
        englishSupport.Status.Should().Be(OrderStatus.Invalid);
        germanMove.Status.Should().Be(OrderStatus.Success);
        germanSupport.Status.Should().Be(OrderStatus.Success);

        board.ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH", false),
                (Nation.England, UnitType.Army, "Yor", true),
                (Nation.England, UnitType.Army, "Lvp", false),
                (Nation.Germany, UnitType.Fleet, "Lon", false),
                (Nation.Germany, UnitType.Army, "Wal", false),
            ]);
        board.ShouldNotHaveNextBoard();

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "DATC A.06. Ordering a unit of another country", Skip = "Not applicable")]
    public void DATC_A_6()
    {
        // Order ownership is controlled by the client. Once an order reaches the server, it retains no ownership
        // information other than the acting unit. So this test case is not valid for the 5D Diplomacy adjudicator.
    }

    [Fact(DisplayName = "DATC A.07. Only armies can be convoyed")]
    public void DATC_A_7()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "Lon"),
                (Nation.England, UnitType.Fleet, "NTH"),
            ]);

        var move = units.Get("Lon").Move("Bel");
        var convoy = units.Get("NTH").Convoy(units.Get("Lon"), "Bel");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        move.Status.Should().Be(OrderStatus.Invalid);
        convoy.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "Lon", false),
                (Nation.England, UnitType.Fleet, "NTH", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "DATC A.08. Support to hold yourself is not possible")]
    public void DATC_A_8()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Italy, UnitType.Army, "Ven"),
                (Nation.Italy, UnitType.Army, "Tyr"),
                (Nation.Austria, UnitType.Fleet, "Tri"),
            ]);

        var italianMove = units.Get("Ven").Move("Tri");
        var italianSupport = units.Get("Tyr").Support(units.Get("Ven"), "Tri");
        var austrianSupport = units.Get("Tri").Support(units.Get("Tri"), "Tri");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        italianMove.Status.Should().Be(OrderStatus.Success);
        italianSupport.Status.Should().Be(OrderStatus.Success);
        austrianSupport.Status.Should().Be(OrderStatus.Invalid);

        board.ShouldHaveUnits(
            [
                (Nation.Italy, UnitType.Army, "Ven", false),
                (Nation.Italy, UnitType.Army, "Tyr", false),
                (Nation.Austria, UnitType.Fleet, "Tri", true),
            ]);
        board.ShouldNotHaveNextBoard();

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "DATC A.09. Fleets must follow coast if not on sea")]
    public void DATC_A_9()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();
        var units = board.AddUnits([(Nation.Italy, UnitType.Fleet, "Rom")]);

        var order = units.Get("Rom").Move("Ven");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits([(Nation.Italy, UnitType.Fleet, "Rom", false)]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "DATC A.10. Support on unreachable destination not possible")]
    public void DATC_A_10()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();
        var units = board.AddUnits(
            [
                (Nation.Austria, UnitType.Army, "Ven"),
                (Nation.Italy, UnitType.Army, "Apu"),
                (Nation.Italy, UnitType.Fleet, "Rom"),
            ]);

        var austrianHold = units.Get("Ven").Hold();
        var italianMove = units.Get("Apu").Move("Ven");
        var italianSupport = units.Get("Rom").Support(units.Get("Apu"), "Ven");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        austrianHold.Status.Should().Be(OrderStatus.Success);
        italianMove.Status.Should().Be(OrderStatus.Failure);
        italianSupport.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits(
            [
                (Nation.Austria, UnitType.Army, "Ven", false),
                (Nation.Italy, UnitType.Army, "Apu", false),
                (Nation.Italy, UnitType.Fleet, "Rom", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "DATC A.11. Simple bounce")]
    public void DATC_A_11()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();
        var units = board.AddUnits(
            [
                (Nation.Austria, UnitType.Army, "Vie"),
                (Nation.Italy, UnitType.Army, "Ven"),
            ]);

        var austrianMove = units.Get("Vie").Move("Tyr");
        var italianMove = units.Get("Ven").Move("Tyr");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        austrianMove.Status.Should().Be(OrderStatus.Failure);
        italianMove.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.Austria, UnitType.Army, "Vie", false),
                (Nation.Italy, UnitType.Army, "Ven", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "DATC A.12. Bounce of three units")]
    public void DATC_A_12()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();
        var units = board.AddUnits(
            [
                (Nation.Austria, UnitType.Army, "Vie"),
                (Nation.Germany, UnitType.Army, "Mun"),
                (Nation.Italy, UnitType.Army, "Ven"),
            ]);

        var austrianMove = units.Get("Vie").Move("Tyr");
        var germanMove = units.Get("Mun").Move("Tyr");
        var italianMove = units.Get("Ven").Move("Tyr");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        austrianMove.Status.Should().Be(OrderStatus.Failure);
        germanMove.Status.Should().Be(OrderStatus.Failure);
        italianMove.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.Austria, UnitType.Army, "Vie", false),
                (Nation.Germany, UnitType.Army, "Mun", false),
                (Nation.Italy, UnitType.Army, "Ven", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }
}
