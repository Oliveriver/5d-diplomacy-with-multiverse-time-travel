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
public class DATC_C : AdjudicationTestBase
{
    [Fact(DisplayName = "C.01. Three army circular movement")]
    public void DATC_C_1()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Turkey, UnitType.Fleet, "Ank"),
                (Nation.Turkey, UnitType.Army, "Con"),
                (Nation.Turkey, UnitType.Army, "Smy"),
            ]);

        var move1 = units.Get("Ank").Move("Con");
        var move2 = units.Get("Con").Move("Smy");
        var move3 = units.Get("Smy").Move("Ank");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        move1.Status.Should().Be(OrderStatus.Success);
        move2.Status.Should().Be(OrderStatus.Success);
        move3.Status.Should().Be(OrderStatus.Success);

        board.Next().ShouldHaveUnits(
            [
                (Nation.Turkey, UnitType.Fleet, "Con", false),
                (Nation.Turkey, UnitType.Army, "Smy", false),
                (Nation.Turkey, UnitType.Army, "Ank", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "C.02. Three army circular movement with support")]
    public void DATC_C_2()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Turkey, UnitType.Fleet, "Ank"),
                (Nation.Turkey, UnitType.Army, "Con"),
                (Nation.Turkey, UnitType.Army, "Smy"),
                (Nation.Turkey, UnitType.Army, "Bul"),
            ]);

        var move1 = units.Get("Ank").Move("Con");
        var move2 = units.Get("Con").Move("Smy");
        var move3 = units.Get("Smy").Move("Ank");
        var support = units.Get("Bul").Support(units.Get("Ank"), "Con");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        move1.Status.Should().Be(OrderStatus.Success);
        move2.Status.Should().Be(OrderStatus.Success);
        move3.Status.Should().Be(OrderStatus.Success);
        support.Status.Should().Be(OrderStatus.Success);

        board.Next().ShouldHaveUnits(
            [
                (Nation.Turkey, UnitType.Fleet, "Con", false),
                (Nation.Turkey, UnitType.Army, "Smy", false),
                (Nation.Turkey, UnitType.Army, "Ank", false),
                (Nation.Turkey, UnitType.Army, "Bul", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "C.03. A disrupted three army circular movement")]
    public void DATC_C_3()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Turkey, UnitType.Fleet, "Ank"),
                (Nation.Turkey, UnitType.Army, "Con"),
                (Nation.Turkey, UnitType.Army, "Smy"),
                (Nation.Turkey, UnitType.Army, "Bul"),
            ]);

        var move1 = units.Get("Ank").Move("Con");
        var move2 = units.Get("Con").Move("Smy");
        var move3 = units.Get("Smy").Move("Ank");
        var move4 = units.Get("Bul").Move("Con");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        move1.Status.Should().Be(OrderStatus.Failure);
        move2.Status.Should().Be(OrderStatus.Failure);
        move3.Status.Should().Be(OrderStatus.Failure);
        move4.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.Turkey, UnitType.Fleet, "Ank", false),
                (Nation.Turkey, UnitType.Army, "Con", false),
                (Nation.Turkey, UnitType.Army, "Smy", false),
                (Nation.Turkey, UnitType.Army, "Bul", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "C.04. A circular movement with attacked convoy")]
    public void DATC_C_4()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Austria, UnitType.Army, "Tri"),
                (Nation.Austria, UnitType.Army, "Ser"),
                (Nation.Turkey, UnitType.Army, "Bul"),
                (Nation.Turkey, UnitType.Fleet, "AEG"),
                (Nation.Turkey, UnitType.Fleet, "ION"),
                (Nation.Turkey, UnitType.Fleet, "ADR"),
                (Nation.Italy, UnitType.Fleet, "Nap"),
            ]);

        var austrianMove1 = units.Get("Tri").Move("Ser");
        var austrianMove2 = units.Get("Ser").Move("Bul");
        var turkishMove = units.Get("Bul").Move("Tri");
        var turkishConvoy1 = units.Get("AEG").Convoy(units.Get("Bul"), "Tri");
        var turkishConvoy2 = units.Get("ION").Convoy(units.Get("Bul"), "Tri");
        var turkishConvoy3 = units.Get("ADR").Convoy(units.Get("Bul"), "Tri");
        var italianMove = units.Get("Nap").Move("ION");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        austrianMove1.Status.Should().Be(OrderStatus.Success);
        austrianMove2.Status.Should().Be(OrderStatus.Success);
        turkishMove.Status.Should().Be(OrderStatus.Success);
        turkishConvoy1.Status.Should().Be(OrderStatus.Success);
        turkishConvoy2.Status.Should().Be(OrderStatus.Success);
        turkishConvoy3.Status.Should().Be(OrderStatus.Success);
        italianMove.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.Austria, UnitType.Army, "Ser", false),
                (Nation.Austria, UnitType.Army, "Bul", false),
                (Nation.Turkey, UnitType.Army, "Tri", false),
                (Nation.Turkey, UnitType.Fleet, "AEG", false),
                (Nation.Turkey, UnitType.Fleet, "ION", false),
                (Nation.Turkey, UnitType.Fleet, "ADR", false),
                (Nation.Italy, UnitType.Fleet, "Nap", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "C.05. A disrupted circular movement due to dislodged convoy")]
    public void DATC_C_5()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Austria, UnitType.Army, "Tri"),
                (Nation.Austria, UnitType.Army, "Ser"),
                (Nation.Turkey, UnitType.Army, "Bul"),
                (Nation.Turkey, UnitType.Fleet, "AEG"),
                (Nation.Turkey, UnitType.Fleet, "ION"),
                (Nation.Turkey, UnitType.Fleet, "ADR"),
                (Nation.Italy, UnitType.Fleet, "Nap"),
                (Nation.Italy, UnitType.Fleet, "Tun"),
            ]);

        var austrianMove1 = units.Get("Tri").Move("Ser");
        var austrianMove2 = units.Get("Ser").Move("Bul");
        var turkishMove = units.Get("Bul").Move("Tri");
        var turkishConvoy1 = units.Get("AEG").Convoy(units.Get("Bul"), "Tri");
        var turkishConvoy2 = units.Get("ION").Convoy(units.Get("Bul"), "Tri");
        var turkishConvoy3 = units.Get("ADR").Convoy(units.Get("Bul"), "Tri");
        var italianMove = units.Get("Nap").Move("ION");
        var italianSupport = units.Get("Tun").Support(units.Get("Nap"), "ION");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        austrianMove1.Status.Should().Be(OrderStatus.Failure);
        austrianMove2.Status.Should().Be(OrderStatus.Failure);
        turkishMove.Status.Should().Be(OrderStatus.Failure);
        turkishConvoy1.Status.Should().Be(OrderStatus.Failure);
        turkishConvoy2.Status.Should().Be(OrderStatus.Failure);
        turkishConvoy3.Status.Should().Be(OrderStatus.Failure);
        italianMove.Status.Should().Be(OrderStatus.Success);
        italianSupport.Status.Should().Be(OrderStatus.Success);

        board.ShouldHaveUnits(
            [
                (Nation.Austria, UnitType.Army, "Tri", false),
                (Nation.Austria, UnitType.Army, "Ser", false),
                (Nation.Turkey, UnitType.Army, "Bul", false),
                (Nation.Turkey, UnitType.Fleet, "AEG", false),
                (Nation.Turkey, UnitType.Fleet, "ION", true),
                (Nation.Turkey, UnitType.Fleet, "ADR", false),
                (Nation.Italy, UnitType.Fleet, "Nap", false),
                (Nation.Italy, UnitType.Fleet, "Tun", false),
            ]);
        board.ShouldNotHaveNextBoard();

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "C.06. Two armies with two convoys")]
    public void DATC_C_6()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH"),
                (Nation.England, UnitType.Army, "Lon"),
                (Nation.France, UnitType.Fleet, "ENG"),
                (Nation.France, UnitType.Army, "Bel"),
            ]);

        var englishMove = units.Get("Lon").Move("Bel");
        var englishConvoy = units.Get("NTH").Convoy(units.Get("Lon"), "Bel");
        var frenchMove = units.Get("Bel").Move("Lon");
        var frenchConvoy = units.Get("ENG").Convoy(units.Get("Bel"), "Lon");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        englishConvoy.Status.Should().Be(OrderStatus.Success);
        frenchMove.Status.Should().Be(OrderStatus.Success);
        frenchConvoy.Status.Should().Be(OrderStatus.Success);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH", false),
                (Nation.England, UnitType.Army, "Bel", false),
                (Nation.France, UnitType.Fleet, "ENG", false),
                (Nation.France, UnitType.Army, "Lon", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "C.07. Disrupted unit swap")]
    public void DATC_C_7()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH"),
                (Nation.England, UnitType.Army, "Lon"),
                (Nation.France, UnitType.Fleet, "ENG"),
                (Nation.France, UnitType.Army, "Bel"),
                (Nation.France, UnitType.Army, "Bur"),
            ]);

        var englishMove = units.Get("Lon").Move("Bel");
        var englishConvoy = units.Get("NTH").Convoy(units.Get("Lon"), "Bel");
        var frenchMove1 = units.Get("Bel").Move("Lon");
        var frenchMove2 = units.Get("Bur").Move("Bel");
        var frenchConvoy = units.Get("ENG").Convoy(units.Get("Bel"), "Lon");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Failure);
        englishConvoy.Status.Should().Be(OrderStatus.Failure);
        frenchMove1.Status.Should().Be(OrderStatus.Failure);
        frenchMove2.Status.Should().Be(OrderStatus.Failure);
        frenchConvoy.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH", false),
                (Nation.England, UnitType.Army, "Bel", false),
                (Nation.France, UnitType.Fleet, "ENG", false),
                (Nation.France, UnitType.Army, "Lon", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "C.08. No self dislodgement in disrupted circular movement")]
    public void DATC_C_8()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Turkey, UnitType.Fleet, "Con"),
                (Nation.Turkey, UnitType.Army, "Bul"),
                (Nation.Turkey, UnitType.Army, "Smy"),
                (Nation.Russia, UnitType.Fleet, "BLA"),
                (Nation.Austria, UnitType.Army, "Ser"),
            ]);

        var turkishMove1 = units.Get("Con").Move("BLA");
        var turkishMove2 = units.Get("Bul").Move("Con");
        var turkishSupport = units.Get("Smy").Support(units.Get("Bul"), "Con");
        var russianMove = units.Get("BLA").Move("Bul_S");
        var austrianMove = units.Get("Ser").Move("Bul");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        turkishMove1.Status.Should().Be(OrderStatus.Failure);
        turkishMove2.Status.Should().Be(OrderStatus.Failure);
        turkishSupport.Status.Should().Be(OrderStatus.Failure);
        russianMove.Status.Should().Be(OrderStatus.Failure);
        austrianMove.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.Turkey, UnitType.Fleet, "Con", false),
                (Nation.Turkey, UnitType.Army, "Bul", false),
                (Nation.Turkey, UnitType.Army, "Smy", false),
                (Nation.Russia, UnitType.Fleet, "BLA", false),
                (Nation.Austria, UnitType.Army, "Ser", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "C.09. No help in dislodgement of own unit in disrupted circular movement")]
    public void DATC_C_9()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Turkey, UnitType.Fleet, "Con"),
                (Nation.Turkey, UnitType.Army, "Smy"),
                (Nation.Russia, UnitType.Fleet, "BLA"),
                (Nation.Austria, UnitType.Army, "Ser"),
                (Nation.Austria, UnitType.Army, "Bul"),
            ]);

        var turkishMove = units.Get("Con").Move("BLA");
        var turkishSupport = units.Get("Smy").Support(units.Get("Bul"), "Con");
        var russianMove = units.Get("BLA").Move("Bul_S");
        var austrianMove1 = units.Get("Ser").Move("Bul");
        var austrianMove2 = units.Get("Bul").Move("Con");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        turkishMove.Status.Should().Be(OrderStatus.Failure);
        turkishSupport.Status.Should().Be(OrderStatus.Failure);
        russianMove.Status.Should().Be(OrderStatus.Failure);
        austrianMove1.Status.Should().Be(OrderStatus.Failure);
        austrianMove2.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.Turkey, UnitType.Fleet, "Con", false),
                (Nation.Turkey, UnitType.Army, "Smy", false),
                (Nation.Russia, UnitType.Fleet, "BLA", false),
                (Nation.Austria, UnitType.Army, "Ser", false),
                (Nation.Austria, UnitType.Army, "Bul", false),
            ]);

        world.ShouldHaveAllOrdersResolved();
    }
}
