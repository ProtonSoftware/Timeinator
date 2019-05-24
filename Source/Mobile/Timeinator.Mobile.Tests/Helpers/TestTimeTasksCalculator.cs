using System;
using Timeinator.Mobile.Core;
using Xunit;

namespace Timeinator.Mobile.Tests
{
    /// <summary>
    /// Tests for <see cref="TimeTasksCalculator"/>
    /// </summary>
    public class TestTimeTasksCalculator
    {
        [Fact]
        public void TimeTasksCalculator_MinimumTime()
        {
            // Arrange
            var calculator = new TimeTasksCalculator();
            var tasksList = TestTaskListProvider.GetMockTimeTaskContexts(1);
            tasksList[0].HasConstantTime = true;
            tasksList[0].AssignedTime = new TimeSpan(1, 0, 0);

            var minimum = calculator.CalculateMinimumTimeForTasks(tasksList);

            // Check minimal time for provided tasks
            Assert.True(minimum == TimeSpan.FromMinutes(62));
        }

        [Fact]
        public void TimeTasksCalculator_ResultTime()
        {
            // Arrange
            var calculator = new TimeTasksCalculator();
            var tasksList = TestTaskListProvider.GetMockTimeTaskContexts(1);
            tasksList[1].AssignedTime = new TimeSpan(1, 0, 0);
            tasksList[2].AssignedTime = new TimeSpan(1, 30, 0);

            var sess = calculator.CalculateTasksForSession(tasksList, TimeSpan.FromMinutes(30));

            // Check times on session tasks
            Assert.True(sess.SumTimes() == TimeSpan.FromMinutes(3));
        }
    }
}
