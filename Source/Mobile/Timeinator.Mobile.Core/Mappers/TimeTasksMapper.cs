using AutoMapper;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Timeinator.Mobile.DataAccess;

namespace Timeinator.Mobile.Core
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
                      .ForMember(context => context.Tags, options => options.MapFrom(ent => ent.Tags.ToList()))
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

        /// <summary>
        /// Maps a <see cref="TimeTask"/> to a <see cref="TimeTaskContext"/> object
        /// </summary>
        /// <param name="entity">The <see cref="TimeTask"/> to map</param>
        /// <returns><see cref="TimeTaskContext"/></returns>
        public TimeTaskContext Map(TimeTask entity) => mMapper.Map<TimeTaskContext>(entity);

        /// <summary>
        /// Maps a list of <see cref="TimeTask"/> to a list of <see cref="TimeTaskContext"/> objects
        /// </summary>
        /// <param name="entities">The list of <see cref="TimeTask"/> to map</param>
        /// <returns>The list of <see cref="TimeTaskContext"/></returns>
        public List<TimeTaskContext> ListMap(List<TimeTask> entities) => mMapper.Map<List<TimeTaskContext>>(entities);

        /// <summary>
        /// Maps a <see cref="TimeTaskContext"/> to a <see cref="ListTimeTaskItemViewModel"/> object
        /// </summary>
        /// <param name="context">The <see cref="TimeTaskContext"/> to map</param>
        /// <returns><see cref="ListTimeTaskItemViewModel"/></returns>
        public ListTimeTaskItemViewModel MapToList(TimeTaskContext context) => mMapper.Map<ListTimeTaskItemViewModel>(context);

        /// <summary>
        /// Maps a list of <see cref="TimeTaskContext"/> to a list of <see cref="ListTimeTaskItemViewModel"/> objects
        /// </summary>
        /// <param name="contexts">The list of <see cref="TimeTaskContext"/> to map</param>
        /// <returns>The list of <see cref="ListTimeTaskItemViewModel"/></returns>
        public List<ListTimeTaskItemViewModel> ListMapToList(List<TimeTaskContext> contexts) => mMapper.Map<List<ListTimeTaskItemViewModel>>(contexts);

        /// <summary>
        /// Maps a <see cref="TimeTaskContext"/> to a <see cref="SummaryTimeTaskItemViewModel"/> object
        /// </summary>
        /// <param name="context">The <see cref="TimeTaskContext"/> to map</param>
        /// <returns><see cref="SummaryTimeTaskItemViewModel"/></returns>
        public SummaryTimeTaskItemViewModel MapToSummary(TimeTaskContext context) => mMapper.Map<SummaryTimeTaskItemViewModel>(context);

        /// <summary>
        /// Maps a list of <see cref="TimeTaskContext"/> to a list of <see cref="SummaryTimeTaskItemViewModel"/> objects
        /// </summary>
        /// <param name="contexts">The list of <see cref="TimeTaskContext"/> to map</param>
        /// <returns>The list of <see cref="SummaryTimeTaskItemViewModel"/></returns>
        public List<SummaryTimeTaskItemViewModel> ListMapToSummary(List<TimeTaskContext> contexts) => mMapper.Map<List<SummaryTimeTaskItemViewModel>>(contexts);

        /// <summary>
        /// Maps a <see cref="TimeTaskContext"/> to a <see cref="SessionTimeTaskItemViewModel"/> object
        /// </summary>
        /// <param name="context">The <see cref="TimeTaskContext"/> to map</param>
        /// <returns><see cref="SessionTimeTaskItemViewModel"/></returns>
        public SessionTimeTaskItemViewModel MapToSession(TimeTaskContext context) => mMapper.Map<SessionTimeTaskItemViewModel>(context);

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

        /// <summary>
        /// Maps a list of <see cref="ListTimeTaskItemViewModel"/> to a list of <see cref="TimeTaskContext"/> objects
        /// </summary>
        /// <param name="viewmodels">The list of <see cref="ListTimeTaskItemViewModel"/> to map</param>
        /// <returns>The list of <see cref="TimeTaskContext"/></returns>
        public List<TimeTaskContext> ListReverseMap(List<ListTimeTaskItemViewModel> viewmodels) => mMapper.Map<List<TimeTaskContext>>(viewmodels);

        /// <summary>
        /// Maps a <see cref="SummaryTimeTaskItemViewModel"/> to a <see cref="TimeTaskContext"/> object
        /// </summary>
        /// <param name="viewmodel">The <see cref="SummaryTimeTaskItemViewModel"/> to map</param>
        /// <returns><see cref="TimeTaskContext"/></returns>
        public TimeTaskContext ReverseMap(SummaryTimeTaskItemViewModel viewmodel) => mMapper.Map<TimeTaskContext>(viewmodel);

        /// <summary>
        /// Maps a list of <see cref="SummaryTimeTaskItemViewModel"/> to a list of <see cref="TimeTaskContext"/> objects
        /// </summary>
        /// <param name="viewmodels">The list of <see cref="SummaryTimeTaskItemViewModel"/> to map</param>
        /// <returns>The list of <see cref="TimeTaskContext"/></returns>
        public List<TimeTaskContext> ListReverseMap(List<SummaryTimeTaskItemViewModel> viewmodels) => mMapper.Map<List<TimeTaskContext>>(viewmodels);

        /// <summary>
        /// Maps a <see cref="SessionTimeTaskItemViewModel"/> to a <see cref="TimeTaskContext"/> object
        /// </summary>
        /// <param name="viewmodel">The <see cref="SessionTimeTaskItemViewModel"/> to map</param>
        /// <returns><see cref="TimeTaskContext"/></returns>
        public TimeTaskContext ReverseMap(SessionTimeTaskItemViewModel viewmodel) => mMapper.Map<TimeTaskContext>(viewmodel);

        /// <summary>
        /// Maps a list of <see cref="SessionTimeTaskItemViewModel"/> to a list of <see cref="TimeTaskContext"/> objects
        /// </summary>
        /// <param name="viewmodels">The list of <see cref="SessionTimeTaskItemViewModel"/> to map</param>
        /// <returns>The list of <see cref="TimeTaskContext"/></returns>
        public List<TimeTaskContext> ListReverseMap(List<SessionTimeTaskItemViewModel> viewmodels) => mMapper.Map<List<TimeTaskContext>>(viewmodels);

        #endregion
    }
}
