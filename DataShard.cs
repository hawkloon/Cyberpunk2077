using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using ThunderRoad;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;

namespace Cyberpunk2077
{
    public class DataShard
    {
        public virtual string Name { get; set; }

        public static List<ItemData> shardDatas = new List<ItemData>();

        public static void GetAllInheriters()
        {
            /*IEnumerable<DataShard> shards = typeof(DataShard)
               .Assembly.GetTypes()
               .Where(t => t.IsSubclassOf(typeof(DataShard)) && !t.IsAbstract)
               .Select(t => (DataShard)Activator.CreateInstance(t));*/
            List<IEnumerable<DataShard>> shards = new List<IEnumerable<DataShard>>();
            foreach(var mod in ModManager.loadedMods)
            {
                if (mod.assemblies.Any())
                {
                    foreach(var asm in mod.assemblies)
                    {
                        var e = asm.GetTypes()
                            .Where(t => t.IsSubclassOf(typeof(DataShard)) && !t.IsAbstract)
                            .Select(t => (DataShard)Activator.CreateInstance(t));
                        shards.Add(e);
                    }
                }
            }
            foreach(var shard in shards)
            {
                foreach(var s in shard)
                {
                    AddModule(s);
                }
            }
            
            EventManager.onLevelLoad += EventManager_onLevelLoad;
        }

        private static void EventManager_onLevelLoad(LevelData levelData, EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart) return;
            var spawners = (UIItemSpawner[])UnityEngine.Object.FindObjectsOfType(typeof(UIItemSpawner));
            if (spawners == null || spawners.Length == 0) return;
            for(int i = 0; i < spawners.Length; i++)
            {
                foreach(ItemData data in shardDatas)
                {
                    spawners[i].container.AddContent(data);
                }
                spawners[i].RefreshItems(spawners[i].container);
            }
        }

        public static void AddPrefabLocationToData(ItemData data)
        {
            var e = Catalog.GetData<ItemData>("Cyberpunk.DataShardTemplate");
            data.prefabLocation = e.prefabLocation;
            data.iconLocation = e.iconLocation;
            data.category = e.category;
        }
        public static ItemData DataShardData(DataShard shard)
        {
            var data = new ItemData();
            var ShardNamePrefix = shard.Name.Split(' ');
            data.id = $"Cyberpunk.DataShard.{ShardNamePrefix[0]}";
            data.localizationId = $"Cyberpunk.DataShard.{ShardNamePrefix[0]}";
            data.displayName = shard.Name;
            data.author = "Hawkloon";
            data.prefabAddress = "Cyberpunk.DataShard";
            data.slot = "DataShard";
            AddPrefabLocationToData(data);
            data.iconAddress = "Cyberpunk.DataShard.Icon";
            var damager = new ItemData.Damager();
            damager.transformName = "Blunt";
            damager.damagerID = "PropBlunt";
            data.damagers.Add(damager);
            var handle = new ItemData.Interactable();
            handle.transformName = "Handle";
            handle.interactableId = "ObjectHandleLight";
            data.Interactables.Add(handle);
            data.snapAudioContainerAddress = "ShardIn";
            data.purchasable = true;
            var m = new DataShardModule();
            m.dataShard = shard;
            data.modules = new List<ItemModule>();
            data.modules.Add(m);
            data.OnCatalogRefresh();
            shardDatas.Add(data);
            return data;
        }

        public static void AddModule(DataShard shard)
        {

            var data = DataShard.DataShardData(shard);

            Catalog.LoadCatalogData(data, null, null, null);
            foreach(ItemModule mod in data.modules)
            {
                Debug.Log(mod.type.Name);
            }
        }
        public virtual void OnCreatureInsert(Creature creature, Item shard)
        {

        }

        public virtual void OnCreatureRemove(Creature creature, Item shard)
        {

        }
    }

    public class DataShardModule : ItemModule
    {
        public DataShard dataShard;

        Item shard;

        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            shard = item;
            shard.OnSnapEvent += Shard_OnSnapEvent;
            shard.OnUnSnapEvent += Shard_OnUnSnapEvent;
        }

        private void Shard_OnUnSnapEvent(Holder holder)
        {
            if (holder.interactableId == "InterfaceHolder" && holder.GetComponentInParent<Creature>() is Creature c)
            {
                foreach (ColliderGroup group in shard.colliderGroups)
                {
                    group.gameObject.SetActive(true);
                }
                dataShard.OnCreatureRemove(c, item);
            }
        }

        private void Shard_OnSnapEvent(Holder holder)
        {
            if(holder.interactableId == "InterfaceHolder" && holder.GetComponentInParent<Creature>() is Creature c)
            {
                dataShard.OnCreatureInsert(c, item);
                foreach(ColliderGroup group in shard.colliderGroups)
                {
                    group.gameObject.SetActive(false);
                }
            }
        }
    }

    public class VirusShard : DataShard
    {
        public override string Name => "Virus Shard";
        public override void OnCreatureInsert(Creature creature, Item shard)
        {
            base.OnCreatureInsert(creature, shard);
            creature.TryElectrocute(100, 5f, true, false);
            creature.handLeft.UnGrab(false);
            creature.handRight.UnGrab(false);
        }
    }

    public class CompanionShard : DataShard
    {
        public override string Name => "Companion Shard";
        public override void OnCreatureInsert(Creature creature, Item shard)
        {
            base.OnCreatureInsert(creature, shard);
            creature.SetFaction(Player.currentCreature.factionId);
            //creature.brain.instance.Load(creature);
            creature.brain.Load(creature.brain.instance.id);
        }
    }

    public class ParalysisShard : DataShard
    {
        public override string Name => "Paralysis Shard";

        public override void OnCreatureInsert(Creature creature, Item shard)
        {
            base.OnCreatureInsert(creature, shard);
            creature.ragdoll.SetState(Ragdoll.State.Destabilized);
            creature.brain.AddNoStandUpModifier(this);
        }


        public IEnumerator RemoveModifier(Creature creature)
        {
            yield return Yielders.ForSeconds(10f);
            creature.brain.RemoveNoStandUpModifier(this);
            creature.brain.Load(creature.brain.instance.id);
        }
    }
}
