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
public class DATC_E : AdjudicationTestBase
{
    [Fact(DisplayName = "DATC E.01. Dislodged unit has no effect on attacker's area")]
    public void DATC_E_1()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Germany, UnitType.Army, "Ber"),
                (Nation.Germany, UnitType.Fleet, "Kie"),
                (Nation.Germany, UnitType.Army, "Sil"),
                (Nation.Russia, UnitType.Army, "Pru"),
            ]);

        var germanMove1 = units.Get("Ber").Move("Pru");
        var germanMove2 = units.Get("Kie").Move("Ber");
        var germanSupport = units.Get("Sil").Support(units.Get("Ber"), "Pru");
        var russianMove = units.Get("Pru").Move("Ber");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        germanMove1.Status.Should().Be(OrderStatus.Success);
        germanMove2.Status.Should().Be(OrderStatus.Success);
        germanSupport.Status.Should().Be(OrderStatus.Success);
        russianMove.Status.Should().Be(OrderStatus.Failure);

        board.ShouldHaveUnits(
            [
                (Nation.Germany, UnitType.Army, "Ber", false),
                (Nation.Germany, UnitType.Fleet, "Kie", false),
                (Nation.Germany, UnitType.Army, "Sil", false),
                (Nation.Russia, UnitType.Army, "Pru", true),
            ]);
        board.ShouldNotHaveNextBoard();

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "DATC E.02. No self dislodgement in head-to-head battle")]
    public void DATC_E_2()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Germany, UnitType.Army, "Ber"),
                (Nation.Germany, UnitType.Fleet, "Kie"),
                (Nation.Germany, UnitType.Army, "Mun"),
            ]);

        var germanMove1 = units.Get("Ber").Move("Kie");
        var germanMove2 = units.Get("Kie").Move("Ber");
        var germanSupport = units.Get("Mun").Support(units.Get("Ber"), "Kie");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        germanMove1.Status.Should().Be(OrderStatus.Failure);
        germanMove2.Status.Should().Be(OrderStatus.Failure);
        germanSupport.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.Germany, UnitType.Army, "Ber", false),
                (Nation.Germany, UnitType.Fleet, "Kie", false),
                (Nation.Germany, UnitType.Army, "Mun", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "DATC E.03. No help in dislodging own unit")]
    public void DATC_E_3()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Germany, UnitType.Army, "Ber"),
                (Nation.Germany, UnitType.Army, "Mun"),
                (Nation.England, UnitType.Fleet, "Kie"),
            ]);

        var germanMove = units.Get("Ber").Move("Kie");
        var germanSupport = units.Get("Mun").Support(units.Get("Kie"), "Ber");
        var englishMove = units.Get("Kie").Move("Ber");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        germanMove.Status.Should().Be(OrderStatus.Failure);
        germanSupport.Status.Should().Be(OrderStatus.Failure);
        englishMove.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.Germany, UnitType.Army, "Ber", false),
                (Nation.Germany, UnitType.Army, "Mun", false),
                (Nation.England, UnitType.Fleet, "Kie", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "DATC E.04. Non-dislodged loser still has effect")]
    public void DATC_E_4()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Germany, UnitType.Fleet, "Hol"),
                (Nation.Germany, UnitType.Fleet, "HEL"),
                (Nation.Germany, UnitType.Fleet, "SKA"),
                (Nation.France, UnitType.Fleet, "NTH"),
                (Nation.France, UnitType.Fleet, "Bel"),
                (Nation.England, UnitType.Fleet, "Edi"),
                (Nation.England, UnitType.Fleet, "Yor"),
                (Nation.England, UnitType.Fleet, "NWG"),
                (Nation.Austria, UnitType.Army, "Kie"),
                (Nation.Austria, UnitType.Army, "Ruh"),
            ]);

        var germanMove = units.Get("Hol").Move("NTH");
        var germanSupport1 = units.Get("HEL").Support(units.Get("Hol"), "NTH");
        var germanSupport2 = units.Get("SKA").Support(units.Get("Hol"), "NTH");
        var frenchMove = units.Get("NTH").Move("Hol");
        var frenchSupport = units.Get("Bel").Support(units.Get("NTH"), "Hol");
        var englishMove = units.Get("NWG").Move("NTH");
        var englishSupport1 = units.Get("Edi").Support(units.Get("NWG"), "NTH");
        var englishSupport2 = units.Get("Yor").Support(units.Get("NWG"), "NTH");
        var austrianMove = units.Get("Ruh").Move("Hol");
        var austrianSupport = units.Get("Kie").Support(units.Get("Ruh"), "Hol");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        germanMove.Status.Should().Be(OrderStatus.Failure);
        germanSupport1.Status.Should().Be(OrderStatus.Failure);
        germanSupport2.Status.Should().Be(OrderStatus.Failure);
        frenchMove.Status.Should().Be(OrderStatus.Failure);
        frenchSupport.Status.Should().Be(OrderStatus.Failure);
        englishMove.Status.Should().Be(OrderStatus.Failure);
        englishSupport1.Status.Should().Be(OrderStatus.Failure);
        englishSupport2.Status.Should().Be(OrderStatus.Failure);
        austrianMove.Status.Should().Be(OrderStatus.Failure);
        austrianSupport.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.Germany, UnitType.Fleet, "Hol", false),
                (Nation.Germany, UnitType.Fleet, "HEL", false),
                (Nation.Germany, UnitType.Fleet, "SKA", false),
                (Nation.France, UnitType.Fleet, "NTH", false),
                (Nation.France, UnitType.Fleet, "Bel", false),
                (Nation.England, UnitType.Fleet, "Edi", false),
                (Nation.England, UnitType.Fleet, "Yor", false),
                (Nation.England, UnitType.Fleet, "NWG", false),
                (Nation.Austria, UnitType.Army, "Kie", false),
                (Nation.Austria, UnitType.Army, "Ruh", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "DATC E.05. Loser dislodged by another army still has effect")]
    public void DATC_E_5()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Germany, UnitType.Fleet, "Hol"),
                (Nation.Germany, UnitType.Fleet, "HEL"),
                (Nation.Germany, UnitType.Fleet, "SKA"),
                (Nation.France, UnitType.Fleet, "NTH"),
                (Nation.France, UnitType.Fleet, "Bel"),
                (Nation.England, UnitType.Fleet, "Edi"),
                (Nation.England, UnitType.Fleet, "Yor"),
                (Nation.England, UnitType.Fleet, "NWG"),
                (Nation.England, UnitType.Fleet, "Lon"),
                (Nation.Austria, UnitType.Army, "Kie"),
                (Nation.Austria, UnitType.Army, "Ruh"),
            ]);

        var germanMove = units.Get("Hol").Move("NTH");
        var germanSupport1 = units.Get("HEL").Support(units.Get("Hol"), "NTH");
        var germanSupport2 = units.Get("SKA").Support(units.Get("Hol"), "NTH");
        var frenchMove = units.Get("NTH").Move("Hol");
        var frenchSupport = units.Get("Bel").Support(units.Get("NTH"), "Hol");
        var englishMove = units.Get("NWG").Move("NTH");
        var englishSupport1 = units.Get("Edi").Support(units.Get("NWG"), "NTH");
        var englishSupport2 = units.Get("Yor").Support(units.Get("NWG"), "NTH");
        var englishSupport3 = units.Get("Lon").Support(units.Get("NWG"), "NTH");
        var austrianMove = units.Get("Ruh").Move("Hol");
        var austrianSupport = units.Get("Kie").Support(units.Get("Ruh"), "Hol");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        germanMove.Status.Should().Be(OrderStatus.Failure);
        germanSupport1.Status.Should().Be(OrderStatus.Failure);
        germanSupport2.Status.Should().Be(OrderStatus.Failure);
        frenchMove.Status.Should().Be(OrderStatus.Failure);
        frenchSupport.Status.Should().Be(OrderStatus.Failure);
        englishMove.Status.Should().Be(OrderStatus.Success);
        englishSupport1.Status.Should().Be(OrderStatus.Success);
        englishSupport2.Status.Should().Be(OrderStatus.Success);
        englishSupport3.Status.Should().Be(OrderStatus.Success);
        austrianMove.Status.Should().Be(OrderStatus.Failure);
        austrianSupport.Status.Should().Be(OrderStatus.Failure);

        board.ShouldHaveUnits(
            [
                (Nation.Germany, UnitType.Fleet, "Hol", false),
                (Nation.Germany, UnitType.Fleet, "HEL", false),
                (Nation.Germany, UnitType.Fleet, "SKA", false),
                (Nation.France, UnitType.Fleet, "NTH", true),
                (Nation.France, UnitType.Fleet, "Bel", false),
                (Nation.England, UnitType.Fleet, "Edi", false),
                (Nation.England, UnitType.Fleet, "Yor", false),
                (Nation.England, UnitType.Fleet, "NWG", false),
                (Nation.England, UnitType.Fleet, "Lon", false),
                (Nation.Austria, UnitType.Army, "Kie", false),
                (Nation.Austria, UnitType.Army, "Ruh", false),
            ]);
        board.ShouldNotHaveNextBoard();

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "DATC E.06. Not dislodge because of own support still has effect")]
    public void DATC_E_6()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Germany, UnitType.Fleet, "Hol"),
                (Nation.Germany, UnitType.Fleet, "HEL"),
                (Nation.France, UnitType.Fleet, "NTH"),
                (Nation.France, UnitType.Fleet, "Bel"),
                (Nation.France, UnitType.Fleet, "ENG"),
                (Nation.Austria, UnitType.Army, "Kie"),
                (Nation.Austria, UnitType.Army, "Ruh"),
            ]);

        var germanMove = units.Get("Hol").Move("NTH");
        var germanSupport = units.Get("HEL").Support(units.Get("Hol"), "NTH");
        var frenchMove = units.Get("NTH").Move("Hol");
        var frenchSupport1 = units.Get("Bel").Support(units.Get("NTH"), "Hol");
        var frenchSupport2 = units.Get("ENG").Support(units.Get("Hol"), "NTH");
        var austrianMove = units.Get("Ruh").Move("Hol");
        var austrianSupport = units.Get("Kie").Support(units.Get("Ruh"), "Hol");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        germanMove.Status.Should().Be(OrderStatus.Failure);
        germanSupport.Status.Should().Be(OrderStatus.Failure);
        frenchMove.Status.Should().Be(OrderStatus.Failure);
        frenchSupport1.Status.Should().Be(OrderStatus.Failure);
        frenchSupport2.Status.Should().Be(OrderStatus.Failure);
        austrianMove.Status.Should().Be(OrderStatus.Failure);
        austrianSupport.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.Germany, UnitType.Fleet, "Hol", false),
                (Nation.Germany, UnitType.Fleet, "HEL", false),
                (Nation.France, UnitType.Fleet, "NTH", false),
                (Nation.France, UnitType.Fleet, "Bel", false),
                (Nation.France, UnitType.Fleet, "ENG", false),
                (Nation.Austria, UnitType.Army, "Kie", false),
                (Nation.Austria, UnitType.Army, "Ruh", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "DATC E.07. No self dislodgement with beleaguered garrison")]
    public void DATC_E_7()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH"),
                (Nation.England, UnitType.Fleet, "Yor"),
                (Nation.Germany, UnitType.Fleet, "Hol"),
                (Nation.Germany, UnitType.Fleet, "HEL"),
                (Nation.Russia, UnitType.Fleet, "SKA"),
                (Nation.Russia, UnitType.Fleet, "Nwy"),
            ]);

        var englishHold = units.Get("NTH").Hold();
        var englishSupport = units.Get("Yor").Support(units.Get("Nwy"), "NTH");
        var germanMove = units.Get("HEL").Move("NTH");
        var germanSupport = units.Get("Hol").Support(units.Get("HEL"), "NTH");
        var russianMove = units.Get("Nwy").Move("NTH");
        var russianSupport = units.Get("SKA").Support(units.Get("Nwy"), "NTH");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishHold.Status.Should().Be(OrderStatus.Success);
        englishSupport.Status.Should().Be(OrderStatus.Failure);
        germanMove.Status.Should().Be(OrderStatus.Failure);
        germanSupport.Status.Should().Be(OrderStatus.Failure);
        russianMove.Status.Should().Be(OrderStatus.Failure);
        russianSupport.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH", false),
                (Nation.England, UnitType.Fleet, "Yor", false),
                (Nation.Germany, UnitType.Fleet, "Hol", false),
                (Nation.Germany, UnitType.Fleet, "HEL", false),
                (Nation.Russia, UnitType.Fleet, "SKA", false),
                (Nation.Russia, UnitType.Fleet, "Nwy", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "DATC E.08. No self dislodgement with beleaguered garrison and head-to-head battle")]
    public void DATC_E_8()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH"),
                (Nation.England, UnitType.Fleet, "Yor"),
                (Nation.Germany, UnitType.Fleet, "Hol"),
                (Nation.Germany, UnitType.Fleet, "HEL"),
                (Nation.Russia, UnitType.Fleet, "SKA"),
                (Nation.Russia, UnitType.Fleet, "Nwy"),
            ]);

        var englishMove = units.Get("NTH").Move("Nwy");
        var englishSupport = units.Get("Yor").Support(units.Get("Nwy"), "NTH");
        var germanMove = units.Get("HEL").Move("NTH");
        var germanSupport = units.Get("Hol").Support(units.Get("HEL"), "NTH");
        var russianMove = units.Get("Nwy").Move("NTH");
        var russianSupport = units.Get("SKA").Support(units.Get("Nwy"), "NTH");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Failure);
        englishSupport.Status.Should().Be(OrderStatus.Failure);
        germanMove.Status.Should().Be(OrderStatus.Failure);
        germanSupport.Status.Should().Be(OrderStatus.Failure);
        russianMove.Status.Should().Be(OrderStatus.Failure);
        russianSupport.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH", false),
                (Nation.England, UnitType.Fleet, "Yor", false),
                (Nation.Germany, UnitType.Fleet, "Hol", false),
                (Nation.Germany, UnitType.Fleet, "HEL", false),
                (Nation.Russia, UnitType.Fleet, "SKA", false),
                (Nation.Russia, UnitType.Fleet, "Nwy", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "DATC E.09. Almost self dislodgement with beleaguered garrison")]
    public void DATC_E_9()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH"),
                (Nation.England, UnitType.Fleet, "Yor"),
                (Nation.Germany, UnitType.Fleet, "Hol"),
                (Nation.Germany, UnitType.Fleet, "HEL"),
                (Nation.Russia, UnitType.Fleet, "SKA"),
                (Nation.Russia, UnitType.Fleet, "Nwy"),
            ]);

        var englishMove = units.Get("NTH").Move("NWG");
        var englishSupport = units.Get("Yor").Support(units.Get("Nwy"), "NTH");
        var germanMove = units.Get("HEL").Move("NTH");
        var germanSupport = units.Get("Hol").Support(units.Get("HEL"), "NTH");
        var russianMove = units.Get("Nwy").Move("NTH");
        var russianSupport = units.Get("SKA").Support(units.Get("Nwy"), "NTH");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        englishSupport.Status.Should().Be(OrderStatus.Success);
        germanMove.Status.Should().Be(OrderStatus.Failure);
        germanSupport.Status.Should().Be(OrderStatus.Failure);
        russianMove.Status.Should().Be(OrderStatus.Success);
        russianSupport.Status.Should().Be(OrderStatus.Success);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "NWG", false),
                (Nation.England, UnitType.Fleet, "Yor", false),
                (Nation.Germany, UnitType.Fleet, "Hol", false),
                (Nation.Germany, UnitType.Fleet, "HEL", false),
                (Nation.Russia, UnitType.Fleet, "SKA", false),
                (Nation.Russia, UnitType.Fleet, "NTH", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "DATC E.10. Almost circular movement with no self dislodgement with beleaguered garrison")]
    public void DATC_E_10()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH"),
                (Nation.England, UnitType.Fleet, "Yor"),
                (Nation.Germany, UnitType.Fleet, "Hol"),
                (Nation.Germany, UnitType.Fleet, "HEL"),
                (Nation.Germany, UnitType.Fleet, "Den"),
                (Nation.Russia, UnitType.Fleet, "SKA"),
                (Nation.Russia, UnitType.Fleet, "Nwy"),
            ]);

        var englishMove = units.Get("NTH").Move("Den");
        var englishSupport = units.Get("Yor").Support(units.Get("Nwy"), "NTH");
        var germanMove1 = units.Get("HEL").Move("NTH");
        var germanMove2 = units.Get("Den").Move("HEL");
        var germanSupport = units.Get("Hol").Support(units.Get("HEL"), "NTH");
        var russianMove = units.Get("Nwy").Move("NTH");
        var russianSupport = units.Get("SKA").Support(units.Get("Nwy"), "NTH");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Failure);
        englishSupport.Status.Should().Be(OrderStatus.Failure);
        germanMove1.Status.Should().Be(OrderStatus.Failure);
        germanMove2.Status.Should().Be(OrderStatus.Failure);
        germanSupport.Status.Should().Be(OrderStatus.Failure);
        russianMove.Status.Should().Be(OrderStatus.Failure);
        russianSupport.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH", false),
                (Nation.England, UnitType.Fleet, "Yor", false),
                (Nation.Germany, UnitType.Fleet, "Hol", false),
                (Nation.Germany, UnitType.Fleet, "HEL", false),
                (Nation.Germany, UnitType.Fleet, "Den", false),
                (Nation.Russia, UnitType.Fleet, "SKA", false),
                (Nation.Russia, UnitType.Fleet, "Nwy", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "DATC E.11. No self dislodgement with beleaguered garrison, unit swap with adjacent convoying and two coasts")]
    public void DATC_E_11()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.France, UnitType.Army, "Spa"),
                (Nation.France, UnitType.Fleet, "MAO"),
                (Nation.France, UnitType.Fleet, "LYO"),
                (Nation.Germany, UnitType.Army, "Mar"),
                (Nation.Germany, UnitType.Army, "Gas"),
                (Nation.Italy, UnitType.Fleet, "Por"),
                (Nation.Italy, UnitType.Fleet, "WES"),
            ]);

        var frenchMove = units.Get("Spa").Move("Por");
        var frenchSupport = units.Get("LYO").Support(units.Get("Por"), "Spa_N");
        var frenchConvoy = units.Get("MAO").Convoy(units.Get("Spa"), "Por");
        var germanMove = units.Get("Gas").Move("Spa");
        var germanSupport = units.Get("Mar").Support(units.Get("Gas"), "Spa");
        var italianMove = units.Get("Por").Move("Spa_N");
        var italianSupport = units.Get("WES").Support(units.Get("Por"), "Spa_N");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        frenchMove.Status.Should().Be(OrderStatus.Success);
        frenchSupport.Status.Should().Be(OrderStatus.Success);
        frenchConvoy.Status.Should().Be(OrderStatus.Success);
        germanMove.Status.Should().Be(OrderStatus.Failure);
        germanSupport.Status.Should().Be(OrderStatus.Failure);
        italianMove.Status.Should().Be(OrderStatus.Success);
        italianSupport.Status.Should().Be(OrderStatus.Success);

        board.Next().ShouldHaveUnits(
            [
                (Nation.France, UnitType.Army, "Por", false),
                (Nation.France, UnitType.Fleet, "MAO", false),
                (Nation.France, UnitType.Fleet, "LYO", false),
                (Nation.Germany, UnitType.Army, "Mar", false),
                (Nation.Germany, UnitType.Army, "Gas", false),
                (Nation.Italy, UnitType.Fleet, "Spa_N", false),
                (Nation.Italy, UnitType.Fleet, "WES", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "DATC E.12. Support on attack on own unit can be used for other means")]
    public void DATC_E_12()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Austria, UnitType.Army, "Bud"),
                (Nation.Austria, UnitType.Army, "Ser"),
                (Nation.Italy, UnitType.Army, "Vie"),
                (Nation.Russia, UnitType.Army, "Gal"),
                (Nation.Russia, UnitType.Army, "Rum"),
            ]);

        var austrianMove = units.Get("Bud").Move("Rum");
        var austrianSupport = units.Get("Ser").Support(units.Get("Vie"), "Bud");
        var italianMove = units.Get("Vie").Move("Bud");
        var russianMove = units.Get("Gal").Move("Bud");
        var russianSupport = units.Get("Rum").Support(units.Get("Gal"), "Bud");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        austrianMove.Status.Should().Be(OrderStatus.Failure);
        austrianSupport.Status.Should().Be(OrderStatus.Failure);
        italianMove.Status.Should().Be(OrderStatus.Failure);
        russianMove.Status.Should().Be(OrderStatus.Failure);
        russianSupport.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.Austria, UnitType.Army, "Bud", false),
                (Nation.Austria, UnitType.Army, "Ser", false),
                (Nation.Italy, UnitType.Army, "Vie", false),
                (Nation.Russia, UnitType.Army, "Gal", false),
                (Nation.Russia, UnitType.Army, "Rum", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "DATC E.13. Three way beleaguered garrison")]
    public void DATC_E_13()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "Edi"),
                (Nation.England, UnitType.Fleet, "Yor"),
                (Nation.France, UnitType.Fleet, "Bel"),
                (Nation.France, UnitType.Fleet, "ENG"),
                (Nation.Germany, UnitType.Fleet, "NTH"),
                (Nation.Russia, UnitType.Fleet, "NWG"),
                (Nation.Russia, UnitType.Fleet, "Nwy"),
            ]);

        var englishMove = units.Get("Yor").Move("NTH");
        var englishSupport = units.Get("Edi").Support(units.Get("Yor"), "NTH");
        var frenchMove = units.Get("Bel").Move("NTH");
        var frenchSupport = units.Get("ENG").Support(units.Get("Bel"), "NTH");
        var germanHold = units.Get("NTH").Hold();
        var russianMove = units.Get("NWG").Move("NTH");
        var russianSupport = units.Get("Nwy").Support(units.Get("NWG"), "NTH");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Failure);
        englishSupport.Status.Should().Be(OrderStatus.Failure);
        frenchMove.Status.Should().Be(OrderStatus.Failure);
        frenchSupport.Status.Should().Be(OrderStatus.Failure);
        germanHold.Status.Should().Be(OrderStatus.Success);
        russianMove.Status.Should().Be(OrderStatus.Failure);
        russianSupport.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "Edi", false),
                (Nation.England, UnitType.Fleet, "Yor", false),
                (Nation.France, UnitType.Fleet, "Bel", false),
                (Nation.France, UnitType.Fleet, "ENG", false),
                (Nation.Germany, UnitType.Fleet, "NTH", false),
                (Nation.Russia, UnitType.Fleet, "NWG", false),
                (Nation.Russia, UnitType.Fleet, "Nwy", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "DATC E.14. Illegal head-to-head battle can still defend")]
    public void DATC_E_14()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Army, "Lvp"),
                (Nation.Russia, UnitType.Fleet, "Edi"),
            ]);

        var englishMove = units.Get("Lvp").Move("Edi");
        var russianMove = units.Get("Edi").Move("Lvp");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Failure);
        russianMove.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Army, "Lvp", false),
                (Nation.Russia, UnitType.Fleet, "Edi", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "DATC E.15. The friendly head-to-head battle")]
    public void DATC_E_15()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "Hol"),
                (Nation.England, UnitType.Army, "Ruh"),
                (Nation.France, UnitType.Army, "Kie"),
                (Nation.France, UnitType.Army, "Mun"),
                (Nation.France, UnitType.Army, "Sil"),
                (Nation.Germany, UnitType.Army, "Ber"),
                (Nation.Germany, UnitType.Fleet, "Den"),
                (Nation.Germany, UnitType.Fleet, "HEL"),
                (Nation.Russia, UnitType.Fleet, "BAL"),
                (Nation.Russia, UnitType.Army, "Pru"),
            ]);

        var englishMove = units.Get("Ruh").Move("Kie");
        var englishSupport = units.Get("Hol").Support(units.Get("Ruh"), "Kie");
        var frenchMove = units.Get("Kie").Move("Ber");
        var frenchSupport1 = units.Get("Mun").Support(units.Get("Kie"), "Ber");
        var frenchSupport2 = units.Get("Sil").Support(units.Get("Kie"), "Ber");
        var germanMove = units.Get("Ber").Move("Kie");
        var germanSupport1 = units.Get("Den").Support(units.Get("Ber"), "Kie");
        var germanSupport2 = units.Get("HEL").Support(units.Get("Ber"), "Kie");
        var russianMove = units.Get("Pru").Move("Ber");
        var russianSupport = units.Get("BAL").Support(units.Get("Pru"), "Ber");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Failure);
        englishSupport.Status.Should().Be(OrderStatus.Failure);
        frenchMove.Status.Should().Be(OrderStatus.Failure);
        frenchSupport1.Status.Should().Be(OrderStatus.Failure);
        frenchSupport2.Status.Should().Be(OrderStatus.Failure);
        germanMove.Status.Should().Be(OrderStatus.Failure);
        germanSupport1.Status.Should().Be(OrderStatus.Failure);
        germanSupport2.Status.Should().Be(OrderStatus.Failure);
        russianMove.Status.Should().Be(OrderStatus.Failure);
        russianSupport.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "Hol", false),
                (Nation.England, UnitType.Army, "Ruh", false),
                (Nation.France, UnitType.Army, "Kie", false),
                (Nation.France, UnitType.Army, "Mun", false),
                (Nation.France, UnitType.Army, "Sil", false),
                (Nation.Germany, UnitType.Army, "Ber", false),
                (Nation.Germany, UnitType.Fleet, "Den", false),
                (Nation.Germany, UnitType.Fleet, "HEL", false),
                (Nation.Russia, UnitType.Fleet, "BAL", false),
                (Nation.Russia, UnitType.Army, "Pru", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }
}
