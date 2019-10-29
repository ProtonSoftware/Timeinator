using AutoMapper;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// The mapper for TimeTasks objects
    /// </summary>
    public class TimeTasksMapper
    {
        #region Private Members

        /// <summary>
        /// AutoMapper configuration for this mapepr
        /// </summary>
        private readonly IMapper mMapper;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TimeTasksMapper()
        {
            // Configure the AutoMapper 
            mMapper = new MapperConfiguration(config =>
            {
                // Create TimeTask entity to TimeTaskContext map
                config.CreateMap<TimeTask, TimeTaskContext>()
                      // Map the HasConstantTime flag based on whether entity has specified assigned time or not
                      .ForMember(context => context.HasConstantTime, options => options.MapFrom(ent => ent.AssignedTime != default))
                      // Map the Tags list from entity's collection
                      .ForMember(context => context.Tags, options => options.MapFrom(ent => ent.Tags))
                      // And the other way around
                      .ReverseMap();
                // Create TimeTaskContext to ListTimeTaskItemViewModel map
                config.CreateMap<TimeTaskContext, ListTimeTaskItemViewModel>()
                      // For context => VM mapping, assign the assigned time flag accordingly
                      .ForMember(viewmodel => viewmodel.IsAssignedTime, options => options.MapFrom(context => context.HasConstantTime))
                      // And the other way around        
                      .ReverseMap()
                      // For VM => context mapping, assign the constant time flag accordingly
                      .ForMember(context => context.HasConstantTime, options => options.MapFrom(vm => vm.IsAssignedTime && vm.AssignedTime != default));
                // Create TimeTaskContext to SummaryTimeTaskItemViewModel map
                config.CreateMap<TimeTaskContext, SummaryTimeTaskItemViewModel>()
                      // And the other way around
                      .ReverseMap();
                // Create TimeTaskContext to SessionTimeTaskItemViewModel map
                config.CreateMap<TimeTaskContext, SessionTimeTaskItemViewModel>()
                      // And the other way around
                      .ReverseMap();
            })
            // And create it afterwards
            .CreateMapper();
        }

        #endregion

        #region Mapping Methods

        /// <summary>
        /// Detects order of <see cref="TimeTaskViewModel"/> by Id and applies it to list of <see cref="TimeTaskContext"/>
        /// </summary>
        public List<TimeTaskContext> SortLike(List<TimeTaskViewModel> taskViewModels, List<TimeTaskContext> taskContexts)
        {
            // Map user order of tasks to a hashtable
            var posForId = new Hashtable(taskContexts.Count);
            for (var i = 0; i < taskViewModels.Count; i++)
                posForId.Add(taskViewModels[i].Id, i);
            // Order tasks by mapped order
            return taskContexts.OrderBy((t) => posForId[t.Id]).ToList();
        }

        public AddNewTimeTaskPageViewModel Map(ListTimeTaskItemViewModel taskViewModel, AddNewTimeTaskPageViewModel pageViewModel)
        {
            pageViewModel.TaskId = taskViewModel.Id;
            pageViewModel.TaskName = taskViewModel.Name;
            pageViewModel.TaskDescription = taskViewModel.Description;
            pageViewModel.TaskTagsString = taskViewModel.Tags.CreateTagsString();
            pageViewModel.TaskConstantTime = taskViewModel.AssignedTime;
            pageViewModel.TaskHasConstantTime = taskViewModel.IsAssignedTime;
            pageViewModel.TaskImmortality = taskViewModel.IsImmortal;
            pageViewModel.TaskPrioritySliderValue = (int)taskViewModel.Priority;
            pageViewModel.TaskImportance = taskViewModel.IsImportant;
            pageViewModel.TaskMaximumProgress = taskViewModel.MaxProgress;

            return pageViewModel;
        }

        /// <summary>
        /// Maps a list of <see cref="TimeTask"/> to a list of <see cref="TimeTaskContext"/> objects
        /// </summary>
        /// <param name="entities">The list of <see cref="TimeTask"/> to map</param>
        /// <returns>The list of <see cref="TimeTaskContext"/></returns>
        public List<TimeTaskContext> ListMap(List<TimeTask> entities) => mMapper.Map<List<TimeTaskContext>>(entities);

        /// <summary>
        /// Maps a list of <see cref="TimeTaskContext"/> to a list of <see cref="ListTimeTaskItemViewModel"/> objects
        /// </summary>
        /// <param name="contexts">The list of <see cref="TimeTaskContext"/> to map</param>
        /// <returns>The list of <see cref="ListTimeTaskItemViewModel"/></returns>
        public List<ListTimeTaskItemViewModel> ListMapToList(List<TimeTaskContext> contexts) => mMapper.Map<List<ListTimeTaskItemViewModel>>(contexts);

        /// <summary>
        /// Maps a list of <see cref="TimeTaskContext"/> to a list of <see cref="SummaryTimeTaskItemViewModel"/> objects
        /// </summary>
        /// <param name="contexts">The list of <see cref="TimeTaskContext"/> to map</param>
        /// <returns>The list of <see cref="SummaryTimeTaskItemViewModel"/></returns>
        public List<SummaryTimeTaskItemViewModel> ListMapToSummary(List<TimeTaskContext> contexts) => mMapper.Map<List<SummaryTimeTaskItemViewModel>>(contexts);

        /// <summary>
        /// Maps a list of <see cref="TimeTaskContext"/> to a list of <see cref="SessionTimeTaskItemViewModel"/> objects
        /// </summary>
        /// <param name="contexts">The list of <see cref="TimeTaskContext"/> to map</param>
        /// <returns>The list of <see cref="SessionTimeTaskItemViewModel"/></returns>
        public List<SessionTimeTaskItemViewModel> ListMapToSession(List<TimeTaskContext> contexts) => mMapper.Map<List<SessionTimeTaskItemViewModel>>(contexts);

        /// <summary>
        /// Maps a <see cref="TimeTaskContext"/> to a <see cref="TimeTask"/> object
        /// </summary>
        /// <param name="context">The <see cref="TimeTaskContext"/> to map</param>
        /// <returns><see cref="TimeTask"/></returns>
        public TimeTask ReverseMap(TimeTaskContext context) => mMapper.Map<TimeTask>(context);

        /// <summary>
        /// Maps a <see cref="ListTimeTaskItemViewModel"/> to a <see cref="TimeTaskContext"/> object
        /// </summary>
        /// <param name="viewmodel">The <see cref="ListTimeTaskItemViewModel"/> to map</param>
        /// <returns><see cref="TimeTaskContext"/></returns>
        public TimeTaskContext ReverseMap(ListTimeTaskItemViewModel viewmodel) => mMapper.Map<TimeTaskContext>(viewmodel);

        #endregion
    }
}
