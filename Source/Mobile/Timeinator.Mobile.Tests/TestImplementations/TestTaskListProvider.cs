using System;
using System.Collections.Generic;
using Timeinator.Core;

namespace Timeinator.Mobile.Tests
{
    /// <summary>
    /// Test class implementation to provide mocked lists of tasks to test on them
    /// </summary>
    public static class TestTaskListProvider
    {
        /// <summary>
        /// Gets mocked list of tasks based on specified index
        /// </summary>
        public static List<TimeTaskContext> GetMockTimeTaskContexts(int index)
        {
            switch(index)
            {
                case 1:
                    return new List<TimeTaskContext>
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

                case 2:
                    return new List<TimeTaskContext>
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

                case 3:
                    return new List<TimeTaskContext>
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

                case 4:
                    return new List<TimeTaskContext>
                    {
                        new TimeTaskContext
                        {
                            Name = "NAME1",
                            Priority = Priority.One,
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
                            IsImportant = true,
                            OrderId = 2
                        },
                        new TimeTaskContext
                        {
                            Name = "NAME4",
                            Priority = Priority.Four,
                            OrderId = 3
                        },
                        new TimeTaskContext
                        {
                            Name = "NAME5",
                            Priority = Priority.Five,
                            OrderId = 4
                        },
                        new TimeTaskContext
                        {
                            Name = "NAME6",
                            Priority = Priority.One,
                            OrderId = 5
                        },
                        new TimeTaskContext
                        {
                            Name = "NAME7",
                            Priority = Priority.Two,
                            IsImportant = true,
                            OrderId = 6
                        },
                        new TimeTaskContext
                        {
                            Name = "NAME8",
                            Priority = Priority.Three,
                            OrderId = 7
                        },
                    };

                case 5:
                    return new List<TimeTaskContext>
                    {
                        new TimeTaskContext
                        {
                            Name = "NAME1",
                            Priority = Priority.One,
                            OrderId = 0
                        },
                        new TimeTaskContext
                        {
                            Name = "NAME2",
                            Priority = Priority.One,
                            OrderId = 1
                        },
                        new TimeTaskContext
                        {
                            Name = "NAME3",
                            Priority = Priority.Five,
                            IsImportant = true,
                            OrderId = 2
                        }
                    };

                case 6:
                    return new List<TimeTaskContext>
                    {
                        new TimeTaskContext
                        {
                            Name = "NAME1",
                            Priority = Priority.One,
                            OrderId = 0
                        },
                        new TimeTaskContext
                        {
                            Name = "NAME2",
                            Priority = Priority.One,
                            AssignedTime = new TimeSpan(0, 15, 0),
                            OrderId = 1
                        },
                        new TimeTaskContext
                        {
                            Name = "NAME3",
                            Priority = Priority.Three,
                            IsImportant = true,
                            OrderId = 2
                        }
                    };

                case 7:
                    return new List<TimeTaskContext>
                    {
                        new TimeTaskContext
                        {
                            Name = "NAME1",
                            Priority = Priority.Five,
                            OrderId = 0
                        },
                        new TimeTaskContext
                        {
                            Name = "NAME2",
                            Priority = Priority.Five,
                            AssignedTime = new TimeSpan(0, 30, 0),
                            OrderId = 1
                        },
                        new TimeTaskContext
                        {
                            Name = "NAME3",
                            Priority = Priority.Five,
                            AssignedTime = new TimeSpan(0, 50, 0),
                            IsImportant = true,
                            OrderId = 2
                        },
                        new TimeTaskContext
                        {
                            Name = "NAME4",
                            Priority = Priority.One,
                            OrderId = 3
                        },
                        new TimeTaskContext
                        {
                            Name = "NAME5",
                            Priority = Priority.Three,
                            AssignedTime = new TimeSpan(0, 40, 0),
                            OrderId = 4
                        },
                        new TimeTaskContext
                        {
                            Name = "NAME6",
                            Priority = Priority.Four,
                            IsImportant = true,
                            OrderId = 5
                        }
                    };

                default:
                    return new List<TimeTaskContext>
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
            }
        }
    }
}
