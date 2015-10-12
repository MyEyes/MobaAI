using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace MobaLib
{
    public class AIController
    {
        protected Champion champion;
        protected Map map;

        public AIController(Map map, Champion champion)
        {
            this.map = map;
            this.champion = champion;
        }

        public virtual void Update(float dt)
        {

        }

        public static AIController Instantiate(string dllName, string className, Map map, Champion champion)
        {
            Assembly assembly = Assembly.LoadFile(Path.GetFullPath(dllName));
            Type t = assembly.GetType(className);
            ConstructorInfo constructor = t.GetConstructor(new Type[] { typeof(Map), typeof(Champion) });
            return constructor.Invoke(new object[] { map, champion }) as AIController;
        }
    }
}
