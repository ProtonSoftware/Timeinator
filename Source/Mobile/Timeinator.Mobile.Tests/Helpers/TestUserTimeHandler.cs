using System;
using System.Collections.Generic;
using Timeinator.Core;
using Xunit;

namespace Timeinator.Mobile.Tests
{
    /// <summary>
    /// The tests for <see cref="TimeTaskManager"/> class
    /// </summary>
    public class TestUserTimeHandler
    {
        [Fact]
        public void UserTimeHandler_ShouldStartHandler()
        {
            // Arrange
            var handler = new UserTimeHandler();
            var tasksList = TestTaskListProvider.GetMockTimeTaskContexts(1);
            tasksList[0].AssignedTime = new TimeSpan(1, 0, 0);
            tasksList[1].AssignedTime = new TimeSpan(1, 0, 0);
            tasksList[2].AssignedTime = new TimeSpan(1, 30, 0);

            // Act
            handler.StartTimeHandler(tasksList);

            // Assert
            Assert.True(handler.TaskTimer.Enabled);
        }

        [Fact]
        public void UserTimeHandler_ShouldStopTask()
        {
            // Arrange
            var handler = new UserTimeHandler();
            var tasksList = TestTaskListProvider.GetMockTimeTaskContexts(1);
            tasksList[0].AssignedTime = new TimeSpan(1, 0, 0);
            tasksList[1].AssignedTime = new TimeSpan(1, 0, 0);
            tasksList[2].AssignedTime = new TimeSpan(1, 30, 0);

            // Act
            handler.StartTimeHandler(tasksList);
            handler.StopTask();

            // Assert
            Assert.False(handler.TaskTimer.Enabled);
        }

        [Fact]
        public void UserTimeHandler_ShouldResumeTask()
        {
            // Arrange
            var handler = new UserTimeHandler();
            var tasksList = TestTaskListProvider.GetMockTimeTaskContexts(1);
            tasksList[0].AssignedTime = new TimeSpan(1, 0, 0);
            tasksList[1].AssignedTime = new TimeSpan(1, 0, 0);
            tasksList[2].AssignedTime = new TimeSpan(1, 30, 0);

            // Act
            handler.StartTimeHandler(tasksList);
            handler.StopTask();
            handler.ResumeTask();

            // Assert
            Assert.True(handler.TaskTimer.Enabled);
        }
    }
}
