using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace Cyberpunk2077
{
    public class Cyberpunk2077 : ThunderScript
    {
        public static bool ShardsGenerated = false;


        public static ModOptionBool[] sandevistanToggle()
        {
            var modOpt= new ModOptionBool[2];
            modOpt[0] = new ModOptionBool("Sandevistan not toggled from book", false);
            modOpt[1] = new ModOptionBool("Sandevistan toggled from book", true);
            return modOpt;
        }
        public static ModOptionFloat[] sandevistanSpeed()
        {
            ModOptionFloat[] option = new ModOptionFloat[20];
            for(int i = 0; i < option.Length; i++)
            {

                option[i] = new ModOptionFloat(i.ToString("0"), i);
            }
            return option;
        }

        [ModOptionButton]
        [ModOptionSaveValue(false)]
        [ModOption("Sandevistan Toggle", "Don't like the sandevistan armor? hit here", valueSourceName = nameof(sandevistanToggle), defaultValueIndex = 0)]
        public static void ToggleSande(bool active)
        {
            if (Sandevistan.activeMono || !Player.currentCreature) return;
            if (active)
            {
                Player.currentCreature.gameObject.AddComponent<SandevistanMono>();
            }
            else
            {
                if(Player.currentCreature.GetComponent<SandevistanMono>() is var mono)
                {
                    GameObject.Destroy(mono);
                }
            }
        }

        [ModOption("Gradient Trail", "the trail you leave in the sandevistan mode will gradually change color", category = "Sandevistan", defaultValueIndex = 0)]
        public static bool useGradient;
        [ModOption("Sandevistan", "How much your speed is multiplied when sandevistan is active", category = "Sandevistan", valueSourceName = nameof(sandevistanSpeed), defaultValueIndex = 19)]

        public static float SandevistanSpeed;
        public override void ScriptLoaded(ModManager.ModData modData)
        {
            base.ScriptLoaded(modData);
            EventManager.onCreatureSpawn += EventManager_onCreatureSpawn;
        }


        public string[] InterfaceCreatures =
        {
            "HumanFemale",
            "HumanMale",
            "PlayerDefaultMale",
            "PlayerDefaultFemale"
        };

        private void PlayerSpawn(Creature creature)
        {
            if(ShardsGenerated == false)
            {
                DataShard.GetAllInheriters();
                ShardsGenerated = true;
            }
        }
        private void EventManager_onCreatureSpawn(Creature creature)
        {
            if (creature.isPlayer) PlayerSpawn(creature);
            if (InterfaceCreatures.Contains(creature.data.id))
            {
                Catalog.InstantiateAsync("Cyberpunk.Interface", Vector3.zero, Quaternion.identity, creature.ragdoll.headPart.transform, inter =>
                {
                    if (creature.data.gender == CreatureData.Gender.Male)
                    {
                        inter.transform.localPosition = new Vector3(-0.012427764f, 0.0732002184f, -0.00174463412f);
                    }
                    else if (creature.data.gender == CreatureData.Gender.Female)
                    {
                        inter.transform.localPosition = new Vector3(-0.0258000009f, 0.0680000037f, -0.0115999999f);
                    }
                    inter.transform.localEulerAngles = new Vector3(350.016968f, 271.12146f, 8.66899143e-07f);

                    inter.gameObject.AddComponent<InterfaceMono>();
                }, "Cyberpunk.Interface");
            }
        }
    }
}
