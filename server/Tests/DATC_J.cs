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
public class DATC_J : AdjudicationTestBase
{
    [Fact(DisplayName = "J.01. Too many disband orders")]
    public void DATC_J_1()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard(phase: Phase.Winter);

        board.AddCentres([(Nation.France, "Par")]);
        var units = board.AddUnits(
            [
                (Nation.France, UnitType.Army, "Pic"),
                (Nation.France, UnitType.Army, "Par"),
            ]);

        // Trying to disband a non-existent unit will crash the program in all sorts of ugly ways, so that has been
        // omitted from this test. Consider it impossible.

        var disband1 = units.Get("Pic", phase: Phase.Winter).Disband();
        var disband2 = units.Get("Par", phase: Phase.Winter).Disband();

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        var disband1Status = disband1.Status;
        disband1Status.Should().BeOneOf(OrderStatus.Success, OrderStatus.Failure);

        if (disband1Status == OrderStatus.Success)
        {
            disband2.Status.Should().Be(OrderStatus.Failure);

            board.Next().ShouldHaveUnits([(Nation.France, UnitType.Army, "Par", false)]);
        }
        else
        {
            disband2.Status.Should().Be(OrderStatus.Success);

            board.Next().ShouldHaveUnits([(Nation.France, UnitType.Army, "Pic", false)]);
        }
    }

    [Fact(DisplayName = "J.02. Removing the same unit twice")]
    public void DATC_J_2()
    {
        // Arrange
        var world = new World();
        var board = world.AddBoard(phase: Phase.Winter);

        var units = board.AddUnits(
            [
                (Nation.France, UnitType.Army, "Par"),
                (Nation.France, UnitType.Army, "Pic"),
            ]);

        var disband1 = units.Get("Par", phase: Phase.Winter).Disband();
        var disband2 = units.Get("Par", phase: Phase.Winter).Disband();

        // Act
        new Adjudicator(world, false, MapFactory, DefaultWorldFactory).Adjudicate();

        // Assert
        disband1.Status.Should().Be(OrderStatus.Success);
        disband2.Status.Should().Be(OrderStatus.Invalid);

        board.Next().ShouldHaveUnits([]);
    }

    [Fact(DisplayName = "J.03. Civil disorder two armies with different distance", Skip = "Decided against")]
    public void DATC_J_3()
    {
        // Due to the effort involved in implementing a distance-based auto-disband system, we have elected to choose
        // unspecified disbands randomly. Thus, this test and all others relating to civil disorder are not valid for
        // 5D Diplomacy. In practice, the server can't handle civil disorder as it will continue waiting until all
        // players have submitted.
    }

    [Fact(DisplayName = "J.04. Civil disorder two armies with equal distance", Skip = "Decided against")]
    public void DATC_J_4()
    {
        // See note in DATC J 3.
    }

    [Fact(DisplayName = "J.05. Civil disorder two fleets with different distance", Skip = "Decided against")]
    public void DATC_J_5()
    {
        // See note in DATC J 3.
    }

    [Fact(DisplayName = "J.06. Civil disorder two fleets with equal distance", Skip = "Decided against")]
    public void DATC_J_6()
    {
        // See note in DATC J 3.
    }

    [Fact(DisplayName = "J.07. Civil disorder two fleets and army with equal distance", Skip = "Decided against")]
    public void DATC_J_7()
    {
        // See note in DATC J 3.
    }

    [Fact(DisplayName = "J.08. Civil disorder a fleet with shorter distance than the army", Skip = "Decided against")]
    public void DATC_J_8()
    {
        // See note in DATC J 3.
    }

    [Fact(DisplayName = "J.09. Civil disorder must be counted from both coasts", Skip = "Decided against")]
    public void DATC_J_9()
    {
        // See note in DATC J 3.
    }

    [Fact(DisplayName = "J.10. Civil disorder counting convoying distance", Skip = "Decided against")]
    public void DATC_J_10()
    {
        // See note in DATC J 3.
    }

    [Fact(DisplayName = "J.11. Distance to owned supply centre", Skip = "Decided against")]
    public void DATC_J_11()
    {
        // See note in DATC J 3.
    }
}
