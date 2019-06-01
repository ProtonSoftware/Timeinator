using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Timeinator.Core;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The view model for time task control
    /// </summary>
    public class TimeTaskViewModel : MvxViewModel
    {
        #region Public Properties

        /// <summary>
        /// Unique id number of the task
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of this task
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Unique ordinal number of the task
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// The priority of this task
        /// </summary>
        public Priority Priority { get; set; }
        
        /// <summary>
        /// Current progress of this task
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        /// The description of this task
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The list of tags for this task
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// Indicates if this task is important
        /// </summary>
        public bool IsImportant { get; set; }

        /// <summary>
        /// Indicates if task is intended to be done in the next session
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Indicates if this task has contant time assigned
        /// If false, AssignedTime property is ignored
        /// </summary>
        public bool IsAssignedTime { get; set; }

        /// <summary>
        /// The time assigned for this specific task
        /// </summary>
        public TimeSpan AssignedTime { get; set; }

        /// <summary>
        /// Indicates if this task should still be saved after its done
        /// Set to false if it should be removed after completion
        /// </summary>
        public bool IsImmortal { get; set; }

        /// <summary>
        /// The date when this task was initially created
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Indicates if context menu for this item should be visible
        /// </summary>
        public bool IsContextMenuVisible { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// The event to fire whenever user wants to edit this task
        /// </summary>
        public event Action<object> OnEditRequest = (vm) => { };

        /// <summary>
        /// The event to fire whenever user wants to remove this task
        /// </summary>
        public event Action<object> OnDeleteRequest = (vm) => { };

        #endregion

        #region Commands

        /// <summary>
        /// The command to fire editing event
        /// </summary>
        public ICommand EditCommand { get; private set; }

        /// <summary>
        /// The command to fire deleting event
        /// </summary>
        public ICommand DeleteCommand { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TimeTaskViewModel()
        {
            // Create commands
            EditCommand = new RelayCommand(() => OnEditRequest.Invoke(this));
            DeleteCommand = new RelayCommand(() => OnDeleteRequest.Invoke(this));
        }

        #endregion
    }
}
