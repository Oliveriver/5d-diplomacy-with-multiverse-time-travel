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
public class DATC_I : AdjudicationTestBase
{
    [Fact(DisplayName = "I.01. Too many build orders")]
    public void DATC_I_1()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard(phase: Phase.Winter);

        board.AddUnits([(Nation.Germany, UnitType.Army, "Sil")]);
        board.AddCentres(
            [
                (Nation.Russia, "War"),
                (Nation.Germany, "Kie"),
                (Nation.Germany, "Mun"),
            ]);

        var build1 = board.Build(Nation.Germany, UnitType.Army, "War");
        var build2 = board.Build(Nation.Germany, UnitType.Army, "Kie");
        var build3 = board.Build(Nation.Germany, UnitType.Army, "Mun");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        build1.Status.Should().Be(OrderStatus.Invalid);
        var build2Status = build2.Status;
        build2Status.Should().BeOneOf(OrderStatus.Success, OrderStatus.Failure);

        if (build2Status == OrderStatus.Success)
        {
            build3.Status.Should().Be(OrderStatus.Failure);

            board.Next().ShouldHaveUnits(
                [
                    (Nation.Germany, UnitType.Army, "Sil", false),
                    (Nation.Germany, UnitType.Army, "Kie", false),
                ]);
        }
        else
        {
            build3.Status.Should().Be(OrderStatus.Success);

            board.Next().ShouldHaveUnits(
                [
                    (Nation.Germany, UnitType.Army, "Sil", false),
                    (Nation.Germany, UnitType.Army, "Mun", false),
                ]);
        }

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "I.02. Fleets cannot be built in land areas")]
    public void DATC_I_2()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard(phase: Phase.Winter);

        board.AddCentres([(Nation.Russia, "Mos")]);

        var build = board.Build(Nation.Russia, UnitType.Fleet, "Mos");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        build.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits([]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "I.03. Supply centre must be empty for building")]
    public void DATC_I_3()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard(phase: Phase.Winter);

        board.AddUnits([(Nation.Germany, UnitType.Army, "Ber")]);
        board.AddCentres(
            [
                (Nation.Germany, "Ber"),
                (Nation.Germany, "Mun"),
            ]);

        var build = board.Build(Nation.Germany, UnitType.Army, "Ber");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        build.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits([(Nation.Germany, UnitType.Army, "Ber", false)]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "I.04. Both coasts must be empty for building")]
    public void DATC_I_4()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard(phase: Phase.Winter);

        board.AddUnits([(Nation.Russia, UnitType.Fleet, "Stp_S")]);
        board.AddCentres(
            [
                (Nation.Russia, "Stp"),
                (Nation.Russia, "Mos"),
            ]);

        var build = board.Build(Nation.Russia, UnitType.Fleet, "Stp_N");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        build.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits([(Nation.Russia, UnitType.Fleet, "Stp_S", false)]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "I.05. Building in home supply centre that is not owned")]
    public void DATC_I_5()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard(phase: Phase.Winter);

        board.AddCentres(
            [
                (Nation.Russia, "Ber"),
                (Nation.Germany, "Mun"),
            ]);

        var build = board.Build(Nation.Germany, UnitType.Army, "Ber");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        build.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits([]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "I.06. Building in owned supply centre that is not a home supply centre")]
    public void DATC_I_6()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard(phase: Phase.Winter);

        board.AddCentres([(Nation.Germany, "War")]);

        var build = board.Build(Nation.Germany, UnitType.Army, "War");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        build.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits([]);

        world.ShouldHaveAllOrdersResolved();
    }

    [Fact(DisplayName = "I.07. Only one build in a home supply centre")]
    public void DATC_I_7()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard(phase: Phase.Winter);

        board.AddCentres(
            [
                (Nation.Russia, "Mos"),
                (Nation.Russia, "Sev"),
            ]);

        var build1 = board.Build(Nation.Russia, UnitType.Army, "Mos");
        var build2 = board.Build(Nation.Russia, UnitType.Army, "Mos");

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        build1.Status.Should().Be(OrderStatus.Success);
        build2.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits([(Nation.Russia, UnitType.Army, "Mos", false)]);

        world.ShouldHaveAllOrdersResolved();
    }
}
