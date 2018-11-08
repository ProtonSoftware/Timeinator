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
        public void TimeTaskManager_ShouldAssignTimeToProvidedTasks()
        {
            // Arrange
            var manager = GetCurrentManager();

            var tasksList = new List<TimeTaskContext>
            {
                new TimeTaskContext
                {
                    Name = "NAME1",
                    Priority = Priority.Two
                },
                new TimeTaskContext
                {
                    Name = "NAME2",
                    Priority = Priority.Two
                },
                new TimeTaskContext
                {
                    Name = "NAME3",
                    Priority = Priority.Three
                },
            };

            // Act
            manager.UploadTasksList(tasksList);
            var firstReturnedList = manager.GetCalculatedTasksListForSpecifiedTime(new TimeSpan(6, 0, 0));
            var secondReturnedList = manager.GetCalculatedTasksListForSpecifiedTime(new TimeSpan(0, 1, 0));

            tasksList.RemoveAt(1);
            manager.UploadTasksList(tasksList);
            var thirdReturnedList = manager.GetCalculatedTasksListForSpecifiedTime(new TimeSpan(6, 0, 0));

            // Assert
            Assert.True(firstReturnedList.Count == 3);
            Assert.True(secondReturnedList.Count == 3);
            Assert.True(thirdReturnedList.Count == 2);

            // TODO: Calculate exact values and put it here, 1h 40min etc is a placeholder
            Assert.Equal(firstReturnedList[0].AssignedTime, new TimeSpan(1, 40, 0));
            Assert.Equal(firstReturnedList[1].AssignedTime, new TimeSpan(1, 40, 0));
            Assert.Equal(firstReturnedList[2].AssignedTime, new TimeSpan(2, 40, 0));

            Assert.Equal(secondReturnedList[0].AssignedTime, new TimeSpan(0, 0, 15));
            Assert.Equal(secondReturnedList[1].AssignedTime, new TimeSpan(0, 0, 15));
            Assert.Equal(secondReturnedList[2].AssignedTime, new TimeSpan(0, 0, 30));

            Assert.Equal(thirdReturnedList[0].AssignedTime, new TimeSpan(2, 30, 0));
            Assert.Equal(thirdReturnedList[1].AssignedTime, new TimeSpan(3, 30, 0));
        }
    }
}
