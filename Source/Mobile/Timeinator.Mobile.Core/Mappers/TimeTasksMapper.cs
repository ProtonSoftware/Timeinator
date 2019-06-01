﻿using AutoMapper;
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
                // Create TimeTaskContext to TimeTaskViewModel map
                config.CreateMap<TimeTaskContext, TimeTaskViewModel>()
                      // For context => VM mapping, assign the assigned time flag accordingly
                      .ForMember(viewmodel => viewmodel.IsAssignedTime, options => options.MapFrom(context => context.HasConstantTime))
                      // And the other way around        
                      .ReverseMap()
                      // For VM => context mapping, assign the constant time flag accordingly
                      .ForMember(context => context.HasConstantTime, options => options.MapFrom(vm => vm.IsAssignedTime && vm.AssignedTime != default));
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
        /// Maps a list of <see cref="TimeTask"/> to a list of <see cref="TimeTaskContext"/> objects
        /// </summary>
        /// <param name="entities">The list of <see cref="TimeTask"/> to map</param>
        /// <returns>The list of <see cref="TimeTaskContext"/></returns>
        public List<TimeTaskContext> ListMap(List<TimeTask> entities) => mMapper.Map<List<TimeTaskContext>>(entities);

        /// <summary>
        /// Maps a <see cref="TimeTaskContext"/> to a <see cref="TimeTaskViewModel"/> object
        /// </summary>
        /// <param name="context">The <see cref="TimeTaskContext"/> to map</param>
        /// <returns><see cref="TimeTaskViewModel"/></returns>
        public TimeTaskViewModel Map(TimeTaskContext context) => mMapper.Map<TimeTaskViewModel>(context);

        /// <summary>
        /// Maps a list of <see cref="TimeTaskContext"/> to a a list of <see cref="TimeTaskViewModel"/> objects
        /// </summary>
        /// <param name="contexts">The list of <see cref="TimeTaskContext"/> to map</param>
        /// <returns>The list of <see cref="TimeTaskViewModel"/></returns>
        public List<TimeTaskViewModel> ListMap(List<TimeTaskContext> contexts) => mMapper.Map<List<TimeTaskViewModel>>(contexts);

        /// <summary>
        /// Maps a <see cref="TimeTaskContext"/> to a <see cref="CalculatedTimeTaskViewModel"/> object
        /// </summary>
        /// <param name="context">The <see cref="TimeTaskContext"/> to map</param>
        /// <returns><see cref="CalculatedTimeTaskViewModel"/></returns>
        public CalculatedTimeTaskViewModel MapCal(TimeTaskContext context) => mMapper.Map<CalculatedTimeTaskViewModel>(context);

        /// <summary>
        /// Maps a list of <see cref="TimeTaskContext"/> to a list of <see cref="CalculatedTimeTaskViewModel"/> objects
        /// </summary>
        /// <param name="contexts">The list of <see cref="TimeTaskContext"/> to map</param>
        /// <returns>The list of <see cref="CalculatedTimeTaskViewModel"/></returns>
        public List<CalculatedTimeTaskViewModel> ListMapCal(List<TimeTaskContext> contexts) => mMapper.Map<List<CalculatedTimeTaskViewModel>>(contexts);

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

        /// <summary>
        /// Maps a list of <see cref="TimeTaskViewModel"/> to a list of <see cref="TimeTaskContext"/> objects
        /// </summary>
        /// <param name="viewmodels">The list of <see cref="TimeTaskViewModel"/> to map</param>
        /// <returns>The list of <see cref="TimeTaskContext"/></returns>
        public List<TimeTaskContext> ListReverseMap(List<TimeTaskViewModel> viewmodels) => mMapper.Map<List<TimeTaskContext>>(viewmodels);

        /// <summary>
        /// Maps a <see cref="CalculatedTimeTaskViewModel"/> to a <see cref="TimeTaskContext"/> object
        /// </summary>
        /// <param name="viewmodel">The <see cref="CalculatedTimeTaskViewModel"/> to map</param>
        /// <returns><see cref="TimeTaskContext"/></returns>
        public TimeTaskContext ReverseMap(CalculatedTimeTaskViewModel viewmodel) => mMapper.Map<TimeTaskContext>(viewmodel);

        /// <summary>
        /// Maps a list of <see cref="CalculatedTimeTaskViewModel"/> to a list of <see cref="TimeTaskContext"/> objects
        /// </summary>
        /// <param name="viewmodels">The list of <see cref="CalculatedTimeTaskViewModel"/> to map</param>
        /// <returns>The list of <see cref="TimeTaskContext"/></returns>
        public List<TimeTaskContext> ListReverseMap(List<CalculatedTimeTaskViewModel> viewmodels) => mMapper.Map<List<TimeTaskContext>>(viewmodels);

        #endregion
    }
}
