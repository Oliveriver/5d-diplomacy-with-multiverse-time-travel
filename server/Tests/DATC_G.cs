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
public class DATC_G : AdjudicationTestBase
{
    [Fact(DisplayName = "G.01. Two units can swap provinces by convoy")]
    public void DATC_G_1()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Army, "Nwy"),
                (Nation.England, UnitType.Fleet, "SKA"),
                (Nation.Russia, UnitType.Army, "Swe"),
            ]);

        var englishMove = units.Get("Nwy").Move("Swe");
        var englishConvoy = units.Get("SKA").Convoy(units.Get("Nwy"), "Swe");
        var russianMove = units.Get("Swe").Move("Nwy");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        englishConvoy.Status.Should().Be(OrderStatus.Success);
        russianMove.Status.Should().Be(OrderStatus.Success);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Army, "Swe", false),
                (Nation.England, UnitType.Fleet, "SKA", false),
                (Nation.Russia, UnitType.Army, "Nwy", false),
            ]);
    }

    [Fact(DisplayName = "G.02. Kidnapping an army")]
    public void DATC_G_2()
    {
        // Decision is to allow non-dislodged convoy kidnapping because it's fun, maybe even more so in 5D.

        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Army, "Nwy"),
                (Nation.Russia, UnitType.Fleet, "Swe"),
                (Nation.Germany, UnitType.Fleet, "SKA"),
            ]);

        var englishMove = units.Get("Nwy").Move("Swe");
        var russianMove = units.Get("Swe").Move("Nwy");
        var germanConvoy = units.Get("SKA").Convoy(units.Get("Nwy"), "Swe");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        russianMove.Status.Should().Be(OrderStatus.Success);
        germanConvoy.Status.Should().Be(OrderStatus.Success);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Army, "Swe", false),
                (Nation.Russia, UnitType.Fleet, "Nwy", false),
                (Nation.Germany, UnitType.Fleet, "SKA", false),
            ]);
    }

    [Fact(DisplayName = "G.03. An unwanted convoy to adjacent province")]
    public void DATC_G_3()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.France, UnitType.Fleet, "Bre"),
                (Nation.France, UnitType.Army, "Pic"),
                (Nation.France, UnitType.Army, "Bur"),
                (Nation.France, UnitType.Fleet, "MAO"),
                (Nation.England, UnitType.Fleet, "ENG"),
            ]);

        var frenchMove1 = units.Get("Bre").Move("ENG");
        var frenchMove2 = units.Get("Pic").Move("Bel");
        var frenchSupport1 = units.Get("MAO").Support(units.Get("Bre"), "ENG");
        var frenchSupport2 = units.Get("Bur").Support(units.Get("Pic"), "Bel");
        var englishConvoy = units.Get("ENG").Convoy(units.Get("Pic"), "Bel");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        frenchMove1.Status.Should().Be(OrderStatus.Success);
        frenchMove2.Status.Should().Be(OrderStatus.Success);
        frenchSupport1.Status.Should().Be(OrderStatus.Success);
        frenchSupport2.Status.Should().Be(OrderStatus.Success);
        englishConvoy.Status.Should().Be(OrderStatus.Failure);

        board.ShouldHaveUnits(
            [
                (Nation.France, UnitType.Fleet, "Bre", false),
                (Nation.France, UnitType.Army, "Pic", false),
                (Nation.France, UnitType.Army, "Bur", false),
                (Nation.France, UnitType.Fleet, "MAO", false),
                (Nation.England, UnitType.Fleet, "ENG", true),
            ]);
        board.ShouldNotHaveNextBoard();
    }

    [Fact(DisplayName = "G.04. An unwanted disrupted convoy to adjacent province and opposite move")]
    public void DATC_G_4()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.France, UnitType.Fleet, "Bre"),
                (Nation.France, UnitType.Army, "Pic"),
                (Nation.France, UnitType.Army, "Bur"),
                (Nation.France, UnitType.Fleet, "MAO"),
                (Nation.England, UnitType.Fleet, "ENG"),
                (Nation.England, UnitType.Army, "Bel"),
            ]);

        var frenchMove1 = units.Get("Bre").Move("ENG");
        var frenchMove2 = units.Get("Pic").Move("Bel");
        var frenchSupport1 = units.Get("MAO").Support(units.Get("Bre"), "ENG");
        var frenchSupport2 = units.Get("Bur").Support(units.Get("Pic"), "Bel");
        var englishMove = units.Get("Bel").Move("Pic");
        var englishConvoy = units.Get("ENG").Convoy(units.Get("Pic"), "Bel");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        frenchMove1.Status.Should().Be(OrderStatus.Success);
        frenchMove2.Status.Should().Be(OrderStatus.Success);
        frenchSupport1.Status.Should().Be(OrderStatus.Success);
        frenchSupport2.Status.Should().Be(OrderStatus.Success);
        englishMove.Status.Should().Be(OrderStatus.Failure);
        englishConvoy.Status.Should().Be(OrderStatus.Failure);

        board.ShouldHaveUnits(
            [
                (Nation.France, UnitType.Fleet, "Bre", false),
                (Nation.France, UnitType.Army, "Pic", false),
                (Nation.France, UnitType.Army, "Bur", false),
                (Nation.France, UnitType.Fleet, "MAO", false),
                (Nation.England, UnitType.Fleet, "ENG", true),
                (Nation.England, UnitType.Army, "Bel", true),
            ]);
        board.ShouldNotHaveNextBoard();
    }

    [Fact(DisplayName = "G.05. Swapping with multiple fleets with one own fleet")]
    public void DATC_G_5()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Italy, UnitType.Army, "Rom"),
                (Nation.Italy, UnitType.Fleet, "TYS"),
                (Nation.Turkey, UnitType.Army, "Apu"),
                (Nation.Turkey, UnitType.Fleet, "ION"),
            ]);

        var italianMove = units.Get("Rom").Move("Apu");
        var italianConvoy = units.Get("TYS").Convoy(units.Get("Rom"), "Apu");
        var turkishMove = units.Get("Apu").Move("Rom");
        var turkishConvoy = units.Get("ION").Convoy(units.Get("Rom"), "Apu");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        italianMove.Status.Should().Be(OrderStatus.Success);
        italianConvoy.Status.Should().Be(OrderStatus.Success);
        turkishMove.Status.Should().Be(OrderStatus.Success);
        turkishConvoy.Status.Should().Be(OrderStatus.Success);

        board.Next().ShouldHaveUnits(
            [
                (Nation.Italy, UnitType.Army, "Apu", false),
                (Nation.Italy, UnitType.Fleet, "TYS", false),
                (Nation.Turkey, UnitType.Army, "Rom", false),
                (Nation.Turkey, UnitType.Fleet, "ION", false),
            ]);
    }

    [Fact(DisplayName = "G.06. Swapping with unintended intent")]
    public void DATC_G_6()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Army, "Lvp"),
                (Nation.England, UnitType.Fleet, "ENG"),
                (Nation.Germany, UnitType.Army, "Edi"),
                (Nation.France, UnitType.Fleet, "IRI"),
                (Nation.France, UnitType.Fleet, "NTH"),
                (Nation.Russia, UnitType.Fleet, "NWG"),
                (Nation.Russia, UnitType.Fleet, "NAO"),
            ]);

        var englishMove = units.Get("Lvp").Move("Edi");
        var englishConvoy = units.Get("ENG").Convoy(units.Get("Lvp"), "Edi");
        var germanMove = units.Get("Edi").Move("Lvp");
        var frenchHold1 = units.Get("IRI").Hold();
        var frenchHold2 = units.Get("NTH").Hold();
        var russianConvoy1 = units.Get("NWG").Convoy(units.Get("Lvp"), "Edi");
        var russianConvoy2 = units.Get("NAO").Convoy(units.Get("Lvp"), "Edi");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        englishConvoy.Status.Should().Be(OrderStatus.Invalid);
        germanMove.Status.Should().Be(OrderStatus.Success);
        frenchHold1.Status.Should().Be(OrderStatus.Success);
        frenchHold2.Status.Should().Be(OrderStatus.Success);
        russianConvoy1.Status.Should().Be(OrderStatus.Success);
        russianConvoy2.Status.Should().Be(OrderStatus.Success);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Army, "Edi", false),
                (Nation.England, UnitType.Fleet, "ENG", false),
                (Nation.Germany, UnitType.Army, "Lvp", false),
                (Nation.France, UnitType.Fleet, "IRI", false),
                (Nation.France, UnitType.Fleet, "NTH", false),
                (Nation.Russia, UnitType.Fleet, "NWG", false),
                (Nation.Russia, UnitType.Fleet, "NAO", false),
            ]);
    }

    [Fact(DisplayName = "G.07. Swapping with illegal intent")]
    public void DATC_G_7()
    {
        // Another overrule of the DATC to allow non-dislodged convoy kidnapping.

        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "SKA"),
                (Nation.England, UnitType.Fleet, "Nwy"),
                (Nation.Russia, UnitType.Army, "Swe"),
                (Nation.Russia, UnitType.Fleet, "BOT"),
            ]);

        var englishMove = units.Get("Nwy").Move("Swe");
        var englishConvoy = units.Get("SKA").Convoy(units.Get("Swe"), "Nwy");
        var russianMove = units.Get("Swe").Move("Nwy");
        var russianConvoy = units.Get("BOT").Convoy(units.Get("Swe"), "Nwy");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        englishConvoy.Status.Should().Be(OrderStatus.Success);
        russianMove.Status.Should().Be(OrderStatus.Success);
        russianConvoy.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "SKA", false),
                (Nation.England, UnitType.Fleet, "Swe", false),
                (Nation.Russia, UnitType.Army, "Nwy", false),
                (Nation.Russia, UnitType.Fleet, "BOT", false),
            ]);
    }

    [Fact(DisplayName = "G.08. Explicit convoy that isn't there", Skip = "Not applicable")]
    public void DATC_G_8()
    {
        // 5D Diplomacy does not have a concept of an explicit "via convoy" move, so this case does not apply.
    }

    [Fact(DisplayName = "G.09. Swapped or dislodged?")]
    public void DATC_G_9()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Army, "Nwy"),
                (Nation.England, UnitType.Fleet, "SKA"),
                (Nation.England, UnitType.Fleet, "Fin"),
                (Nation.Russia, UnitType.Army, "Swe"),
            ]);

        var englishMove = units.Get("Nwy").Move("Swe");
        var englishSupport = units.Get("Fin").Support(units.Get("Nwy"), "Swe");
        var englishConvoy = units.Get("SKA").Convoy(units.Get("Nwy"), "Swe");
        var russianMove = units.Get("Swe").Move("Nwy");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        englishSupport.Status.Should().Be(OrderStatus.Success);
        englishConvoy.Status.Should().Be(OrderStatus.Success);
        russianMove.Status.Should().Be(OrderStatus.Success);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Army, "Swe", false),
                (Nation.England, UnitType.Fleet, "SKA", false),
                (Nation.England, UnitType.Fleet, "Fin", false),
                (Nation.Russia, UnitType.Army, "Nwy", false),
            ]);
    }

    [Fact(DisplayName = "G.10. Swapped or a head-to-head battle?")]
    public void DATC_G_10()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Army, "Nwy"),
                (Nation.England, UnitType.Fleet, "Den"),
                (Nation.England, UnitType.Fleet, "Fin"),
                (Nation.Germany, UnitType.Fleet, "SKA"),
                (Nation.Russia, UnitType.Army, "Swe"),
                (Nation.Russia, UnitType.Fleet, "BAR"),
                (Nation.France, UnitType.Fleet, "NWG"),
                (Nation.France, UnitType.Fleet, "NTH"),
            ]);

        var englishMove = units.Get("Nwy").Move("Swe");
        var englishSupport1 = units.Get("Fin").Support(units.Get("Nwy"), "Swe");
        var englishSupport2 = units.Get("Den").Support(units.Get("Nwy"), "Swe");
        var germanConvoy = units.Get("SKA").Convoy(units.Get("Nwy"), "Swe");
        var russianMove = units.Get("Swe").Move("Nwy");
        var russianSupport = units.Get("BAR").Support(units.Get("Swe"), "Nwy");
        var frenchMove = units.Get("NWG").Move("Nwy");
        var frenchSupport = units.Get("NTH").Support(units.Get("NWG"), "Nwy");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        englishSupport1.Status.Should().Be(OrderStatus.Success);
        englishSupport2.Status.Should().Be(OrderStatus.Success);
        germanConvoy.Status.Should().Be(OrderStatus.Success);
        russianMove.Status.Should().Be(OrderStatus.Failure);
        russianSupport.Status.Should().Be(OrderStatus.Failure);
        frenchMove.Status.Should().Be(OrderStatus.Failure);
        frenchSupport.Status.Should().Be(OrderStatus.Failure);

        board.ShouldHaveUnits(
            [
                (Nation.England, UnitType.Army, "Nwy", false),
                (Nation.England, UnitType.Fleet, "Den", false),
                (Nation.England, UnitType.Fleet, "Fin", false),
                (Nation.Germany, UnitType.Fleet, "SKA", false),
                (Nation.Russia, UnitType.Army, "Swe", true),
                (Nation.Russia, UnitType.Fleet, "BAR", false),
                (Nation.France, UnitType.Fleet, "NWG", false),
                (Nation.France, UnitType.Fleet, "NTH", false),
            ]);
        board.ShouldNotHaveNextBoard();
    }

    [Fact(DisplayName = "G.11. A convoy to an adjacent province with a paradox")]
    public void DATC_G_11()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "Nwy"),
                (Nation.England, UnitType.Fleet, "NTH"),
                (Nation.Russia, UnitType.Army, "Swe"),
                (Nation.Russia, UnitType.Fleet, "SKA"),
                (Nation.Russia, UnitType.Fleet, "BAR"),
            ]);

        var englishMove = units.Get("NTH").Move("SKA");
        var englishSupport = units.Get("Nwy").Support(units.Get("NTH"), "SKA");
        var russianMove = units.Get("Swe").Move("Nwy");
        var russianSupport = units.Get("BAR").Support(units.Get("Swe"), "Nwy");
        var russianConvoy = units.Get("SKA").Convoy(units.Get("Swe"), "Nwy");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        englishSupport.Status.Should().Be(OrderStatus.Success);
        russianMove.Status.Should().Be(OrderStatus.Failure);
        russianSupport.Status.Should().Be(OrderStatus.Failure);
        russianConvoy.Status.Should().Be(OrderStatus.Failure);

        board.ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "Nwy", false),
                (Nation.England, UnitType.Fleet, "NTH", false),
                (Nation.Russia, UnitType.Army, "Swe", false),
                (Nation.Russia, UnitType.Fleet, "SKA", true),
                (Nation.Russia, UnitType.Fleet, "BAR", false),
            ]);
        board.ShouldNotHaveNextBoard();
    }

    [Fact(DisplayName = "G.12. Swapping two units with two convoys")]
    public void DATC_G_12()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Army, "Lvp"),
                (Nation.England, UnitType.Fleet, "NAO"),
                (Nation.England, UnitType.Fleet, "NWG"),
                (Nation.Germany, UnitType.Army, "Edi"),
                (Nation.Germany, UnitType.Fleet, "NTH"),
                (Nation.Germany, UnitType.Fleet, "ENG"),
                (Nation.Germany, UnitType.Fleet, "IRI"),
            ]);

        var englishMove = units.Get("Lvp").Move("Edi");
        var englishConvoy1 = units.Get("NAO").Convoy(units.Get("Lvp"), "Edi");
        var englishConvoy2 = units.Get("NWG").Convoy(units.Get("Lvp"), "Edi");
        var germanMove = units.Get("Edi").Move("Lvp");
        var germanConvoy1 = units.Get("NTH").Convoy(units.Get("Edi"), "Lvp");
        var germanConvoy2 = units.Get("ENG").Convoy(units.Get("Edi"), "Lvp");
        var germanConvoy3 = units.Get("IRI").Convoy(units.Get("Edi"), "Lvp");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        englishConvoy1.Status.Should().Be(OrderStatus.Success);
        englishConvoy2.Status.Should().Be(OrderStatus.Success);
        germanMove.Status.Should().Be(OrderStatus.Success);
        germanConvoy1.Status.Should().Be(OrderStatus.Success);
        germanConvoy2.Status.Should().Be(OrderStatus.Success);
        germanConvoy3.Status.Should().Be(OrderStatus.Success);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Army, "Edi", false),
                (Nation.England, UnitType.Fleet, "NAO", false),
                (Nation.England, UnitType.Fleet, "NWG", false),
                (Nation.Germany, UnitType.Army, "Lvp", false),
                (Nation.Germany, UnitType.Fleet, "NTH", false),
                (Nation.Germany, UnitType.Fleet, "ENG", false),
                (Nation.Germany, UnitType.Fleet, "IRI", false),
            ]);
    }

    [Fact(DisplayName = "G.13. Support cut on attack on itself via convoy")]
    public void DATC_G_13()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.Austria, UnitType.Fleet, "ADR"),
                (Nation.Austria, UnitType.Army, "Tri"),
                (Nation.Italy, UnitType.Army, "Ven"),
                (Nation.Italy, UnitType.Fleet, "Alb"),
            ]);

        var austrianMove = units.Get("Tri").Move("Ven");
        var austrianConvoy = units.Get("ADR").Convoy(units.Get("Tri"), "Ven");
        var italianMove = units.Get("Alb").Move("Tri");
        var italianSupport = units.Get("Ven").Support(units.Get("Alb"), "Tri");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        austrianMove.Status.Should().Be(OrderStatus.Failure);
        austrianConvoy.Status.Should().Be(OrderStatus.Failure);
        italianMove.Status.Should().Be(OrderStatus.Success);
        italianSupport.Status.Should().Be(OrderStatus.Success);

        board.ShouldHaveUnits(
            [
                (Nation.Austria, UnitType.Fleet, "ADR", false),
                (Nation.Austria, UnitType.Army, "Tri", true),
                (Nation.Italy, UnitType.Army, "Ven", false),
                (Nation.Italy, UnitType.Fleet, "Alb", false),
            ]);
        board.ShouldNotHaveNextBoard();
    }

    [Fact(DisplayName = "G.14. Bounce by convoy to adjacent province")]
    public void DATC_G_14()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Army, "Nwy"),
                (Nation.England, UnitType.Fleet, "Den"),
                (Nation.England, UnitType.Fleet, "Fin"),
                (Nation.France, UnitType.Fleet, "NWG"),
                (Nation.France, UnitType.Fleet, "NTH"),
                (Nation.Germany, UnitType.Fleet, "SKA"),
                (Nation.Russia, UnitType.Army, "Swe"),
                (Nation.Russia, UnitType.Fleet, "BAR"),
            ]);

        var englishMove = units.Get("Nwy").Move("Swe");
        var englishSupport1 = units.Get("Den").Support(units.Get("Nwy"), "Swe");
        var englishSupport2 = units.Get("Fin").Support(units.Get("Nwy"), "Swe");
        var frenchMove = units.Get("NWG").Move("Nwy");
        var frenchSupport = units.Get("NTH").Support(units.Get("NWG"), "Nwy");
        var germanConvoy = units.Get("SKA").Convoy(units.Get("Swe"), "Nwy");
        var russianMove = units.Get("Swe").Move("Nwy");
        var russianSupport = units.Get("BAR").Support(units.Get("Swe"), "Nwy");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove.Status.Should().Be(OrderStatus.Success);
        englishSupport1.Status.Should().Be(OrderStatus.Success);
        englishSupport2.Status.Should().Be(OrderStatus.Success);
        frenchMove.Status.Should().Be(OrderStatus.Failure);
        frenchSupport.Status.Should().Be(OrderStatus.Failure);
        germanConvoy.Status.Should().Be(OrderStatus.Failure);
        russianMove.Status.Should().Be(OrderStatus.Failure);
        russianSupport.Status.Should().Be(OrderStatus.Failure);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Army, "Swe", false),
                (Nation.England, UnitType.Fleet, "Den", false),
                (Nation.England, UnitType.Fleet, "Fin", false),
                (Nation.France, UnitType.Fleet, "NWG", false),
                (Nation.France, UnitType.Fleet, "NTH", false),
                (Nation.Germany, UnitType.Fleet, "SKA", false),
                (Nation.Russia, UnitType.Fleet, "BAR", false),
            ]);
    }

    [Fact(DisplayName = "G.15. Bounce and dislodge with double convoy")]
    public void DATC_G_15()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH"),
                (Nation.England, UnitType.Army, "Hol"),
                (Nation.England, UnitType.Army, "Yor"),
                (Nation.England, UnitType.Army, "Lon"),
                (Nation.France, UnitType.Fleet, "ENG"),
                (Nation.France, UnitType.Army, "Bel"),
            ]);

        var englishMove1 = units.Get("Yor").Move("Lon");
        var englishMove2 = units.Get("Lon").Move("Bel");
        var englishSupport = units.Get("Hol").Support(units.Get("Lon"), "Bel");
        var englishConvoy = units.Get("NTH").Convoy(units.Get("Lon"), "Bel");
        var frenchMove = units.Get("Bel").Move("Lon");
        var frenchSupport = units.Get("ENG").Convoy(units.Get("Bel"), "Lon");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove1.Status.Should().Be(OrderStatus.Failure);
        englishMove2.Status.Should().Be(OrderStatus.Success);
        englishSupport.Status.Should().Be(OrderStatus.Success);
        englishConvoy.Status.Should().Be(OrderStatus.Success);
        frenchMove.Status.Should().Be(OrderStatus.Failure);
        frenchSupport.Status.Should().Be(OrderStatus.Failure);

        board.ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH", false),
                (Nation.England, UnitType.Army, "Hol", false),
                (Nation.England, UnitType.Army, "Yor", false),
                (Nation.England, UnitType.Army, "Lon", false),
                (Nation.France, UnitType.Fleet, "ENG", false),
                (Nation.France, UnitType.Army, "Bel", true),
            ]);
        board.ShouldNotHaveNextBoard();
    }

    [Fact(DisplayName = "G.16. The two unit in one area bug, moving by convoy")]
    public void DATC_G_16()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Army, "Nwy"),
                (Nation.England, UnitType.Army, "Den"),
                (Nation.England, UnitType.Fleet, "BAL"),
                (Nation.England, UnitType.Fleet, "NTH"),
                (Nation.Russia, UnitType.Army, "Swe"),
                (Nation.Russia, UnitType.Fleet, "SKA"),
                (Nation.Russia, UnitType.Fleet, "NWG"),
            ]);

        var englishMove1 = units.Get("Nwy").Move("Swe");
        var englishMove2 = units.Get("NTH").Move("Nwy");
        var englishSupport1 = units.Get("Den").Support(units.Get("Nwy"), "Swe");
        var englishSupport2 = units.Get("BAL").Support(units.Get("Nwy"), "Swe");
        var russianMove = units.Get("Swe").Move("Nwy");
        var russianSupport = units.Get("NWG").Support(units.Get("Swe"), "Nwy");
        var russianConvoy = units.Get("SKA").Convoy(units.Get("Swe"), "Nwy");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove1.Status.Should().Be(OrderStatus.Success);
        englishMove2.Status.Should().Be(OrderStatus.Failure);
        englishSupport1.Status.Should().Be(OrderStatus.Success);
        englishSupport2.Status.Should().Be(OrderStatus.Success);
        russianMove.Status.Should().Be(OrderStatus.Success);
        russianSupport.Status.Should().Be(OrderStatus.Success);
        russianConvoy.Status.Should().Be(OrderStatus.Success);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Army, "Swe", false),
                (Nation.England, UnitType.Army, "Den", false),
                (Nation.England, UnitType.Fleet, "BAL", false),
                (Nation.England, UnitType.Fleet, "NTH", false),
                (Nation.Russia, UnitType.Army, "Nwy", false),
                (Nation.Russia, UnitType.Fleet, "SKA", false),
                (Nation.Russia, UnitType.Fleet, "NWG", false),
            ]);
    }

    [Fact(DisplayName = "G.17. The two unit in one area bug, moving over land")]
    public void DATC_G_17()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Army, "Nwy"),
                (Nation.England, UnitType.Army, "Den"),
                (Nation.England, UnitType.Fleet, "BAL"),
                (Nation.England, UnitType.Fleet, "SKA"),
                (Nation.England, UnitType.Fleet, "NTH"),
                (Nation.Russia, UnitType.Army, "Swe"),
                (Nation.Russia, UnitType.Fleet, "NWG"),
            ]);

        var englishMove1 = units.Get("Nwy").Move("Swe");
        var englishMove2 = units.Get("NTH").Move("Nwy");
        var englishSupport1 = units.Get("Den").Support(units.Get("Nwy"), "Swe");
        var englishSupport2 = units.Get("BAL").Support(units.Get("Nwy"), "Swe");
        var englishConvoy = units.Get("SKA").Convoy(units.Get("Nwy"), "Swe");
        var russianMove = units.Get("Swe").Move("Nwy");
        var russianSupport = units.Get("NWG").Support(units.Get("Swe"), "Nwy");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove1.Status.Should().Be(OrderStatus.Success);
        englishMove2.Status.Should().Be(OrderStatus.Failure);
        englishSupport1.Status.Should().Be(OrderStatus.Success);
        englishSupport2.Status.Should().Be(OrderStatus.Success);
        englishConvoy.Status.Should().Be(OrderStatus.Success);
        russianMove.Status.Should().Be(OrderStatus.Success);
        russianSupport.Status.Should().Be(OrderStatus.Success);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Army, "Swe", false),
                (Nation.England, UnitType.Army, "Den", false),
                (Nation.England, UnitType.Fleet, "BAL", false),
                (Nation.England, UnitType.Fleet, "SKA", false),
                (Nation.England, UnitType.Fleet, "NTH", false),
                (Nation.Russia, UnitType.Army, "Nwy", false),
                (Nation.Russia, UnitType.Fleet, "NWG", false),
            ]);
    }

    [Fact(DisplayName = "G.18. The two unit in one area bug, with double convoy")]
    public void DATC_G_18()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH"),
                (Nation.England, UnitType.Army, "Hol"),
                (Nation.England, UnitType.Army, "Yor"),
                (Nation.England, UnitType.Army, "Lon"),
                (Nation.England, UnitType.Army, "Ruh"),
                (Nation.France, UnitType.Fleet, "ENG"),
                (Nation.France, UnitType.Army, "Bel"),
                (Nation.France, UnitType.Army, "Wal"),
            ]);

        var englishMove1 = units.Get("Yor").Move("Lon");
        var englishMove2 = units.Get("Lon").Move("Bel");
        var englishSupport1 = units.Get("Hol").Support(units.Get("Lon"), "Bel");
        var englishSupport2 = units.Get("Ruh").Support(units.Get("Lon"), "Bel");
        var englishConvoy = units.Get("NTH").Convoy(units.Get("Lon"), "Bel");
        var frenchMove = units.Get("Bel").Move("Lon");
        var frenchSupport = units.Get("Wal").Support(units.Get("Bel"), "Lon");
        var frenchConvoy = units.Get("ENG").Convoy(units.Get("Bel"), "Lon");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        englishMove1.Status.Should().Be(OrderStatus.Failure);
        englishMove2.Status.Should().Be(OrderStatus.Success);
        englishSupport1.Status.Should().Be(OrderStatus.Success);
        englishSupport2.Status.Should().Be(OrderStatus.Success);
        englishConvoy.Status.Should().Be(OrderStatus.Success);
        frenchMove.Status.Should().Be(OrderStatus.Success);
        frenchSupport.Status.Should().Be(OrderStatus.Success);
        frenchConvoy.Status.Should().Be(OrderStatus.Success);

        board.Next().ShouldHaveUnits(
            [
                (Nation.England, UnitType.Fleet, "NTH", false),
                (Nation.England, UnitType.Army, "Hol", false),
                (Nation.England, UnitType.Army, "Yor", false),
                (Nation.England, UnitType.Army, "Bel", false),
                (Nation.England, UnitType.Army, "Ruh", false),
                (Nation.France, UnitType.Fleet, "ENG", false),
                (Nation.France, UnitType.Army, "Lon", false),
                (Nation.France, UnitType.Army, "Wal", false),
            ]);
    }

    [Fact(DisplayName = "G.19. Swapping with intent of unnecesary convoy")]
    public void DATC_G_19()
    {
        // Another case of overruling the DATC on convoy kidnapping.

        // Arrange
        var world = new World();
        var board = world.AddBoard();

        var units = board.AddUnits(
            [
                (Nation.France, UnitType.Army, "Mar"),
                (Nation.France, UnitType.Fleet, "WES"),
                (Nation.Italy, UnitType.Fleet, "LYO"),
                (Nation.Italy, UnitType.Army, "Spa"),
            ]);

        var frenchMove = units.Get("Mar").Move("Spa");
        var frenchConvoy = units.Get("WES").Convoy(units.Get("Mar"), "Spa");
        var italianMove = units.Get("Spa").Move("Mar");
        var italianConvoy = units.Get("LYO").Convoy(units.Get("Mar"), "Spa");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        frenchMove.Status.Should().Be(OrderStatus.Success);
        frenchConvoy.Status.Should().Be(OrderStatus.Invalid);
        italianMove.Status.Should().Be(OrderStatus.Success);
        italianConvoy.Status.Should().Be(OrderStatus.Success);

        board.Next().ShouldHaveUnits(
            [
                (Nation.France, UnitType.Army, "Spa", false),
                (Nation.France, UnitType.Fleet, "WES", false),
                (Nation.Italy, UnitType.Fleet, "LYO", false),
                (Nation.Italy, UnitType.Army, "Mar", false),
            ]);
    }

    [Fact(DisplayName = "G.20. Explicit convoy to adjacent province disrupted", Skip = "Not applicable")]
    public void DATC_G_20()
    {
        // 5D Diplomacy does not have a concept of specifying a move is "via convoy", so this test case is irrelevant.
    }
}
