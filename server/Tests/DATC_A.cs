using Adjudication;
using Entities;
using Enums;
using FluentAssertions;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Tests;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
public class DATC_A : AdjudicationTestBase
{
    [Fact(DisplayName = "A.1. Moving to an area that is not a neighbour")]
    public void DATC_A_1()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();
        var units = board.AddUnits([(Nation.England, UnitType.Fleet, "NTH")]);

        var order = units[0].Move("Pic");

        // Act
        new Adjudicator(world, false, Regions, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits([(Nation.England, UnitType.Fleet, "NTH", false)]);
    }

    [Fact(DisplayName = "A.2. Move army to sea")]
    public void DATC_A_2()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();
        var units = board.AddUnits([(Nation.England, UnitType.Army, "Lvp")]);

        var order = units[0].Move("IRI");

        // Act
        new Adjudicator(world, false, Regions, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits([(Nation.England, UnitType.Army, "Lvp", false)]);
    }

    [Fact(DisplayName = "A.3. Move fleet to land")]
    public void DATC_A_3()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();
        var units = board.AddUnits([(Nation.Germany, UnitType.Fleet, "Kie")]);

        var order = units[0].Move("Mun");

        // Act
        new Adjudicator(world, false, Regions, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits([(Nation.Germany, UnitType.Fleet, "Kie", false)]);
    }

    [Fact(DisplayName = "A.4. Move to own sector")]
    public void DATC_A_4()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();
        var units = board.AddUnits([(Nation.Germany, UnitType.Fleet, "Kie")]);

        var order = units[0].Move("Kie");

        // Act
        new Adjudicator(world, false, Regions, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits([(Nation.Germany, UnitType.Fleet, "Kie", false)]);
    }

    [Fact(DisplayName = "A.5. Move to own sector with convoy")]
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

        var englishMove = units[1].Move("Yor");
        var englishConvoy = units[0].Convoy(units[1], "Yor");
        var englishSupport = units[2].Support(units[1], "Yor");
        var germanMove = units[3].Move("Yor");
        var germanSupport = units[4].Support(units[3], "Yor");

        // Act
        new Adjudicator(world, false, Regions, DefaultWorldFactory).Adjudicate();

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
                (Nation.Germany, UnitType.Fleet, "Yor", false),
                (Nation.Germany, UnitType.Army, "Wal", false),
            ]);
        board.ShouldNotHaveNextBoard();
    }

    [Fact(DisplayName = "A.6. Ordering a unit of another country", Skip = "Not applicable")]
    public void DATC_A_6()
    {
        // Order ownership is controlled by the client. Once an order reaches the server, it retains no ownership
        // information other than the acting unit. So this test case is not valid for the 5D Diplomacy adjudicator.
    }

    [Fact(DisplayName = "A.7. Only armies can be convoyed")]
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

        var move = units[0].Move("Bel");
        var convoy = units[1].Convoy(units[0], "Bel");

        // Act
        new Adjudicator(world, false, Regions, DefaultWorldFactory).Adjudicate();

        // Assert
        move.Status.Should().Be(OrderStatus.Invalid);
        convoy.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "Lon", false),
                (Nation.England, UnitType.Fleet, "NTH", false),
            ]);
    }

    [Fact(DisplayName = "A.8. Support to hold yourself is not possible")]
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

        var italianMove = units[0].Move("Tri");
        var italianSupport = units[1].Support(units[0], "Tri");
        var austrianSupport = units[2].Support(units[2], "Tri");

        // Act
        new Adjudicator(world, false, Regions, DefaultWorldFactory).Adjudicate();

        // Assert
        italianMove.Status.Should().Be(OrderStatus.Success);
        italianSupport.Status.Should().Be(OrderStatus.Success);
        austrianSupport.Status.Should().Be(OrderStatus.Invalid);

        board.ShouldHaveUnits(
            [
                (Nation.Italy, UnitType.Army, "Ven", true),
                (Nation.Italy, UnitType.Army, "Tyr", false),
                (Nation.Austria, UnitType.Fleet, "Ven", false),
            ]);
        board.ShouldNotHaveNextBoard();
    }

    [Fact(DisplayName = "A.9. Fleets must follow coast if not on sea")]
    public void DATC_A_9()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();
        var units = board.AddUnits([(Nation.Italy, UnitType.Fleet, "Rom")]);

        var order = units[0].Move("Ven");

        // Act
        new Adjudicator(world, false, Regions, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits([(Nation.Italy, UnitType.Fleet, "Rom", false)]);
    }

    [Fact(DisplayName = "A.10. Support on unreachable destination not possible")]
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

        var austrianHold = units[0].Hold();
        var italianMove = units[1].Move("Ven");
        var italianSupport = units[2].Support(units[1], "Ven");

        // Act
        new Adjudicator(world, false, Regions, DefaultWorldFactory).Adjudicate();

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
    }

    [Fact(DisplayName = "A.11. Simple bounce")]
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

        var austrianMove = units[0].Move("Tyr");
        var italianMove = units[1].Move("Tyr");

        // Act
        new Adjudicator(world, false, Regions, DefaultWorldFactory).Adjudicate();

        // Assert
        austrianMove.Status.Should().Be(OrderStatus.Failure);
        italianMove.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.Austria, UnitType.Army, "Vie", false),
                (Nation.Italy, UnitType.Army, "Ven", false),
            ]);
    }

    [Fact(DisplayName = "A.12. Bounce of three units")]
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

        var austrianMove = units[0].Move("Tyr");
        var germanMove = units[1].Move("Tyr");
        var italianMove = units[2].Move("Tyr");

        // Act
        new Adjudicator(world, false, Regions, DefaultWorldFactory).Adjudicate();

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
    }
}
