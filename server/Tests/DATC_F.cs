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
public class DATC_F : AdjudicationTestBase
{
    [Fact(DisplayName = "F.01. No convoys in coastal areas")]
    public void DATC_F_1()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Turkey, UnitType.Army, "Gre"),
                (Nation.Turkey, UnitType.Fleet, "AEG"),
                (Nation.Turkey, UnitType.Fleet, "Con"),
                (Nation.Turkey, UnitType.Fleet, "BLA"),
            ]);

        var move = units.Get("Gre").Move("Sev");
        var convoy1 = units.Get("AEG").Convoy(units.Get("Gre"), "Sev");
        var convoy2 = units.Get("Con").Convoy(units.Get("Gre"), "Sev");
        var convoy3 = units.Get("BLA").Convoy(units.Get("Gre"), "Sev");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        move.Status.Should().Be(OrderStatus.Invalid);
        convoy1.Status.Should().Be(OrderStatus.Invalid);
        convoy2.Status.Should().Be(OrderStatus.Invalid);
        convoy3.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits(
            [
                (Nation.Turkey, UnitType.Army, "Gre", false),
                (Nation.Turkey, UnitType.Fleet, "AEG", false),
                (Nation.Turkey, UnitType.Fleet, "Con", false),
                (Nation.Turkey, UnitType.Fleet, "BLA", false),
            ]);
    }

    [Fact(DisplayName = "F.02. An army being convoyed can bounce as normal")]
    public void DATC_F_2()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "ENG"),
                (Nation.England, UnitType.Army, "Lon"),
                (Nation.France, UnitType.Army, "Par"),
            ]);

        var englishMove = units.Get("Lon").Move("Bre");
        var englishConvoy = units.Get("ENG").Convoy(units.Get("Lon"), "Bre");
        var frenchMove = units.Get("Par").Move("Bre");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Failure);
        englishConvoy.Status.Should().Be(OrderStatus.Failure);
        frenchMove.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "ENG", false),
                (Nation.England, UnitType.Army, "Lon", false),
                (Nation.France, UnitType.Army, "Par", false),
            ]);
    }

    [Fact(DisplayName = "F.03. An army being convoyed can receive support")]
    public void DATC_F_3()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "ENG"),
                (Nation.England, UnitType.Army, "Lon"),
                (Nation.England, UnitType.Fleet, "MAO"),
                (Nation.France, UnitType.Army, "Par"),
            ]);

        var englishMove = units.Get("Lon").Move("Bre");
        var englishSupport = units.Get("MAO").Support(units.Get("Lon"), "Bre");
        var englishConvoy = units.Get("ENG").Convoy(units.Get("Lon"), "Bre");
        var frenchMove = units.Get("Par").Move("Bre");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        englishSupport.Status.Should().Be(OrderStatus.Success);
        englishConvoy.Status.Should().Be(OrderStatus.Success);
        frenchMove.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "ENG", false),
                (Nation.England, UnitType.Army, "Bre", false),
                (Nation.England, UnitType.Fleet, "MAO", false),
                (Nation.France, UnitType.Army, "Par", false),
            ]);
    }

    [Fact(DisplayName = "F.04. An attacked convoy is not disrupted")]
    public void DATC_F_4()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH"),
                (Nation.England, UnitType.Army, "Lon"),
                (Nation.Germany, UnitType.Fleet, "SKA"),
            ]);

        var englishMove = units.Get("Lon").Move("Hol");
        var englishConvoy = units.Get("NTH").Convoy(units.Get("Lon"), "Hol");
        var germanMove = units.Get("SKA").Move("NTH");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        englishConvoy.Status.Should().Be(OrderStatus.Success);
        germanMove.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH", false),
                (Nation.England, UnitType.Army, "Hol", false),
                (Nation.Germany, UnitType.Fleet, "SKA", false),
            ]);
    }

    [Fact(DisplayName = "F.05. A beleaguered convoy is not disrupted")]
    public void DATC_F_5()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH"),
                (Nation.England, UnitType.Army, "Lon"),
                (Nation.France, UnitType.Fleet, "ENG"),
                (Nation.France, UnitType.Fleet, "Bel"),
                (Nation.Germany, UnitType.Fleet, "SKA"),
                (Nation.Germany, UnitType.Fleet, "Den"),
            ]);

        var englishMove = units.Get("Lon").Move("Hol");
        var englishConvoy = units.Get("NTH").Convoy(units.Get("Lon"), "Hol");
        var frenchMove = units.Get("ENG").Move("NTH");
        var frenchSupport = units.Get("Bel").Support(units.Get("ENG"), "NTH");
        var germanMove = units.Get("SKA").Move("NTH");
        var germanSupport = units.Get("Den").Support(units.Get("SKA"), "NTH");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        englishConvoy.Status.Should().Be(OrderStatus.Success);
        frenchMove.Status.Should().Be(OrderStatus.Failure);
        frenchSupport.Status.Should().Be(OrderStatus.Failure);
        germanMove.Status.Should().Be(OrderStatus.Failure);
        germanSupport.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH", false),
                (Nation.England, UnitType.Army, "Hol", false),
                (Nation.France, UnitType.Fleet, "ENG", false),
                (Nation.France, UnitType.Fleet, "Bel", false),
                (Nation.Germany, UnitType.Fleet, "SKA", false),
                (Nation.Germany, UnitType.Fleet, "Den", false),
            ]);
    }

    [Fact(DisplayName = "F.06. Dislodged convoy does not cut support")]
    public void DATC_F_6()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH"),
                (Nation.England, UnitType.Army, "Lon"),
                (Nation.Germany, UnitType.Army, "Hol"),
                (Nation.Germany, UnitType.Army, "Bel"),
                (Nation.Germany, UnitType.Fleet, "HEL"),
                (Nation.Germany, UnitType.Fleet, "SKA"),
                (Nation.France, UnitType.Army, "Pic"),
                (Nation.France, UnitType.Army, "Bur"),
            ]);

        var englishMove = units.Get("Lon").Move("Hol");
        var englishConvoy = units.Get("NTH").Convoy(units.Get("Lon"), "Hol");
        var germanMove = units.Get("SKA").Move("NTH");
        var germanSupport1 = units.Get("Hol").Support(units.Get("Bel"));
        var germanSupport2 = units.Get("Bel").Support(units.Get("Hol"));
        var germanSupport3 = units.Get("HEL").Support(units.Get("SKA"), "NTH");
        var frenchMove = units.Get("Pic").Move("Bel");
        var frenchSupport = units.Get("Bur").Support(units.Get("Pic"), "Bel");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Failure);
        englishConvoy.Status.Should().Be(OrderStatus.Failure);
        germanMove.Status.Should().Be(OrderStatus.Success);
        germanSupport1.Status.Should().Be(OrderStatus.Success);
        germanSupport2.Status.Should().Be(OrderStatus.Failure);
        germanSupport3.Status.Should().Be(OrderStatus.Success);
        frenchMove.Status.Should().Be(OrderStatus.Failure);
        frenchSupport.Status.Should().Be(OrderStatus.Failure);

        board.ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH", true),
                (Nation.England, UnitType.Army, "Lon", false),
                (Nation.Germany, UnitType.Army, "Hol", false),
                (Nation.Germany, UnitType.Army, "Bel", false),
                (Nation.Germany, UnitType.Fleet, "HEL", false),
                (Nation.Germany, UnitType.Fleet, "SKA", false),
                (Nation.France, UnitType.Army, "Pic", false),
                (Nation.France, UnitType.Army, "Bur", false),
            ]);
        board.ShouldNotHaveNextBoard();
    }

    [Fact(DisplayName = "F.07. Dislodged convoy does not cause contested area")]
    public void DATC_F_7()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH"),
                (Nation.England, UnitType.Army, "Lon"),
                (Nation.Germany, UnitType.Fleet, "HEL"),
                (Nation.Germany, UnitType.Fleet, "SKA"),
            ]);

        var englishMove = units.Get("Lon").Move("Hol");
        var englishConvoy = units.Get("NTH").Convoy(units.Get("Lon"), "Hol");
        var germanMove = units.Get("SKA").Move("NTH");
        var germanSupport = units.Get("HEL").Support(units.Get("SKA"), "NTH");

        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        englishMove.Status.Should().Be(OrderStatus.Failure);
        englishConvoy.Status.Should().Be(OrderStatus.Failure);
        germanMove.Status.Should().Be(OrderStatus.Success);
        germanSupport.Status.Should().Be(OrderStatus.Success);

        board.ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH", true),
                (Nation.England, UnitType.Army, "Lon", false),
                (Nation.Germany, UnitType.Fleet, "HEL", false),
                (Nation.Germany, UnitType.Fleet, "SKA", false),
            ]);
        board.ShouldNotHaveNextBoard();

        var englishRetreat = units.Get("NTH").Move("Hol");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishRetreat.Status.Should().Be(OrderStatus.Retreat);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "Hol", false),
                (Nation.England, UnitType.Army, "Lon", false),
                (Nation.Germany, UnitType.Fleet, "HEL", false),
                (Nation.Germany, UnitType.Fleet, "SKA", false),
            ]);
    }

    [Fact(DisplayName = "F.08. Dislodged convoy does not cause a bounce")]
    public void DATC_F_8()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH"),
                (Nation.England, UnitType.Army, "Lon"),
                (Nation.Germany, UnitType.Fleet, "HEL"),
                (Nation.Germany, UnitType.Fleet, "SKA"),
                (Nation.Germany, UnitType.Army, "Bel"),
            ]);

        var englishMove = units.Get("Lon").Move("Hol");
        var englishConvoy = units.Get("NTH").Convoy(units.Get("Lon"), "Hol");
        var germanMove1 = units.Get("SKA").Move("NTH");
        var germanMove2 = units.Get("Bel").Move("Hol");
        var germanSupport = units.Get("HEL").Support(units.Get("SKA"), "NTH");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Failure);
        englishConvoy.Status.Should().Be(OrderStatus.Failure);
        germanMove1.Status.Should().Be(OrderStatus.Success);
        germanMove2.Status.Should().Be(OrderStatus.Success);
        germanSupport.Status.Should().Be(OrderStatus.Success);

        board.ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH", true),
                (Nation.England, UnitType.Army, "Lon", false),
                (Nation.Germany, UnitType.Fleet, "HEL", false),
                (Nation.Germany, UnitType.Fleet, "SKA", false),
                (Nation.Germany, UnitType.Army, "Bel", false),
            ]);
        board.ShouldNotHaveNextBoard();
    }

    [Fact(DisplayName = "F.09. Dislodge of multi-route convoy")]
    public void DATC_F_9()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH"),
                (Nation.England, UnitType.Army, "Lon"),
                (Nation.England, UnitType.Fleet, "ENG"),
                (Nation.France, UnitType.Fleet, "Bre"),
                (Nation.France, UnitType.Fleet, "MAO"),
            ]);

        var englishMove = units.Get("Lon").Move("Bel");
        var englishConvoy1 = units.Get("NTH").Convoy(units.Get("Lon"), "Bel");
        var englishConvoy2 = units.Get("ENG").Convoy(units.Get("Lon"), "Bel");
        var frenchMove = units.Get("MAO").Move("ENG");
        var frenchSupport = units.Get("Bre").Support(units.Get("MAO"), "ENG");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        englishConvoy1.Status.Should().Be(OrderStatus.Success);
        englishConvoy2.Status.Should().Be(OrderStatus.Failure);
        frenchMove.Status.Should().Be(OrderStatus.Success);
        frenchSupport.Status.Should().Be(OrderStatus.Success);

        board.ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH", false),
                (Nation.England, UnitType.Army, "Lon", false),
                (Nation.England, UnitType.Fleet, "ENG", true),
                (Nation.France, UnitType.Fleet, "Bre", false),
                (Nation.France, UnitType.Fleet, "MAO", false),
            ]);
        board.ShouldNotHaveNextBoard();
    }

    [Fact(DisplayName = "F.10. Dislodge of multi-route convoy with foreign fleet")]
    public void DATC_F_10()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH"),
                (Nation.England, UnitType.Army, "Lon"),
                (Nation.Germany, UnitType.Fleet, "ENG"),
                (Nation.France, UnitType.Fleet, "Bre"),
                (Nation.France, UnitType.Fleet, "MAO"),
            ]);

        var englishMove = units.Get("Lon").Move("Bel");
        var englishConvoy = units.Get("NTH").Convoy(units.Get("Lon"), "Bel");
        var germanConvoy = units.Get("ENG").Convoy(units.Get("Lon"), "Bel");
        var frenchMove = units.Get("MAO").Move("ENG");
        var frenchSupport = units.Get("Bre").Support(units.Get("MAO"), "ENG");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        englishConvoy.Status.Should().Be(OrderStatus.Success);
        germanConvoy.Status.Should().Be(OrderStatus.Failure);
        frenchMove.Status.Should().Be(OrderStatus.Success);
        frenchSupport.Status.Should().Be(OrderStatus.Success);

        board.ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH", false),
                (Nation.England, UnitType.Army, "Lon", false),
                (Nation.Germany, UnitType.Fleet, "ENG", true),
                (Nation.France, UnitType.Fleet, "Bre", false),
                (Nation.France, UnitType.Fleet, "MAO", false),
            ]);
        board.ShouldNotHaveNextBoard();
    }

    [Fact(DisplayName = "F.11. Dislodge of multi-route convoy with only foreign fleets")]
    public void DATC_F_11()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Army, "Lon"),
                (Nation.Germany, UnitType.Fleet, "ENG"),
                (Nation.Russia, UnitType.Fleet, "NTH"),
                (Nation.France, UnitType.Fleet, "Bre"),
                (Nation.France, UnitType.Fleet, "MAO"),
            ]);

        var englishMove = units.Get("Lon").Move("Bel");
        var germanConvoy = units.Get("ENG").Convoy(units.Get("Lon"), "Bel");
        var russianConvoy = units.Get("NTH").Convoy(units.Get("Lon"), "Bel");
        var frenchMove = units.Get("MAO").Move("ENG");
        var frenchSupport = units.Get("Bre").Support(units.Get("MAO"), "ENG");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        germanConvoy.Status.Should().Be(OrderStatus.Failure);
        russianConvoy.Status.Should().Be(OrderStatus.Success);
        frenchMove.Status.Should().Be(OrderStatus.Success);
        frenchSupport.Status.Should().Be(OrderStatus.Success);

        board.ShouldHaveUnits(
            [
                (Nation.England, UnitType.Army, "Lon", false),
                (Nation.Germany, UnitType.Fleet, "ENG", true),
                (Nation.Russia, UnitType.Fleet, "NTH", false),
                (Nation.France, UnitType.Fleet, "Bre", false),
                (Nation.France, UnitType.Fleet, "MAO", false),
            ]);
        board.ShouldNotHaveNextBoard();
    }

    [Fact(DisplayName = "F.12. Dislodged convoying fleet not on route")]
    public void DATC_F_12()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "ENG"),
                (Nation.England, UnitType.Army, "Lon"),
                (Nation.England, UnitType.Fleet, "IRI"),
                (Nation.France, UnitType.Fleet, "NAO"),
                (Nation.France, UnitType.Fleet, "MAO"),
            ]);

        var englishMove = units.Get("Lon").Move("Bel");
        var englishConvoy1 = units.Get("ENG").Convoy(units.Get("Lon"), "Bel");
        var englishConvoy2 = units.Get("IRI").Convoy(units.Get("Lon"), "Bel");
        var frenchMove = units.Get("MAO").Move("IRI");
        var frenchSupport = units.Get("NAO").Support(units.Get("MAO"), "IRI");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        englishConvoy1.Status.Should().Be(OrderStatus.Success);
        englishConvoy2.Status.Should().Be(OrderStatus.Invalid);
        frenchMove.Status.Should().Be(OrderStatus.Success);
        frenchSupport.Status.Should().Be(OrderStatus.Success);

        board.ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "ENG", false),
                (Nation.England, UnitType.Army, "Lon", false),
                (Nation.England, UnitType.Fleet, "IRI", true),
                (Nation.France, UnitType.Fleet, "NAO", false),
                (Nation.France, UnitType.Fleet, "MAO", false),
            ]);
        board.ShouldNotHaveNextBoard();
    }

    [Fact(DisplayName = "F.13. The unwanted alternative")]
    public void DATC_F_13()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Army, "Lon"),
                (Nation.England, UnitType.Fleet, "NTH"),
                (Nation.France, UnitType.Fleet, "ENG"),
                (Nation.Germany, UnitType.Fleet, "Hol"),
                (Nation.Germany, UnitType.Fleet, "Den"),
            ]);

        var englishMove = units.Get("Lon").Move("Bel");
        var englishConvoy = units.Get("NTH").Convoy(units.Get("Lon"), "Bel");
        var frenchConvoy = units.Get("ENG").Convoy(units.Get("Lon"), "Bel");
        var germanMove = units.Get("Den").Move("NTH");
        var germanSupport = units.Get("Hol").Support(units.Get("Den"), "NTH");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        englishConvoy.Status.Should().Be(OrderStatus.Success);
        frenchConvoy.Status.Should().Be(OrderStatus.Failure);
        germanMove.Status.Should().Be(OrderStatus.Success);
        germanSupport.Status.Should().Be(OrderStatus.Success);

        board.ShouldHaveUnits(
            [
                (Nation.England, UnitType.Army, "Lon", false),
                (Nation.England, UnitType.Fleet, "NTH", false),
                (Nation.France, UnitType.Fleet, "ENG", true),
                (Nation.Germany, UnitType.Fleet, "Hol", false),
                (Nation.Germany, UnitType.Fleet, "Den", false),
            ]);
        board.ShouldNotHaveNextBoard();
    }

    [Fact(DisplayName = "F.14. Simple convoy paradox")]
    public void DATC_F_14()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "Lon"),
                (Nation.England, UnitType.Fleet, "Wal"),
                (Nation.France, UnitType.Army, "Bre"),
                (Nation.France, UnitType.Fleet, "ENG"),
            ]);

        var englishMove = units.Get("Wal").Move("ENG");
        var englishSupport = units.Get("Lon").Support(units.Get("Wal"), "ENG");
        var frenchMove = units.Get("Bre").Move("Lon");
        var frenchConvoy = units.Get("ENG").Convoy(units.Get("Bre"), "Lon");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        englishSupport.Status.Should().Be(OrderStatus.Success);
        frenchMove.Status.Should().Be(OrderStatus.Failure);
        frenchConvoy.Status.Should().Be(OrderStatus.Failure);

        board.ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "Lon", false),
                (Nation.England, UnitType.Fleet, "Wal", false),
                (Nation.France, UnitType.Army, "Bre", false),
                (Nation.France, UnitType.Fleet, "ENG", true),
            ]);
        board.ShouldNotHaveNextBoard();
    }

    [Fact(DisplayName = "F.15. Simple convoy paradox with additional convoy")]
    public void DATC_F_15()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "Lon"),
                (Nation.England, UnitType.Fleet, "Wal"),
                (Nation.France, UnitType.Army, "Bre"),
                (Nation.France, UnitType.Fleet, "ENG"),
                (Nation.Italy, UnitType.Fleet, "IRI"),
                (Nation.Italy, UnitType.Fleet, "MAO"),
                (Nation.Italy, UnitType.Army, "Naf"),
            ]);

        var englishMove = units.Get("Wal").Move("ENG");
        var englishSupport = units.Get("Lon").Support(units.Get("Wal"), "ENG");
        var frenchMove = units.Get("Bre").Move("Lon");
        var frenchConvoy = units.Get("ENG").Convoy(units.Get("Bre"), "Lon");
        var italianMove = units.Get("Naf").Move("Wal");
        var italianConvoy1 = units.Get("MAO").Convoy(units.Get("Naf"), "Wal");
        var italianConvoy2 = units.Get("IRI").Convoy(units.Get("Naf"), "Wal");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        englishSupport.Status.Should().Be(OrderStatus.Success);
        frenchMove.Status.Should().Be(OrderStatus.Failure);
        frenchConvoy.Status.Should().Be(OrderStatus.Failure);
        italianMove.Status.Should().Be(OrderStatus.Success);
        italianConvoy1.Status.Should().Be(OrderStatus.Success);
        italianConvoy2.Status.Should().Be(OrderStatus.Success);

        board.ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "Lon", false),
                (Nation.England, UnitType.Fleet, "Wal", false),
                (Nation.France, UnitType.Army, "Bre", false),
                (Nation.France, UnitType.Fleet, "ENG", true),
                (Nation.Italy, UnitType.Fleet, "IRI", false),
                (Nation.Italy, UnitType.Fleet, "MAO", false),
                (Nation.Italy, UnitType.Army, "Naf", false),
            ]);
        board.ShouldNotHaveNextBoard();
    }

    [Fact(DisplayName = "F.16. Pandin's paradox")]
    public void DATC_F_16()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "Lon"),
                (Nation.England, UnitType.Fleet, "Wal"),
                (Nation.France, UnitType.Army, "Bre"),
                (Nation.France, UnitType.Fleet, "ENG"),
                (Nation.Germany, UnitType.Fleet, "NTH"),
                (Nation.Germany, UnitType.Fleet, "Bel"),
            ]);

        var englishMove = units.Get("Wal").Move("ENG");
        var englishSupport = units.Get("Lon").Support(units.Get("Wal"), "ENG");
        var frenchMove = units.Get("Bre").Move("Lon");
        var frenchConvoy = units.Get("ENG").Convoy(units.Get("Bre"), "Lon");
        var germanMove = units.Get("Bel").Move("ENG");
        var germanSupport = units.Get("NTH").Support(units.Get("Bel"), "ENG");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Failure);
        englishSupport.Status.Should().Be(OrderStatus.Failure);
        frenchMove.Status.Should().Be(OrderStatus.Failure);
        frenchConvoy.Status.Should().Be(OrderStatus.Failure);
        germanMove.Status.Should().Be(OrderStatus.Failure);
        germanSupport.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "Lon", false),
                (Nation.England, UnitType.Fleet, "Wal", false),
                (Nation.France, UnitType.Army, "Bre", false),
                (Nation.France, UnitType.Fleet, "ENG", false),
                (Nation.Germany, UnitType.Fleet, "NTH", false),
                (Nation.Germany, UnitType.Fleet, "Bel", false),
            ]);
    }

    [Fact(DisplayName = "F.17. Pandin's extended paradox")]
    public void DATC_F_17()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "Lon"),
                (Nation.England, UnitType.Fleet, "Wal"),
                (Nation.France, UnitType.Army, "Bre"),
                (Nation.France, UnitType.Fleet, "ENG"),
                (Nation.France, UnitType.Fleet, "Yor"),
                (Nation.Germany, UnitType.Fleet, "NTH"),
                (Nation.Germany, UnitType.Fleet, "Bel"),
            ]);

        var englishMove = units.Get("Wal").Move("ENG");
        var englishSupport = units.Get("Lon").Support(units.Get("Wal"), "ENG");
        var frenchMove = units.Get("Bre").Move("Lon");
        var frenchSupport = units.Get("Yor").Support(units.Get("Bre"), "Lon");
        var frenchConvoy = units.Get("ENG").Convoy(units.Get("Bre"), "Lon");
        var germanMove = units.Get("Bel").Move("ENG");
        var germanSupport = units.Get("NTH").Support(units.Get("Bel"), "ENG");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Failure);
        englishSupport.Status.Should().Be(OrderStatus.Failure);
        frenchMove.Status.Should().Be(OrderStatus.Failure);
        frenchSupport.Status.Should().Be(OrderStatus.Failure);
        frenchConvoy.Status.Should().Be(OrderStatus.Failure);
        germanMove.Status.Should().Be(OrderStatus.Failure);
        germanSupport.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "Lon", false),
                (Nation.England, UnitType.Fleet, "Wal", false),
                (Nation.France, UnitType.Army, "Bre", false),
                (Nation.France, UnitType.Fleet, "ENG", false),
                (Nation.Germany, UnitType.Fleet, "NTH", false),
                (Nation.Germany, UnitType.Fleet, "Bel", false),
            ]);
    }

    [Fact(DisplayName = "F.18. Betrayal paradox")]
    public void DATC_F_18()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH"),
                (Nation.England, UnitType.Army, "Lon"),
                (Nation.England, UnitType.Fleet, "ENG"),
                (Nation.France, UnitType.Fleet, "Bel"),
                (Nation.Germany, UnitType.Fleet, "HEL"),
                (Nation.Germany, UnitType.Fleet, "SKA"),
            ]);

        var englishMove = units.Get("Lon").Move("Bel");
        var englishSupport = units.Get("ENG").Support(units.Get("Lon"), "Bel");
        var englishConvoy = units.Get("NTH").Convoy(units.Get("Lon"), "Bel");
        var frenchSupport = units.Get("Bel").Support(units.Get("NTH"));
        var germanMove = units.Get("SKA").Move("NTH");
        var germanSupport = units.Get("HEL").Support(units.Get("SKA"), "NTH");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Failure);
        englishSupport.Status.Should().Be(OrderStatus.Failure);
        englishConvoy.Status.Should().Be(OrderStatus.Failure);
        frenchSupport.Status.Should().Be(OrderStatus.Success);
        germanMove.Status.Should().Be(OrderStatus.Failure);
        germanSupport.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH", false),
                (Nation.England, UnitType.Army, "Lon", false),
                (Nation.England, UnitType.Fleet, "ENG", false),
                (Nation.France, UnitType.Fleet, "Bel", false),
                (Nation.Germany, UnitType.Fleet, "HEL", false),
                (Nation.Germany, UnitType.Fleet, "SKA", false),
            ]);
    }

    [Fact(DisplayName = "F.19. Multi-route convoy disruption paradox")]
    public void DATC_F_19()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.France, UnitType.Army, "Tun"),
                (Nation.France, UnitType.Fleet, "TYS"),
                (Nation.France, UnitType.Fleet, "ION"),
                (Nation.Italy, UnitType.Fleet, "Nap"),
                (Nation.Italy, UnitType.Fleet, "Rom"),
            ]);

        var frenchMove = units.Get("Tun").Move("Nap");
        var frenchConvoy1 = units.Get("TYS").Convoy(units.Get("Tun"), "Nap");
        var frenchConvoy2 = units.Get("ION").Convoy(units.Get("Tun"), "Nap");
        var italianMove = units.Get("Rom").Move("TYS");
        var italianSupport = units.Get("Nap").Support(units.Get("Rom"), "TYS");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        frenchMove.Status.Should().Be(OrderStatus.Failure);
        frenchConvoy1.Status.Should().Be(OrderStatus.Failure);
        frenchConvoy2.Status.Should().Be(OrderStatus.Failure);
        italianMove.Status.Should().Be(OrderStatus.Failure);
        italianSupport.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.France, UnitType.Army, "Tun", false),
                (Nation.France, UnitType.Fleet, "TYS", false),
                (Nation.France, UnitType.Fleet, "ION", false),
                (Nation.Italy, UnitType.Fleet, "Nap", false),
                (Nation.Italy, UnitType.Fleet, "Rom", false),
            ]);
    }

    [Fact(DisplayName = "F.20. Unwanted multi-route convoy paradox")]
    public void DATC_F_20()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.France, UnitType.Army, "Tun"),
                (Nation.France, UnitType.Fleet, "TYS"),
                (Nation.Italy, UnitType.Fleet, "Nap"),
                (Nation.Italy, UnitType.Fleet, "ION"),
                (Nation.Turkey, UnitType.Fleet, "AEG"),
                (Nation.Turkey, UnitType.Fleet, "EAS"),
            ]);

        var frenchMove = units.Get("Tun").Move("Nap");
        var frenchConvoy = units.Get("TYS").Convoy(units.Get("Tun"), "Nap");
        var italianSupport = units.Get("Nap").Support(units.Get("ION"));
        var italianConvoy = units.Get("ION").Convoy(units.Get("Tun"), "Nap");
        var turkishMove = units.Get("EAS").Move("ION");
        var turkishSupport = units.Get("AEG").Support(units.Get("EAS"), "ION");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        frenchMove.Status.Should().Be(OrderStatus.Failure);
        frenchConvoy.Status.Should().Be(OrderStatus.Failure);
        italianSupport.Status.Should().Be(OrderStatus.Failure);
        italianConvoy.Status.Should().Be(OrderStatus.Failure);
        turkishMove.Status.Should().Be(OrderStatus.Success);
        turkishSupport.Status.Should().Be(OrderStatus.Success);

        board.ShouldHaveUnits(
            [
                (Nation.France, UnitType.Army, "Tun", false),
                (Nation.France, UnitType.Fleet, "TYS", false),
                (Nation.Italy, UnitType.Fleet, "Nap", false),
                (Nation.Italy, UnitType.Fleet, "ION", true),
                (Nation.Turkey, UnitType.Fleet, "AEG", false),
                (Nation.Turkey, UnitType.Fleet, "EAS", false),
            ]);
        board.ShouldNotHaveNextBoard();
    }

    [Fact(DisplayName = "F.21. Dad's army convoy")]
    public void DATC_F_21()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Russia, UnitType.Army, "Edi"),
                (Nation.Russia, UnitType.Fleet, "NWG"),
                (Nation.Russia, UnitType.Army, "Nwy"),
                (Nation.France, UnitType.Fleet, "IRI"),
                (Nation.France, UnitType.Fleet, "MAO"),
                (Nation.England, UnitType.Army, "Lvp"),
                (Nation.England, UnitType.Fleet, "NAO"),
                (Nation.England, UnitType.Fleet, "Cly"),
            ]);

        var russianMove = units.Get("Nwy").Move("Cly");
        var russianSupport = units.Get("Edi").Support(units.Get("Nwy"), "Cly");
        var russianConvoy = units.Get("NWG").Convoy(units.Get("Nwy"), "Cly");
        var frenchMove = units.Get("MAO").Move("NAO");
        var frenchSupport = units.Get("IRI").Support(units.Get("MAO"), "NAO");
        var englishMove = units.Get("Lvp").Move("Cly");
        var englishSupport = units.Get("Cly").Support(units.Get("NAO"));
        var englishConvoy = units.Get("NAO").Convoy(units.Get("Lvp"), "Cly");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        russianMove.Status.Should().Be(OrderStatus.Success);
        russianSupport.Status.Should().Be(OrderStatus.Success);
        russianConvoy.Status.Should().Be(OrderStatus.Success);
        frenchMove.Status.Should().Be(OrderStatus.Success);
        frenchSupport.Status.Should().Be(OrderStatus.Success);
        englishMove.Status.Should().Be(OrderStatus.Failure);
        englishSupport.Status.Should().Be(OrderStatus.Failure);
        englishConvoy.Status.Should().Be(OrderStatus.Failure);

        board.ShouldHaveUnits(
            [
                (Nation.Russia, UnitType.Army, "Edi", true),
                (Nation.Russia, UnitType.Fleet, "NWG", false),
                (Nation.Russia, UnitType.Army, "Nwy", false),
                (Nation.France, UnitType.Fleet, "IRI", false),
                (Nation.France, UnitType.Fleet, "MAO", false),
                (Nation.England, UnitType.Army, "Lvp", false),
                (Nation.England, UnitType.Fleet, "NAO", true),
                (Nation.England, UnitType.Fleet, "Cly", false),
            ]);
        board.ShouldNotHaveNextBoard();
    }

    [Fact(DisplayName = "F.22. Second order paradox with two resolutions")]
    public void DATC_F_22()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "Edi"),
                (Nation.England, UnitType.Fleet, "Lon"),
                (Nation.France, UnitType.Army, "Bre"),
                (Nation.France, UnitType.Fleet, "ENG"),
                (Nation.Germany, UnitType.Fleet, "Bel"),
                (Nation.Germany, UnitType.Fleet, "Pic"),
                (Nation.Russia, UnitType.Army, "Nwy"),
                (Nation.Russia, UnitType.Fleet, "NTH"),
            ]);

        var englishMove = units.Get("Edi").Move("NTH");
        var englishSupport = units.Get("Lon").Support(units.Get("Edi"), "NTH");
        var frenchMove = units.Get("Bre").Move("Lon");
        var frenchConvoy = units.Get("ENG").Convoy(units.Get("Bre"), "Lon");
        var germanMove = units.Get("Pic").Move("ENG");
        var germanSupport = units.Get("Bel").Support(units.Get("Pic"), "ENG");
        var russianMove = units.Get("Nwy").Move("Bel");
        var russianConvoy = units.Get("NTH").Convoy(units.Get("Nwy"), "Bel");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        englishSupport.Status.Should().Be(OrderStatus.Success);
        frenchMove.Status.Should().Be(OrderStatus.Failure);
        frenchConvoy.Status.Should().Be(OrderStatus.Failure);
        germanMove.Status.Should().Be(OrderStatus.Success);
        germanSupport.Status.Should().Be(OrderStatus.Success);
        russianMove.Status.Should().Be(OrderStatus.Failure);
        russianConvoy.Status.Should().Be(OrderStatus.Failure);

        board.ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "Edi", false),
                (Nation.England, UnitType.Fleet, "Lon", false),
                (Nation.France, UnitType.Army, "Bre", false),
                (Nation.France, UnitType.Fleet, "ENG", true),
                (Nation.Germany, UnitType.Fleet, "Bel", false),
                (Nation.Germany, UnitType.Fleet, "Pic", false),
                (Nation.Russia, UnitType.Army, "Nwy", false),
                (Nation.Russia, UnitType.Fleet, "NTH", true),
            ]);
        board.ShouldNotHaveNextBoard();
    }

    [Fact(DisplayName = "F.23. Second order paradox with two exclusive convoys")]
    public void DATC_F_23()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "Edi"),
                (Nation.England, UnitType.Fleet, "Yor"),
                (Nation.France, UnitType.Army, "Bre"),
                (Nation.France, UnitType.Fleet, "ENG"),
                (Nation.Germany, UnitType.Fleet, "Bel"),
                (Nation.Germany, UnitType.Fleet, "Lon"),
                (Nation.Italy, UnitType.Fleet, "MAO"),
                (Nation.Italy, UnitType.Fleet, "IRI"),
                (Nation.Russia, UnitType.Army, "Nwy"),
                (Nation.Russia, UnitType.Fleet, "NTH"),
            ]);

        var englishMove = units.Get("Edi").Move("NTH");
        var englishSupport = units.Get("Yor").Support(units.Get("Edi"), "NTH");
        var frenchMove = units.Get("Bre").Move("Lon");
        var frenchConvoy = units.Get("ENG").Convoy(units.Get("Bre"), "Lon");
        var germanSupport1 = units.Get("Bel").Support(units.Get("ENG"));
        var germanSupport2 = units.Get("Lon").Support(units.Get("NTH"));
        var italianMove = units.Get("MAO").Move("ENG");
        var italianSupport = units.Get("IRI").Support(units.Get("MAO"), "ENG");
        var russianMove = units.Get("Nwy").Move("Bel");
        var russianConvoy = units.Get("NTH").Convoy(units.Get("Nwy"), "Bel");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Failure);
        englishSupport.Status.Should().Be(OrderStatus.Failure);
        frenchMove.Status.Should().Be(OrderStatus.Failure);
        frenchConvoy.Status.Should().Be(OrderStatus.Failure);
        germanSupport1.Status.Should().Be(OrderStatus.Success);
        germanSupport2.Status.Should().Be(OrderStatus.Success);
        italianMove.Status.Should().Be(OrderStatus.Failure);
        italianSupport.Status.Should().Be(OrderStatus.Failure);
        russianMove.Status.Should().Be(OrderStatus.Failure);
        russianConvoy.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "Edi", false),
                (Nation.England, UnitType.Fleet, "Yor", false),
                (Nation.France, UnitType.Army, "Bre", false),
                (Nation.France, UnitType.Fleet, "ENG", false),
                (Nation.Germany, UnitType.Fleet, "Bel", false),
                (Nation.Germany, UnitType.Fleet, "Lon", false),
                (Nation.Italy, UnitType.Fleet, "MAO", false),
                (Nation.Italy, UnitType.Fleet, "IRI", false),
                (Nation.Russia, UnitType.Army, "Nwy", false),
                (Nation.Russia, UnitType.Fleet, "NTH", false),
            ]);
    }

    [Fact(DisplayName = "F.24. Second order paradox with no resolution")]
    public void DATC_F_24()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "Edi"),
                (Nation.England, UnitType.Fleet, "Lon"),
                (Nation.England, UnitType.Fleet, "IRI"),
                (Nation.England, UnitType.Fleet, "MAO"),
                (Nation.France, UnitType.Army, "Bre"),
                (Nation.France, UnitType.Fleet, "ENG"),
                (Nation.France, UnitType.Fleet, "Bel"),
                (Nation.Russia, UnitType.Army, "Nwy"),
                (Nation.Russia, UnitType.Fleet, "NTH"),
            ]);

        var englishMove1 = units.Get("Edi").Move("NTH");
        var englishMove2 = units.Get("IRI").Move("ENG");
        var englishSupport1 = units.Get("Lon").Support(units.Get("Edi"), "NTH");
        var englishSupport2 = units.Get("MAO").Support(units.Get("IRI"), "ENG");
        var frenchMove = units.Get("Bre").Move("Lon");
        var frenchSupport = units.Get("Bel").Support(units.Get("ENG"));
        var frenchConvoy = units.Get("ENG").Convoy(units.Get("Bre"), "Lon");
        var russianMove = units.Get("Nwy").Move("Bel");
        var russianConvoy = units.Get("NTH").Convoy(units.Get("Nwy"), "Bel");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove1.Status.Should().Be(OrderStatus.Success);
        englishMove2.Status.Should().Be(OrderStatus.Failure);
        englishSupport1.Status.Should().Be(OrderStatus.Success);
        englishSupport2.Status.Should().Be(OrderStatus.Failure);
        frenchMove.Status.Should().Be(OrderStatus.Failure);
        frenchSupport.Status.Should().Be(OrderStatus.Success);
        frenchConvoy.Status.Should().Be(OrderStatus.Failure);
        russianMove.Status.Should().Be(OrderStatus.Failure);
        russianConvoy.Status.Should().Be(OrderStatus.Failure);

        board.ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "Edi", false),
                (Nation.England, UnitType.Fleet, "Lon", false),
                (Nation.England, UnitType.Fleet, "IRI", false),
                (Nation.England, UnitType.Fleet, "MAO", false),
                (Nation.France, UnitType.Army, "Bre", false),
                (Nation.France, UnitType.Fleet, "ENG", false),
                (Nation.France, UnitType.Fleet, "Bel", false),
                (Nation.Russia, UnitType.Army, "Nwy", false),
                (Nation.Russia, UnitType.Fleet, "NTH", true),
            ]);
        board.ShouldNotHaveNextBoard();
    }

    [Fact(DisplayName = "F.25. Cut support last")]
    public void DATC_F_25()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Germany, UnitType.Army, "Ruh"),
                (Nation.Germany, UnitType.Army, "Hol"),
                (Nation.Germany, UnitType.Army, "Den"),
                (Nation.Germany, UnitType.Fleet, "SKA"),
                (Nation.Germany, UnitType.Army, "Fin"),
                (Nation.England, UnitType.Army, "Yor"),
                (Nation.England, UnitType.Fleet, "NTH"),
                (Nation.England, UnitType.Fleet, "HEL"),
                (Nation.England, UnitType.Army, "Bel"),
                (Nation.Russia, UnitType.Fleet, "NWG"),
                (Nation.Russia, UnitType.Fleet, "Nwy"),
                (Nation.Russia, UnitType.Fleet, "Swe"),
            ]);

        var germanMove1 = units.Get("Ruh").Move("Bel");
        var germanMove2 = units.Get("Den").Move("Nwy");
        var germanSupport1 = units.Get("Hol").Support(units.Get("Ruh"), "Bel");
        var germanSupport2 = units.Get("Fin").Support(units.Get("Den"), "Nwy");
        var germanConvoy = units.Get("SKA").Convoy(units.Get("Den"), "Nwy");
        var englishHold = units.Get("Bel").Hold();
        var englishMove = units.Get("Yor").Move("Hol");
        var englishSupport = units.Get("HEL").Support(units.Get("Yor"), "Hol");
        var englishConvoy = units.Get("NTH").Convoy(units.Get("Yor"), "Hol");
        var russianMove1 = units.Get("NWG").Move("NTH");
        var russianMove2 = units.Get("Swe").Move("SKA");
        var russianSupport = units.Get("Nwy").Support(units.Get("NWG"), "NTH");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        germanMove1.Status.Should().Be(OrderStatus.Failure);
        germanMove2.Status.Should().Be(OrderStatus.Success);
        germanSupport1.Status.Should().Be(OrderStatus.Failure);
        germanSupport2.Status.Should().Be(OrderStatus.Success);
        germanConvoy.Status.Should().Be(OrderStatus.Success);
        englishHold.Status.Should().Be(OrderStatus.Success);
        englishMove.Status.Should().Be(OrderStatus.Success);
        englishSupport.Status.Should().Be(OrderStatus.Success);
        englishConvoy.Status.Should().Be(OrderStatus.Success);
        russianMove1.Status.Should().Be(OrderStatus.Failure);
        russianMove2.Status.Should().Be(OrderStatus.Failure);
        russianSupport.Status.Should().Be(OrderStatus.Failure);

        board.ShouldHaveUnits(
            [
                (Nation.Germany, UnitType.Army, "Ruh", false),
                (Nation.Germany, UnitType.Army, "Hol", true),
                (Nation.Germany, UnitType.Army, "Den", false),
                (Nation.Germany, UnitType.Fleet, "SKA", false),
                (Nation.Germany, UnitType.Army, "Fin", false),
                (Nation.England, UnitType.Army, "Yor", false),
                (Nation.England, UnitType.Fleet, "NTH", false),
                (Nation.England, UnitType.Fleet, "HEL", false),
                (Nation.England, UnitType.Army, "Bel", false),
                (Nation.Russia, UnitType.Fleet, "NWG", false),
                (Nation.Russia, UnitType.Fleet, "Nwy", true),
                (Nation.Russia, UnitType.Fleet, "Swe", false),
            ]);
        board.ShouldNotHaveNextBoard();
    }
}
