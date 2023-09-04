using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace Cyberpunk2077
{
    internal class InterfaceMono : MonoBehaviour
    {
        Creature creature;
        Holder holder;

        void Awake()
        {
            creature = transform.root.GetComponent<Creature>();
            holder = GetComponent<Holder>();

            holder.Load(Catalog.GetData<InteractableData>("InterfaceHolder"));
            holder.data.forceAllowTouchOnPlayer = false;
            holder.data.disableTouch = false;
            holder.RefreshChildAndParentHolder();

            Debug.Log($"Initializing Interface");
        }
        /*public void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.transform.root.TryGetComponent<Item>(out Item shard) && shard.data.id.StartsWith("Cyberpunk.DataShard"))
            {
                shard.data.GetModule<DataShardModule>().dataShard.OnCreatureInsert(creature);
            }
        }*/
    }
}
