using System;
using System.Collections.Generic;
using Timeinator.Core;
using Xunit;

namespace Timeinator.Mobile.Tests
{
    /// <summary>
    /// The tests for <see cref="TimeTasksManager"/> class
    /// </summary>
    public class TestTimeTasksManager
    {
        /// <summary>
        /// Simple test with assigning time
        /// </summary>
        [Fact]
        public void TimeTaskManager_ShouldAssignTime1()
        {
            // Arrange
            var manager = new TimeTasksManager();
            var tasksList = TestTaskListProvider.GetMockTimeTaskContexts(1);

            // Act
            manager.UploadTasksList(tasksList, new TimeSpan(7, 0, 0));
            var firstReturnedList = manager.GetCalculatedTasksListForSpecifiedTime();

            // Assert
            Assert.True(firstReturnedList.Count == tasksList.Count);

            // 7 hours
            Assert.Equal(firstReturnedList[0].AssignedTime, new TimeSpan(2, 0, 0));
            Assert.Equal(firstReturnedList[1].AssignedTime, new TimeSpan(2, 0, 0));
            Assert.Equal(firstReturnedList[2].AssignedTime, new TimeSpan(3, 0, 0));
        }

        /// <summary>
        /// Simple test with assigning time
        /// </summary>
        [Fact]
        public void TimeTaskManager_ShouldAssignTime2()
        {
            // Arrange
            var manager = new TimeTasksManager();
            var tasksList = TestTaskListProvider.GetMockTimeTaskContexts(2);

            // Act
            manager.UploadTasksList(tasksList, new TimeSpan(0, 49, 0));
            var secondReturnedList = manager.GetCalculatedTasksListForSpecifiedTime();

            // Assert
            Assert.True(secondReturnedList.Count == tasksList.Count);

            // 49 minutes
            Assert.Equal(secondReturnedList[0].AssignedTime, new TimeSpan(0, 14, 0));
            Assert.Equal(secondReturnedList[1].AssignedTime, new TimeSpan(0, 14, 0));
            Assert.Equal(secondReturnedList[2].AssignedTime, new TimeSpan(0, 21, 0));
        }

        /// <summary>
        /// Simple test with assigning time for only two tasks
        /// </summary>
        [Fact]
        public void TimeTaskManager_ShouldAssignTime3()
        {
            // Arrange
            var manager = new TimeTasksManager();
            var tasksList = TestTaskListProvider.GetMockTimeTaskContexts(3);

            // Act
            manager.UploadTasksList(tasksList, new TimeSpan(2, 30, 0));
            var thirdReturnedList = manager.GetCalculatedTasksListForSpecifiedTime();

            // Assert
            Assert.True(thirdReturnedList.Count == tasksList.Count);

            // 2 hours 30 minutes
            Assert.Equal(thirdReturnedList[0].AssignedTime, new TimeSpan(1, 0, 0));
            Assert.Equal(thirdReturnedList[1].AssignedTime, new TimeSpan(1, 30, 0));
        }

        /// <summary>
        /// More advanced test that requires time rounding for seconds
        /// </summary>
        [Fact]
        public void TimeTaskManager_ShouldAssignTime4()
        {
            // Arrange
            var manager = new TimeTasksManager();
            var tasksList = TestTaskListProvider.GetMockTimeTaskContexts(4);

            // Act
            manager.UploadTasksList(tasksList, new TimeSpan(2, 0, 0));
            var fourthReturnedList = manager.GetCalculatedTasksListForSpecifiedTime();

            // Assert
            Assert.True(fourthReturnedList.Count == tasksList.Count);

            // 2 hours
            Assert.Equal(fourthReturnedList[0].AssignedTime, new TimeSpan(0, 5, 43));
            Assert.Equal(fourthReturnedList[1].AssignedTime, new TimeSpan(0, 11, 26));
            Assert.Equal(fourthReturnedList[2].AssignedTime, new TimeSpan(0, 17, 9));
            Assert.Equal(fourthReturnedList[3].AssignedTime, new TimeSpan(0, 22, 52));
            Assert.Equal(fourthReturnedList[4].AssignedTime, new TimeSpan(0, 28, 35));
            Assert.Equal(fourthReturnedList[5].AssignedTime, new TimeSpan(0, 5, 43));
            Assert.Equal(fourthReturnedList[6].AssignedTime, new TimeSpan(0, 11, 26));
            Assert.Equal(fourthReturnedList[7].AssignedTime, new TimeSpan(0, 17, 9));
        }

        /// <summary>
        /// Test that utilizes application's requirement about minimum 1 minute per task
        /// Although the tasks aren't equal priority, it should assign them the same time because of that
        /// </summary>
        [Fact]
        public void TimeTaskManager_ShouldAssignTime5()
        {
            // Arrange
            var manager = new TimeTasksManager();
            var tasksList = TestTaskListProvider.GetMockTimeTaskContexts(5);

            // Act
            manager.UploadTasksList(tasksList, new TimeSpan(0, 3, 0));
            var fifthReturnedList = manager.GetCalculatedTasksListForSpecifiedTime();

            // Assert
            Assert.True(fifthReturnedList.Count == tasksList.Count);

            // 3 minutes
            Assert.Equal(fifthReturnedList[0].AssignedTime, new TimeSpan(0, 1, 0));
            Assert.Equal(fifthReturnedList[1].AssignedTime, new TimeSpan(0, 1, 0));
            Assert.Equal(fifthReturnedList[2].AssignedTime, new TimeSpan(0, 1, 0));
        }

        /// <summary>
        /// Simple test with assigning time with one task constant time
        /// </summary>
        [Fact]
        public void TimeTaskManager_ShouldAssignTime6()
        {
            // Arrange
            var manager = new TimeTasksManager();
            var tasksList = TestTaskListProvider.GetMockTimeTaskContexts(6);

            // Act
            manager.UploadTasksList(tasksList, new TimeSpan(1, 0, 0));
            var firstReturnedList = manager.GetCalculatedTasksListForSpecifiedTime();

            // Assert
            Assert.True(firstReturnedList.Count == tasksList.Count);

            // 1 hour
            Assert.Equal(firstReturnedList[0].AssignedTime, new TimeSpan(0, 11, 15));
            Assert.Equal(firstReturnedList[1].AssignedTime, new TimeSpan(0, 15, 0));
            Assert.Equal(firstReturnedList[2].AssignedTime, new TimeSpan(0, 33, 45));
        }

        /// <summary>
        /// Advanced test with assigning time with mutliple tasks having assigned constant time already
        /// </summary>
        [Fact]
        public void TimeTaskManager_ShouldAssignTime7()
        {
            // Arrange
            var manager = new TimeTasksManager();
            var tasksList = TestTaskListProvider.GetMockTimeTaskContexts(7);

            // Act
            manager.UploadTasksList(tasksList, new TimeSpan(2, 30, 0));
            var firstReturnedList = manager.GetCalculatedTasksListForSpecifiedTime();

            // Assert
            Assert.True(firstReturnedList.Count == tasksList.Count);

            // 2 hours 30 mins
            Assert.Equal(firstReturnedList[0].AssignedTime, new TimeSpan(0, 15, 0));
            Assert.Equal(firstReturnedList[1].AssignedTime, new TimeSpan(0, 30, 0));
            Assert.Equal(firstReturnedList[2].AssignedTime, new TimeSpan(0, 50, 0));
            Assert.Equal(firstReturnedList[3].AssignedTime, new TimeSpan(0, 3, 0));
            Assert.Equal(firstReturnedList[4].AssignedTime, new TimeSpan(0, 40, 0));
            Assert.Equal(firstReturnedList[5].AssignedTime, new TimeSpan(0, 12, 0));
        }

        /// <summary>
        /// Test with constant times that utilizes minimum time assignment
        /// Provided time is not enough to properly distribute, so every task should have absolute minimum
        /// (Those with constants should remain unchanged though)
        /// </summary>
        [Fact]
        public void TimeTaskManager_ShouldAssignTime8()
        {
            // Arrange
            var manager = new TimeTasksManager();
            var tasksList = TestTaskListProvider.GetMockTimeTaskContexts(7);

            // Act
            manager.UploadTasksList(tasksList, new TimeSpan(1, 0, 0));
            var firstReturnedList = manager.GetCalculatedTasksListForSpecifiedTime();

            // Assert
            Assert.True(firstReturnedList.Count == tasksList.Count);

            // 1 hour
            Assert.Equal(firstReturnedList[0].AssignedTime, new TimeSpan(0, 1, 0));
            Assert.Equal(firstReturnedList[1].AssignedTime, new TimeSpan(0, 30, 0));
            Assert.Equal(firstReturnedList[2].AssignedTime, new TimeSpan(0, 50, 0));
            Assert.Equal(firstReturnedList[3].AssignedTime, new TimeSpan(0, 1, 0));
            Assert.Equal(firstReturnedList[4].AssignedTime, new TimeSpan(0, 40, 0));
            Assert.Equal(firstReturnedList[5].AssignedTime, new TimeSpan(0, 1, 0));
        }
    }
}
