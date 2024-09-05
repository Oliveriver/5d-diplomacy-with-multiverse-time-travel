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
public class DATC_H : AdjudicationTestBase
{
    [Fact(DisplayName = "H.01. No supports during retreat")]
    public void DATC_H_1()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Austria, UnitType.Fleet, "Tri"),
                (Nation.Austria, UnitType.Army, "Ser"),
                (Nation.Turkey, UnitType.Fleet, "Gre"),
            ]);

        units.Get("Tri").MustRetreat = true;
        units.Get("Gre").MustRetreat = true;

        var austrianMove = units.Get("Tri").Move("Alb");
        var austrianSupport = units.Get("Ser").Support(units.Get("Tri"), "Alb");
        var turkishMove = units.Get("Gre").Move("Alb");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        austrianMove.Status.Should().Be(OrderStatus.Failure);
        austrianSupport.Status.Should().Be(OrderStatus.Invalid);
        turkishMove.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.Austria, UnitType.Army, "Ser", false),
            ]);
    }

    [Fact(DisplayName = "H.02. No supports from retreating unit")]
    public void DATC_H_2()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "Nwy"),
                (Nation.Russia, UnitType.Fleet, "Edi"),
                (Nation.Russia, UnitType.Fleet, "Hol"),
            ]);

        units.Get("Nwy").MustRetreat = true;
        units.Get("Edi").MustRetreat = true;
        units.Get("Hol").MustRetreat = true;

        var englishMove = units.Get("Nwy").Move("NTH");
        var russianMove = units.Get("Edi").Move("NTH");
        var russianSupport = units.Get("Hol").Support(units.Get("Edi"), "NTH");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Failure);
        russianMove.Status.Should().Be(OrderStatus.Failure);
        russianSupport.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits([]);
    }

    [Fact(DisplayName = "H.03. No convoy during retreat")]
    public void DATC_H_3()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Army, "Hol"),
                (Nation.England, UnitType.Fleet, "NTH"),
            ]);

        units.Get("Hol").MustRetreat = true;

        var englishMove = units.Get("Hol").Move("Yor");
        var englishConvoy = units.Get("NTH").Convoy(units.Get("Hol"), "Yor");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Invalid);
        englishConvoy.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH", false),
            ]);
    }

    [Fact(DisplayName = "H.04. No other moves during retreat")]
    public void DATC_H_4()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Army, "Hol"),
                (Nation.England, UnitType.Fleet, "NTH"),
            ]);

        units.Get("Hol").MustRetreat = true;

        var englishMove1 = units.Get("Hol").Move("Bel");
        var englishMove2 = units.Get("NTH").Move("NWG");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove1.Status.Should().Be(OrderStatus.Retreat);
        englishMove2.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Army, "Bel", false),
                (Nation.England, UnitType.Fleet, "NTH", false),
            ]);
    }

    [Fact(DisplayName = "H.05. A unit may not retreat to the area from which it is attacked")]
    public void DATC_H_5()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Russia, UnitType.Fleet, "Con"),
                (Nation.Russia, UnitType.Fleet, "BLA"),
                (Nation.Turkey, UnitType.Fleet, "Ank"),
            ]);

        units.Get("BLA").Move("Ank", status: OrderStatus.Success);
        units.Get("Con").Support(units.Get("BLA"), "Ank", status: OrderStatus.Success);
        units.Get("Ank").Hold(status: OrderStatus.Failure);
        units.Get("Ank").MustRetreat = true;

        var move = units.Get("Ank").Move("BLA");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        move.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.Russia, UnitType.Fleet, "Con", false),
                (Nation.Russia, UnitType.Fleet, "Ank", false),
            ]);
    }

    [Fact(DisplayName = "H.06. Unit may not retreat to a contested area")]
    public void DATC_H_6()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Austria, UnitType.Army, "Bud"),
                (Nation.Austria, UnitType.Army, "Tri"),
                (Nation.Germany, UnitType.Army, "Mun"),
                (Nation.Germany, UnitType.Army, "Sil"),
                (Nation.Italy, UnitType.Army, "Vie"),
            ]);

        units.Get("Tri").Move("Vie", status: OrderStatus.Success);
        units.Get("Bud").Support(units.Get("Tri"), "Vie", status: OrderStatus.Success);
        units.Get("Mun").Move("Boh", status: OrderStatus.Failure);
        units.Get("Sil").Move("Boh", status: OrderStatus.Failure);
        units.Get("Vie").Hold(status: OrderStatus.Failure);
        units.Get("Vie").MustRetreat = true;

        var move = units.Get("Vie").Move("Boh");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        move.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.Austria, UnitType.Army, "Bud", false),
                (Nation.Austria, UnitType.Army, "Vie", false),
                (Nation.Germany, UnitType.Army, "Mun", false),
                (Nation.Germany, UnitType.Army, "Sil", false),
            ]);
    }

    [Fact(DisplayName = "H.07. Multiple reteat to same area will disband units")]
    public void DATC_H_7()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Italy, UnitType.Army, "Boh"),
                (Nation.Italy, UnitType.Army, "Vie"),
            ]);

        units.Get("Boh").MustRetreat = true;
        units.Get("Vie").MustRetreat = true;

        var move1 = units.Get("Boh").Move("Tyr");
        var move2 = units.Get("Vie").Move("Tyr");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        move1.Status.Should().Be(OrderStatus.Failure);
        move2.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits([]);
    }

    [Fact(DisplayName = "H.08. Triple retreat to same area will disband units")]
    public void DATC_H_8()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "Nwy"),
                (Nation.Russia, UnitType.Fleet, "Edi"),
                (Nation.Russia, UnitType.Fleet, "Hol"),
            ]);

        units.Get("Nwy").MustRetreat = true;
        units.Get("Edi").MustRetreat = true;
        units.Get("Hol").MustRetreat = true;

        var englishMove = units.Get("Nwy").Move("NTH");
        var russianMove1 = units.Get("Edi").Move("NTH");
        var russianMove2 = units.Get("Hol").Move("NTH");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Failure);
        russianMove1.Status.Should().Be(OrderStatus.Failure);
        russianMove2.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits([]);
    }

    [Fact(DisplayName = "H.09. Dislodged unit will not make attacker's area contested")]
    public void DATC_H_9()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "HEL"),
                (Nation.England, UnitType.Fleet, "Den"),
                (Nation.Germany, UnitType.Army, "Ber"),
                (Nation.Germany, UnitType.Fleet, "Kie"),
                (Nation.Germany, UnitType.Army, "Sil"),
                (Nation.Russia, UnitType.Army, "Pru"),
            ]);

        units.Get("HEL").Move("Kie", status: OrderStatus.Success);
        units.Get("Den").Support(units.Get("HEL"), "Kie", status: OrderStatus.Success);
        units.Get("Kie").Hold(status: OrderStatus.Failure);
        units.Get("Ber").Move("Pru", status: OrderStatus.Success);
        units.Get("Sil").Support(units.Get("Ber"), "Pru", status: OrderStatus.Success);
        units.Get("Pru").Move("Ber", status: OrderStatus.Failure);
        units.Get("Kie").MustRetreat = true;
        units.Get("Pru").MustRetreat = true;

        var move = units.Get("Kie").Move("Ber");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        move.Status.Should().Be(OrderStatus.Retreat);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "Kie", false),
                (Nation.England, UnitType.Fleet, "Den", false),
                (Nation.Germany, UnitType.Army, "Pru", false),
                (Nation.Germany, UnitType.Fleet, "Ber", false),
                (Nation.Germany, UnitType.Army, "Sil", false),
            ]);
    }

    [Fact(DisplayName = "H.10. Not retreating to attacker does not mean contested")]
    public void DATC_H_10()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Army, "Kie"),
                (Nation.Germany, UnitType.Army, "Ber"),
                (Nation.Germany, UnitType.Army, "Mun"),
                (Nation.Germany, UnitType.Army, "Pru"),
                (Nation.Russia, UnitType.Army, "War"),
                (Nation.Russia, UnitType.Army, "Sil"),
            ]);

        units.Get("Kie").Hold(status: OrderStatus.Failure);
        units.Get("Pru").Hold(status: OrderStatus.Failure);
        units.Get("Ber").Move("Kie", status: OrderStatus.Success);
        units.Get("Mun").Support(units.Get("Ber"), "Kie", status: OrderStatus.Success);
        units.Get("War").Move("Pru", status: OrderStatus.Success);
        units.Get("Sil").Support(units.Get("War"), "Pru", status: OrderStatus.Success);
        units.Get("Kie").MustRetreat = true;
        units.Get("Pru").MustRetreat = true;

        var englishMove = units.Get("Kie").Move("Ber");
        var germanMove = units.Get("Pru").Move("Ber");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Failure);
        germanMove.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.Germany, UnitType.Army, "Kie", false),
                (Nation.Germany, UnitType.Army, "Mun", false),
                (Nation.Germany, UnitType.Army, "Ber", false),
                (Nation.Russia, UnitType.Army, "Pru", false),
                (Nation.Russia, UnitType.Army, "Sil", false),
            ]);
    }
}
