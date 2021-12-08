using UnityEngine;
using System;

namespace SteveRogers
{
    public class Pool<T> where T : class
    {
        #region Variables (Private)

        // Stay

        private T[] originItems = null;

        public delegate T CreateIdleItemMethod(T busyingItem);
        private CreateIdleItemMethod CloneMethod = null;

        // Status

        private SafeList<T> idle = null;
        private SafeList<T> busy = null;

        // Ctor

        public Pool(CreateIdleItemMethod CloneMethod, bool isSingleType, params T[] items) // Main
        {
            if (items.IsNullOrEmpty())
            {
                throw new System.Exception("Not has any items!");
            }

            if (isSingleType)
                originItems = null;
            else
                originItems = items;

            this.CloneMethod = CloneMethod;

            idle = new SafeList<T>(items.Length);
            busy = new SafeList<T>(System.Math.Max(items.Length / 2, 1));

            foreach (var i in items)
                idle.Add(i);
        }

        public Pool(bool isUseDefaultCreateMethod, params T[] items) : this(null, true, items)
        {
            if (isUseDefaultCreateMethod)
                CloneMethod = CreateIdlePoolItemMethodDefault;
        }

        public Pool(params T[] items) : this(null, true, items) // Sub 
        {}

        public Pool(int preCloneCount, CreateIdleItemMethod CloneMethod, bool isSingleType, params T[] items) : this(CloneMethod, isSingleType, items) // Sub 
        {
            for (int i = 1; i < preCloneCount; i++)
            {
                foreach (var x in items)
                    idle.Add(x);
            }
        }

        #endregion

        #region Core

        public static T CreateIdlePoolItemMethodDefault(T busying)
        {
            GameObject newbie;

            if (typeof(T) == typeof(GameObject))
                newbie = Utilities.InstantiateFarFromHome(busying as GameObject);
            else
            {
                var go = busying as Component;

                if (go == null)
                {
                    Debug.LogErrorFormat("CreateIdlePoolItemMethodDefault - Cant convert {0} to Component! ", typeof(T));
                    return null;
                }

                newbie = Utilities.InstantiateFarFromHome(go.gameObject);
            }

            newbie.SetActive(false);

            if (typeof(T) == typeof(GameObject))
            {
                newbie.transform.position = (busying as GameObject).transform.position;
                return (newbie as T);
            }
            else
            {
                newbie.transform.position = (busying as Component).transform.position;
                return newbie.GetComponent<T>();
            }
        }

        #endregion

        #region Utils (Public)

        public bool IsAvailable { get { return idle.Count > 0; } }

        public Action<T> OnBeforeReturn = null;

        public T Pick()
        {
            if (idle.Count > 0)
            {
                var pick = idle[0];

                busy.Add(pick);
                idle.RemoveAt(0);

                return pick;
            }
            else
            {
                if (CloneMethod == null)
                    return null;
                else
                {
                    var newbie = CloneMethod(originItems == null ? busy[0] : originItems[busy.Count % originItems.Length]);
                    idle.Add(newbie);

                    return Pick();
                }
            }
        }

        public void Return(T item)
        {
            OnBeforeReturn.SafeCall(item);
            busy.Remove(item);
            idle.Add(item);
        }

        public void ReturnAll()
        {
            while (busy.Count > 0)
                Return(busy[0]);
        }

        #endregion
    }
}