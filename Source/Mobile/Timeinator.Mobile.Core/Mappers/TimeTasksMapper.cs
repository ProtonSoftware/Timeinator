using AutoMapper;
using System;
using System.Collections.Generic;
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
                      .ForMember(context => context.HasConstantTime, options => options.MapFrom(ent => ent.AssignedTime != default(TimeSpan)))
                      // And the other way around
                      .ReverseMap();
                // Create TimeTaskContext to TimeTaskViewModel map
                config.CreateMap<TimeTaskContext, TimeTaskViewModel>()
                      // And the other way around        
                      .ReverseMap()
                      // For VM => context mapping, assign the constant time flag accordingly
                      .ForMember(context => context.HasConstantTime, options => options.MapFrom(vm => vm.AssignedTime != default(TimeSpan)));
                // Create TimeTaskContext to CalculatedTimeTaskViewModel map
                config.CreateMap<TimeTaskContext, CalculatedTimeTaskViewModel>()
                      // And the other way around
                      .ReverseMap();
            })
            // And create it afterwards
            .CreateMapper();
        }

        #endregion

        #region Mapping Methods

        /// <summary>
        /// Maps a <see cref="TimeTask"/> to a <see cref="TimeTaskContext"/> object
        /// </summary>
        /// <param name="entity">The <see cref="TimeTask"/> to map</param>
        /// <returns><see cref="TimeTaskContext"/></returns>
        public TimeTaskContext Map(TimeTask entity) => mMapper.Map<TimeTaskContext>(entity);

        /// <summary>
        /// Maps a <see cref="TimeTaskContext"/> to a <see cref="TimeTaskViewModel"/> object
        /// </summary>
        /// <param name="context">The <see cref="TimeTaskContext"/> to map</param>
        /// <returns><see cref="TimeTaskViewModel"/></returns>
        public TimeTaskViewModel Map(TimeTaskContext context) => mMapper.Map<TimeTaskViewModel>(context);
        public List<TimeTaskViewModel> ListMap(List<TimeTaskContext> contexts)
        {
            var n = new List<TimeTaskViewModel>();
            foreach (var c in contexts)
                n.Add(Map(c));
            return n;
        }

        /// <summary>
        /// Maps a <see cref="TimeTaskContext"/> to a <see cref="CalculatedTimeTaskViewModel"/> object
        /// </summary>
        /// <param name="context">The <see cref="TimeTaskContext"/> to map</param>
        /// <returns><see cref="CalculatedTimeTaskViewModel"/></returns>
        public CalculatedTimeTaskViewModel MapCal(TimeTaskContext context) => mMapper.Map<CalculatedTimeTaskViewModel>(context);
        public List<CalculatedTimeTaskViewModel> ListMapCal(List<TimeTaskContext> contexts)
        {
            var n = new List<CalculatedTimeTaskViewModel>();
            foreach (var c in contexts)
                n.Add(MapCal(c));
            return n;
        }

        /// <summary>
        /// Maps a <see cref="TimeTaskContext"/> to a <see cref="TimeTask"/> object
        /// </summary>
        /// <param name="context">The <see cref="TimeTaskContext"/> to map</param>
        /// <returns><see cref="TimeTask"/></returns>
        public TimeTask ReverseMap(TimeTaskContext context) => mMapper.Map<TimeTask>(context);

        /// <summary>
        /// Maps a <see cref="TimeTaskViewModel"/> to a <see cref="TimeTaskContext"/> object
        /// </summary>
        /// <param name="viewmodel">The <see cref="TimeTaskViewModel"/> to map</param>
        /// <returns><see cref="TimeTaskContext"/></returns>
        public TimeTaskContext ReverseMap(TimeTaskViewModel viewmodel) => mMapper.Map<TimeTaskContext>(viewmodel);
        public List<TimeTaskContext> ListReverseMap(List<TimeTaskViewModel> contexts)
        {
            var n = new List<TimeTaskContext>();
            foreach (var c in contexts)
                n.Add(ReverseMap(c));
            return n;
        }

        /// <summary>
        /// Maps a <see cref="CalculatedTimeTaskViewModel"/> to a <see cref="TimeTaskContext"/> object
        /// </summary>
        /// <param name="viewmodel">The <see cref="CalculatedTimeTaskViewModel"/> to map</param>
        /// <returns><see cref="TimeTaskContext"/></returns>
        public TimeTaskContext ReverseMap(CalculatedTimeTaskViewModel viewmodel) => mMapper.Map<TimeTaskContext>(viewmodel);
        public List<TimeTaskContext> ListReverseMap(List<CalculatedTimeTaskViewModel> contexts)
        {
            var n = new List<TimeTaskContext>();
            foreach (var c in contexts)
                n.Add(ReverseMap(c));
            return n;
        }

        #endregion
    }
}
