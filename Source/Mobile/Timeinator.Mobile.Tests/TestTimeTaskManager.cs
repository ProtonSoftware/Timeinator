using System;
using System.Collections.Generic;
using Timeinator.Core;
using Xunit;

namespace Timeinator.Mobile.Tests
{
    /// <summary>
    /// The tests for <see cref="TimeTaskManager"/> class
    /// </summary>
    public class TestTimeTaskManager
    {
        #region Setup

        /// <summary>
        /// Gets us current <see cref="TimeTasksManager"/> implementation as interface for testing
        /// </summary>
        private ITimeTasksManager GetCurrentManager() => new TimeTasksManager();

        #endregion

        [Fact]
        public void TimeTaskManager_TimeAssigning1()
        {
            // Arrange
            var manager = GetCurrentManager();

            var tasksList = new List<TimeTaskContext>
            {
                new TimeTaskContext
                {
                    Name = "NAME1",
                    Priority = Priority.Two,
                    OrderId = 0
                },
                new TimeTaskContext
                {
                    Name = "NAME2",
                    Priority = Priority.Two,
                    OrderId = 1
                },
                new TimeTaskContext
                {
                    Name = "NAME3",
                    Priority = Priority.Three,
                    OrderId = 2
                },
            };

            // Act
            manager.UploadTasksList(tasksList, new TimeSpan(7, 0, 0));
            var firstReturnedList = manager.GetCalculatedTasksListForSpecifiedTime();

            // Assert
            Assert.True(firstReturnedList.Count == 3);

            // slices 7 hours
            Assert.Equal(firstReturnedList[0].AssignedTime, new TimeSpan(2, 0, 0));
            Assert.Equal(firstReturnedList[1].AssignedTime, new TimeSpan(2, 0, 0));
            Assert.Equal(firstReturnedList[2].AssignedTime, new TimeSpan(3, 0, 0));
        }

        [Fact]
        public void TimeTaskManager_TimeAssigning2()
        {
            // Arrange
            var manager = GetCurrentManager();

            var tasksList = new List<TimeTaskContext>
            {
                new TimeTaskContext
                {
                    Name = "NAME1",
                    Priority = Priority.Two,
                    OrderId = 0
                },
                new TimeTaskContext
                {
                    Name = "NAME2",
                    Priority = Priority.Two,
                    OrderId = 1
                },
                new TimeTaskContext
                {
                    Name = "NAME3",
                    Priority = Priority.Three,
                    OrderId = 2
                },
            };

            // Act
            manager.UploadTasksList(tasksList, new TimeSpan(0, 49, 0));
            var secondReturnedList = manager.GetCalculatedTasksListForSpecifiedTime();

            // Assert
            Assert.True(secondReturnedList.Count == 3);

            // 49 minutes
            Assert.Equal(secondReturnedList[0].AssignedTime, new TimeSpan(0, 14, 0));
            Assert.Equal(secondReturnedList[1].AssignedTime, new TimeSpan(0, 14, 0));
            Assert.Equal(secondReturnedList[2].AssignedTime, new TimeSpan(0, 21, 0));
        }

        [Fact]
        public void TimeTaskManager_TimeAssigning3()
        {
            // Arrange
            var manager = GetCurrentManager();

            var tasksList = new List<TimeTaskContext>
            {
                new TimeTaskContext
                {
                    Name = "NAME1",
                    Priority = Priority.Two,
                    OrderId = 0
                },
                new TimeTaskContext
                {
                    Name = "NAME3",
                    Priority = Priority.Three,
                    OrderId = 2
                },
            };

            // Act
            manager.UploadTasksList(tasksList, new TimeSpan(2, 30, 0));
            var thirdReturnedList = manager.GetCalculatedTasksListForSpecifiedTime();

            // Assert
            Assert.True(thirdReturnedList.Count == 2);

            // 2 hours 30 minutes
            Assert.Equal(thirdReturnedList[0].AssignedTime, new TimeSpan(1, 0, 0));
            Assert.Equal(thirdReturnedList[1].AssignedTime, new TimeSpan(1, 30, 0));
        }
    }
}
