using System;
using System.Collections.Generic;
using Timeinator.Core;
using Timeinator.Mobile;
using Timeinator.Mobile.Core;
using Timeinator.Mobile.DataAccess;
using Xunit;

namespace Timeinator.Mobile.Tests
{
    /// <summary>
    /// The tests for <see cref="TimeTasksService"/>
    /// </summary>
    public class TestTimeTasksService : BaseDatabaseTests
    {
        /// <summary>
        /// Simple test that saves tasks in the database and calls service to load them properly
        /// </summary>
        [Fact]
        public void TimeTaskService_ShouldSaveAndLoadFromDatabase()
        {
            // Arrange
            var manager = new TimeTasksManager();
            var repository = new TimeTasksRepository(DatabaseContext);
            var handler = new UserTimeHandler();
            var mapper = new TimeTasksMapper();

            var service = new TimeTasksService(manager, repository, handler, mapper);
            var tasksList = TestTaskListProvider.GetMockTimeTaskContexts(1);

            // Act
            repository.SaveTask(mapper.ReverseMap(tasksList[0]));
            repository.SaveTask(mapper.ReverseMap(tasksList[1]));
            repository.SaveTask(mapper.ReverseMap(tasksList[2]));

            var returnedList = service.LoadStoredTasks();

            // Assert
            Assert.True(returnedList.Count == tasksList.Count);
            Assert.True(returnedList[0].Name == "NAME1");
            Assert.True(returnedList[2].Priority == Priority.Three);
        }

        /// <summary>
        /// Simple test that makes calls to the manager using service
        /// </summary>
        [Fact]
        public void TimeTaskService_ShouldHandleManagerCalls()
        {
            // Arrange
            var manager = new TimeTasksManager();
            var repository = new TimeTasksRepository(DatabaseContext);
            var handler = new UserTimeHandler();
            var mapper = new TimeTasksMapper();

            var service = new TimeTasksService(manager, repository, handler, mapper);
            var tasksList = TestTaskListProvider.GetMockTimeTaskContexts(2);

            // Act
            service.ConveyTasksToManager(tasksList, new TimeSpan(0, 49, 0));
            var returnedList = service.GetCalculatedTasksFromManager();

            // Assert
            Assert.True(returnedList.Count == tasksList.Count);
            Assert.Equal(returnedList[0].AssignedTime, new TimeSpan(0, 14, 0));
            Assert.Equal(returnedList[1].AssignedTime, new TimeSpan(0, 14, 0));
            Assert.Equal(returnedList[2].AssignedTime, new TimeSpan(0, 21, 0));
        }

        /// <summary>
        /// Simple test that removes specified task from the database via service
        /// </summary>
        [Fact]
        public void TimeTaskService_ShouldRemoveTask()
        {
            // Arrange
            var manager = new TimeTasksManager();
            var repository = new TimeTasksRepository(DatabaseContext);
            var handler = new UserTimeHandler();
            var mapper = new TimeTasksMapper();

            var service = new TimeTasksService(manager, repository, handler, mapper);
            var tasksList = TestTaskListProvider.GetMockTimeTaskContexts(1);
            var task = tasksList[1];

            // Act
            repository.SaveTask(mapper.ReverseMap(tasksList[0]));
            repository.SaveTask(mapper.ReverseMap(tasksList[1]));
            repository.SaveTask(mapper.ReverseMap(tasksList[2]));

            service.RemoveTask(task);

            var returnedList = service.LoadStoredTasks();

            // Assert
            Assert.True(returnedList.Count == tasksList.Count - 1);
            Assert.True(returnedList[0].Name == "NAME1");
            Assert.True(returnedList[1].Name == "NAME2");
            Assert.True(returnedList[1].Priority == Priority.Three);
        }
    }
}
