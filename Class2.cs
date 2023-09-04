using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace Cyberpunk2077
{
    public static class Utils 
    {
        public static bool TryGeetOrAddComponent<T>(this GameObject gameObject, out T component) where T : Component
        {
            if(!gameObject.TryGetComponent<T>(out component))
            {
                component = gameObject.AddComponent<T>();
                return false;
            }

            return component;
        }
    }
}
