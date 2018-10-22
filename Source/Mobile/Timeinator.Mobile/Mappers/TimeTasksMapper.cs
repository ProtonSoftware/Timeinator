using AutoMapper;
using Timeinator.Mobile.DataAccess;

namespace Timeinator.Mobile
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
                config.CreateMap<TimeTask, TimeTaskContext>();
                // Create TimeTaskContext to TimeTaskViewModel map
                config.CreateMap<TimeTaskContext, TimeTaskViewModel>();
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

        #endregion
    }
}
