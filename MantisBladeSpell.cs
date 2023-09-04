using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace Cyberpunk2077
{
    public class MantisBladeSpell : SpellCastCharge
    {
        public override void Fire(bool active)
        {
            base.Fire(active);
            if (active)
            {
                Catalog.GetData<ItemData>("MantisBladeBlade").SpawnAsync(blade =>
                {
                    spellCaster.ragdollHand.Grab(blade.GetMainHandle(spellCaster.side), true);
                    blade.gameObject.AddComponent<MantisBladeMono>();
                }, spellCaster.transform.position, spellCaster.transform.rotation);
            }
        }
    }
}
