﻿using System;
using System.Collections.Generic;
using Timeinator.Core;
using Timeinator.Mobile;
using Timeinator.Mobile.DataAccess;
using Xunit;

namespace Timeinator.Mobile.Tests
{
    /// <summary>
    /// The tests for <see cref="TimeTasksService"/>
    /// </summary>
    public class TestTimeTasksService : BaseDatabaseTests
    {
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
            Assert.True(returnedList.Count == 3);
            Assert.True(returnedList[0].Name == "NAME1");
            Assert.True(returnedList[2].Priority == Priority.Three);
        }
    }
}
