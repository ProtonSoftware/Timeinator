using System;
using System.Collections.Generic;
using Timeinator.Core;
using Xunit;

namespace Timeinator.Mobile.Tests
{
    /// <summary>
    /// The tests for <see cref="TasksSessionPageViewModel"/>
    /// </summary>
    public class TestTaskSessionPageViewModel
    {
        #region Setup

        #endregion

        [Fact]
        public void TaskSessionPageViewModel_Should()
        {
            // Arrange
            //var pageviewmodel = GetPageViewModel();

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

            // Assert
            Assert.True(true);
        }

    }
}
