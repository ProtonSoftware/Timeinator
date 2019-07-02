using System.Collections.Generic;
using System.Linq;
using Timeinator.Core;
using Timeinator.Mobile.Core;
using Timeinator.Mobile.DataAccess;
using Xunit;

namespace Timeinator.Mobile.Tests
{
    /// <summary>
    /// The tests for <see cref="TimeTasksService"/>
    /// </summary>
    public class TestTimeTasksRepository : BaseDatabaseTests
    {
        /// <summary>
        /// Simple test that saves tasks in the database and tries to load them
        /// </summary>
        [Fact]
        public void TestTimeTasksRepository_ShouldSaveAndLoadFromDatabase()
        {
            // Arrange
            var repository = new TimeTasksRepository(DatabaseContext);
            var mapper = new TimeTasksMapper();
            var tasksList = TestTaskListProvider.GetMockTimeTaskContexts(1);

            // Act
            repository.SaveTask(mapper.ReverseMap(tasksList[0]));
            repository.SaveTask(mapper.ReverseMap(tasksList[1]));
            repository.SaveTask(mapper.ReverseMap(tasksList[2]));

            var returnedList = repository.GetSavedTasksForToday("").ToList();

            // Assert
            Assert.True(returnedList.Count == tasksList.Count);
            Assert.True(returnedList[0].Name == "NAME1");
            Assert.True(returnedList[2].Priority == Priority.Three);
        }

        /// <summary>
        /// Simple test that removes specified task from the database via service
        /// </summary>
        [Fact]
        public void TestTimeTasksRepository_ShouldRemoveTask()
        {
            // Arrange
            var repository = new TimeTasksRepository(DatabaseContext);
            var mapper = new TimeTasksMapper();
            var tasksList = TestTaskListProvider.GetMockTimeTaskContexts(1);
            var task = tasksList[1];

            // Act
            repository.SaveTask(mapper.ReverseMap(tasksList[0]));
            repository.SaveTask(mapper.ReverseMap(tasksList[1]));
            repository.SaveTask(mapper.ReverseMap(tasksList[2]));

            repository.RemoveTasks(new List<int> { task.Id } );

            var returnedList = repository.GetSavedTasksForToday("").ToList();

            // Assert
            Assert.True(returnedList.Count == tasksList.Count - 1);
            Assert.True(returnedList[0].Name == "NAME1");
            Assert.True(returnedList[1].Name == "NAME2");
            Assert.True(returnedList[1].Priority == Priority.Three);
        }
    }
}
