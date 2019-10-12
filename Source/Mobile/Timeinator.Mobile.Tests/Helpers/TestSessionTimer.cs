using System;
using Timeinator.Mobile.Core;
using Xunit;

namespace Timeinator.Mobile.Tests
{
    /// <summary>
    /// Tests for <see cref="SessionHandler"/>
    /// </summary>
    /*public class TestSessionTimer
    {
        [Fact]
        public void SessionTimer_()
        {
            // Arrange
            var timer = new SessionHandler();
            var tasksList = TestTaskListProvider.GetMockTimeTaskContexts(1);
            tasksList[0].AssignedTime = new TimeSpan(1, 0, 0);
            tasksList[1].AssignedTime = new TimeSpan(1, 0, 0);
            tasksList[2].AssignedTime = new TimeSpan(1, 30, 0);

            // Act
            timer.SetupSession(null, null);

            // Assert
            Assert.True(timer.SessionDuration.Minutes > 0);
        }
    }*/
}
