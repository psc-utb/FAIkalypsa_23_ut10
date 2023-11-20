using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Pool_Spawner_Pro_Upgrade
{
    public interface IDespawnable
    {
        event Action<GameObject, bool> Despawn;
    }
}
