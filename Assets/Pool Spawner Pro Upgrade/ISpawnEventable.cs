using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Assets.Pool_Spawner_Pro_Upgrade
{
    public interface ISpawnEventable
    {
        event Action SpawnEvent;
    }
}
