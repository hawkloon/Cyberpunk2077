using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace Cyberpunk2077
{
    public class DismemebermentShard : DataShard
    {
        public override string Name => "Limbs Be Gone Shard";

        public override void OnCreatureInsert(Creature creature, Item shard)
        {
            base.OnCreatureInsert(creature, shard);
            if (creature.isPlayer) return;
            creature.Kill();
            for (int i = creature.ragdoll.parts.Count - 1; i >= 0; --i) creature.ragdoll.parts[i].TrySlice();
        }
    }
}
