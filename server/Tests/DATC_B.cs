﻿using Adjudication;
using Entities;
using Enums;
using FluentAssertions;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Tests;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
public class DATC_B : AdjudicationTestBase
{
    [Fact(DisplayName = "B.1. Moving with unspecified coast when coast is necessary")]
    public void DATC_B_1()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();
        var units = board.AddUnits([(Nation.France, UnitType.Fleet, "Por")]);

        var order = units[0].Move("Spa");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits([(Nation.France, UnitType.Fleet, "Por", false)]);
    }

    [Fact(DisplayName = "B.2. Moving with unspecified coast when coast is not necessary", Skip = "Decided against")]
    public void DATC_B_2()
    {
        // Although a more lenient adjudicator would allow this, for simplicity of implementation, 5D Diplomacy always
        // requires a coast to be specified where applicable. The client makes this obvious to the user, so an ambiguous
        // order should never be possible.
    }

    [Fact(DisplayName = "B.3. Moving with wrong coast when coast is not necessary")]
    public void DATC_B_3()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();
        var units = board.AddUnits([(Nation.France, UnitType.Fleet, "Gas")]);

        var order = units[0].Move("Spa_S");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits([(Nation.France, UnitType.Fleet, "Gas", false)]);
    }

    [Fact(DisplayName = "B.4. Support to unreachable coast allowed")]
    public void DATC_B_4()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.France, UnitType.Fleet, "Gas"),
                (Nation.France, UnitType.Fleet, "Mar"),
                (Nation.Italy, UnitType.Fleet, "WES"),
            ]);

        var frenchMove = units[0].Move("Spa_N");
        var frenchSupport = units[1].Support(units[0], "Spa_N");
        var italianMove = units[2].Move("Spa_S");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        frenchMove.Status.Should().Be(OrderStatus.Success);
        frenchSupport.Status.Should().Be(OrderStatus.Success);
        italianMove.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.France, UnitType.Fleet, "Spa_N", false),
                (Nation.France, UnitType.Fleet, "Mar", false),
                (Nation.Italy, UnitType.Fleet, "WES", false),
            ]);
    }

    [Fact(DisplayName = "B.5. Support from unreachable coast not allowed")]
    public void DATC_B_5()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.France, UnitType.Fleet, "Mar"),
                (Nation.France, UnitType.Fleet, "Spa_N"),
                (Nation.Italy, UnitType.Fleet, "LYO"),
            ]);

        var frenchMove = units[0].Move("LYO");
        var frenchSupport = units[1].Support(units[0], "LYO");
        var italianMove = units[2].Hold();

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        frenchMove.Status.Should().Be(OrderStatus.Failure);
        frenchSupport.Status.Should().Be(OrderStatus.Invalid);
        italianMove.Status.Should().Be(OrderStatus.Success);

        board.Next().ShouldHaveUnits(
            [
                (Nation.France, UnitType.Fleet, "Mar", false),
                (Nation.France, UnitType.Fleet, "Spa_N", false),
                (Nation.Italy, UnitType.Fleet, "LYO", false),
            ]);
    }

    [Fact(DisplayName = "B.6. Support can be cut with other coast")]
    public void DATC_B_6()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "IRI"),
                (Nation.England, UnitType.Fleet, "NAO"),
                (Nation.France, UnitType.Fleet, "Spa_N"),
                (Nation.France, UnitType.Fleet, "MAO"),
                (Nation.Italy, UnitType.Fleet, "LYO"),
            ]);

        var englishMove = units[1].Move("MAO");
        var englishSupport = units[0].Support(units[1], "MAO");
        var frenchHold = units[3].Hold();
        var frenchSupport = units[2].Support(units[3], "MAO");
        var italianMove = units[4].Move("Spa_S");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        englishSupport.Status.Should().Be(OrderStatus.Success);
        frenchHold.Status.Should().Be(OrderStatus.Failure);
        frenchSupport.Status.Should().Be(OrderStatus.Failure);
        italianMove.Status.Should().Be(OrderStatus.Failure);

        board.ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "IRI", false),
                (Nation.England, UnitType.Fleet, "MAO", false),
                (Nation.France, UnitType.Fleet, "Spa_N", false),
                (Nation.France, UnitType.Fleet, "MAO", true),
                (Nation.Italy, UnitType.Fleet, "LYO", false),
            ]);
        board.ShouldNotHaveNextBoard();
    }

    [Fact(DisplayName = "B.7. Supporting own unit with unspecified coast", Skip = "Decided against")]
    public void DATC_B_7()
    {
        // Again, the client for 5D Diplomacy should enforce a fleet to specify a coast in such a scenario, so the move
        // is invalid in this context. The DATC specifies it should succeed, but we choose to be strict.
    }

    [Fact(DisplayName = "B.8. Supporting with unspecified coast when only one coast is possible", Skip = "Decided against")]
    public void DATC_B_8()
    {
        // Similarly, such an order would not be possible to enter with the 5D Diplomacy client, so we decide it should
        // be invalid.
    }

    [Fact(DisplayName = "B.9. Supporting with wrong coast")]
    public void DATC_B_9()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.France, UnitType.Fleet, "Por"),
                (Nation.France, UnitType.Fleet, "MAO"),
                (Nation.Italy, UnitType.Fleet, "LYO"),
                (Nation.Italy, UnitType.Fleet, "WES"),
            ]);

        var frenchMove = units[1].Move("Spa_S");
        var frenchSupport = units[0].Support(units[1], "Spa_N");
        var italianMove = units[3].Move("Spa_S");
        var italianSupport = units[2].Support(units[3], "Spa_S");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        frenchMove.Status.Should().Be(OrderStatus.Failure);
        frenchSupport.Status.Should().Be(OrderStatus.Invalid);
        italianMove.Status.Should().Be(OrderStatus.Success);
        italianSupport.Status.Should().Be(OrderStatus.Success);

        board.Next().ShouldHaveUnits(
            [
                (Nation.France, UnitType.Fleet, "Por", false),
                (Nation.France, UnitType.Fleet, "MAO", false),
                (Nation.Italy, UnitType.Fleet, "LYO", false),
                (Nation.Italy, UnitType.Fleet, "Spa_S", false),
            ]);
    }

    [Fact(DisplayName = "B.10. Unit ordered with wrong coast", Skip = "Decided against")]
    public void DATC_B_10()
    {
        // Another one where the client prevents such an order from being entered, so the decision is to disallow it, in
        // contrast to the DATC.
    }

    [Fact(DisplayName = "B.11. Coast cannot be ordered to change")]
    public void DATC_B_11()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();
        var units = board.AddUnits([(Nation.France, UnitType.Fleet, "Spa_N")]);

        var order = units[0].Move("LYO");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits([(Nation.France, UnitType.Fleet, "Spa_N", false)]);
    }

    [Fact(DisplayName = "B.12. Army movement with coastal specification", Skip = "Decided against")]
    public void DATC_B_12()
    {
        // Again, the 5D Diplomacy adjudicator accepts only perfect orders, so we go against the DATC's conclusion
        // here.
    }

    [Fact(DisplayName = "B.13. Coastal crawl not allowed")]
    public void DATC_B_13()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Turkey, UnitType.Fleet, "Bul_S"),
                (Nation.Turkey, UnitType.Fleet, "Con"),
            ]);

        var turkishMove1 = units[0].Move("Con");
        var turkishMove2 = units[1].Move("Bul_E");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        turkishMove1.Status.Should().Be(OrderStatus.Failure);
        turkishMove2.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.Turkey, UnitType.Fleet, "Bul_S", false),
                (Nation.Turkey, UnitType.Fleet, "Con", false),
            ]);
    }

    [Fact(DisplayName = "B.14. Building with unspecified coast")]
    public void DATC_B_14()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard(phase: Phase.Winter);

        board.AddCentres([(Nation.Russia, "Stp")]);

        var order = board.Build(Nation.Russia, UnitType.Fleet, "Stp");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        order.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits([]);
    }

    [Fact(DisplayName = "B.15. Supporting foreign unit with unspecified coast", Skip = "Decided against")]
    public void DATC_B_15()
    {
        // This is another example where 5D Diplomacy considers the order invalid as an act of pedantry.
    }
}
