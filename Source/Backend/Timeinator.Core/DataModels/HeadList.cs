using System;
using System.Collections.Generic;

namespace Timeinator.Core
{
    /// <summary>
    /// The extended list functionality that separates first element of the list and the remaining list
    /// </summary>
    /// <typeparam name="T">The type of object that list contains</typeparam>
    public class HeadList<T>
        where T : new()
    {
        #region Public Properties

        /// <summary>
        /// The first, head element
        /// </summary>
        public T Head => WholeList.Count > 0 ? WholeList[0] : default;

        /// <summary>
        /// The remaining list, without the head
        /// </summary>
        public List<T> RemainingList
        {
            get
            {
                if (WholeList.Count <= 0)
                    return new List<T>();

                // Copy the list so we don't modify current properties
                var listCopy = WholeList;

                // Remove head element from the beginning
                listCopy.RemoveAt(0);

                // And return that copy
                return listCopy;
            }
        }

        /// <summary>
        /// The whole list of elements containing the head
        /// </summary>
        public List<T> WholeList { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor with main list provided
        /// </summary>
        /// <param name="listData">The whole list of <see cref="T"/></param>
        public HeadList(List<T> listData)
        {
            // Set the inside list
            WholeList = listData;
        }

        /// <summary>
        /// Constructor with separate head and list provided
        /// </summary>
        /// <param name="first">The first element as head</param>
        /// <param name="remainingData">The list without first element</param>
        public HeadList(T first, List<T> remainingData)
        {
            // Insert first element to the provided list
            remainingData.Insert(0, first);

            // Set the inside list
            WholeList = remainingData;
        }

        #endregion
    }
}
