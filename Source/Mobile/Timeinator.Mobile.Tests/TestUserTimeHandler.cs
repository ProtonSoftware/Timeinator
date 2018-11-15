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
        #region Setup

        /// <summary>
        /// Gets us current <see cref="UserTimeHandler"/> implementation as interface for testing
        /// </summary>
        private IUserTimeHandler GetCurrentHandler() => new UserTimeHandler();

        #endregion

        [Fact]
        public void UserTimeHandler_ShouldStartHandler()
        {
            // Arrange
            var handler = GetCurrentHandler();

            var tasksList = new List<TimeTaskContext>
            {
                new TimeTaskContext
                {
                    Name = "NAME1",
                    Priority = Priority.Two,
                    OrderId = 0,
                    AssignedTime = new TimeSpan(1, 0, 0)
                },
                new TimeTaskContext
                {
                    Name = "NAME2",
                    Priority = Priority.Two,
                    OrderId = 1,
                    AssignedTime = new TimeSpan(1, 0, 0)
                },
                new TimeTaskContext
                {
                    Name = "NAME3",
                    Priority = Priority.Three,
                    OrderId = 2,
                    AssignedTime = new TimeSpan(1, 30, 0)
                },
            };

            // Act
            handler.StartTimeHandler(tasksList);

            // Assert
            Assert.True(handler.TaskTimer.Enabled);
        }

        [Fact]
        public void UserTimeHandler_ShouldStopTask()
        {
            // Arrange
            var handler = GetCurrentHandler();

            var tasksList = new List<TimeTaskContext>
            {
                new TimeTaskContext
                {
                    Name = "NAME1",
                    Priority = Priority.Two,
                    OrderId = 0,
                    AssignedTime = new TimeSpan(1, 0, 0)
                },
                new TimeTaskContext
                {
                    Name = "NAME2",
                    Priority = Priority.Two,
                    OrderId = 1,
                    AssignedTime = new TimeSpan(1, 0, 0)
                },
                new TimeTaskContext
                {
                    Name = "NAME3",
                    Priority = Priority.Three,
                    OrderId = 2,
                    AssignedTime = new TimeSpan(1, 30, 0)
                },
            };

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
            var handler = GetCurrentHandler();

            var tasksList = new List<TimeTaskContext>
            {
                new TimeTaskContext
                {
                    Name = "NAME1",
                    Priority = Priority.Two,
                    OrderId = 0,
                    AssignedTime = new TimeSpan(1, 0, 0)
                },
                new TimeTaskContext
                {
                    Name = "NAME2",
                    Priority = Priority.Two,
                    OrderId = 1,
                    AssignedTime = new TimeSpan(1, 0, 0)
                },
                new TimeTaskContext
                {
                    Name = "NAME3",
                    Priority = Priority.Three,
                    OrderId = 2,
                    AssignedTime = new TimeSpan(1, 30, 0)
                },
            };

            // Act
            handler.StartTimeHandler(tasksList);
            handler.StopTask();
            handler.ResumeTask();

            // Assert
            Assert.True(handler.TaskTimer.Enabled);
        }
    }
}
