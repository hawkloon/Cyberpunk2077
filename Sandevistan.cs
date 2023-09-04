using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;
using System.Collections;

namespace Cyberpunk2077
{
    public class Sandevistan : ItemModuleApparel
    {
        public static SandevistanMono activeMono;
        public IEnumerator EquipDelay(Creature creature)
        {
            yield return Yielders.ForSeconds(1.5f);
            if (Player.currentCreature.GetComponent<SandevistanMono>() != null) yield break;
            activeMono = creature.gameObject.AddComponent<SandevistanMono>();
        }
        public override void OnEquip(Creature creature, ApparelModuleType equippedOn, ItemModuleWardrobe.CreatureWardrobe wardrobeData)
        {
            base.OnEquip(creature, equippedOn, wardrobeData);
            GameManager.local.StartCoroutine(EquipDelay(creature));
        }
        public override void OnUnequip(Creature creature, ApparelModuleType equippedOn, ItemModuleWardrobe.CreatureWardrobe wardrobeData)
        {
            base.OnUnequip(creature, equippedOn, wardrobeData);
            if (!activeMono) return;
            GameObject.Destroy(activeMono);
        }
    }
    public class SandevistanMono : MonoBehaviour
    {
        bool active;
        GameObject volume;
        Gradient trailGradient;
        float gradientFactor;
        float originalScale;
        EffectData originalData;
        static float trailTimerMax = 0.075f;
        float trailTimer = trailTimerMax;
        Material trailMaterial;
        List<GameObject> activeTrailMeshes = new List<GameObject> ();
        public void Start()
        {
            if (Player.currentCreature)
            {
                var slowTime = Player.currentCreature.mana.GetPowerSlowTime();
                originalData = slowTime.effectData;
                slowTime.effectData = Catalog.GetData<EffectData>("Cyberpunk.Sandevistan.SFX");
                originalScale = slowTime.scale;
                GradientSetUp();
                Catalog.LoadAssetAsync<Material>("Cyberpunk.SandevistanTrailMaterial", mat => trailMaterial = mat, "Sandevistan Trail Material");
                slowTime.scale = 0.3f;
            }
        }

        public void GradientSetUp()
        {
            trailGradient = new Gradient();
            var colors = new GradientColorKey[3];
            colors[0] = new GradientColorKey(Color.red, 0.0f);
            colors[1] = new GradientColorKey(Color.green, 0.5f);
            colors[2] = new GradientColorKey(Color.blue, 1.0f);
            var alpha = new GradientAlphaKey[]
            {
                new GradientAlphaKey(0.0f, 0.0f),
                new GradientAlphaKey(0.0f, 1.0f)
            };
            trailGradient.SetKeys(colors, alpha);
            gradientFactor = 0;
        }
        public void OnDestroy()
        {
            if (Player.currentCreature)
            {
                var slowTime = Player.currentCreature.mana.GetPowerSlowTime();
                slowTime.scale = originalScale;
                slowTime.effectData = originalData;

            }
        }

        public Vector3 PlayerLagBehind(int i )
        {
            var vec = Player.currentCreature.renderers[i].renderer.transform.position + (-Player.currentCreature.currentLocomotion.velocity / 10);
            return vec;
        }


        private void TrailUpdate()
        {
            if(trailTimer > 0)
            {
                trailTimer -= Time.deltaTime;
                return;
            }

            trailTimer = trailTimerMax;

            if (Player.currentCreature.renderers == null || Player.currentCreature.renderers.Count == 0) return;

            if (Player.currentCreature.currentLocomotion.velocity.sqrMagnitude < 4 || trailMaterial == null) return;
            if (gradientFactor >= 1.0f) gradientFactor = 0.0f;

            gradientFactor += 0.1f;

            for (int i = 0; i < Player.currentCreature.renderers.Count; i++)
            {
                GameObject game = new GameObject();

                game.transform.SetPositionAndRotation(PlayerLagBehind(i), Player.currentCreature.renderers[i].renderer.transform.rotation);

                var renderer =game.AddComponent<MeshRenderer>();
                var filter = game.AddComponent<MeshFilter>();

                var mesh = new Mesh();

                Player.currentCreature.renderers[i].renderer.BakeMesh(mesh);

                filter.mesh = mesh;
                renderer.material = trailMaterial;
                if(Cyberpunk2077.useGradient)renderer.material.SetColor("_Color", trailGradient.Evaluate(gradientFactor));
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                activeTrailMeshes.Add(game);
                StartCoroutine(MaterialDissolve(renderer.material, 0, 0.005f, game));

            }


        }
        private IEnumerator MaterialDissolve(Material mat, float goal, float rate, GameObject game)
        {
            float f = mat.GetFloat("_Alpha");

            while(f > goal)
            {
                f -= rate;
                mat.SetFloat("_Alpha", f);
                yield return Yielders.EndOfFrame;
            }
            if (activeTrailMeshes.Contains(game)) activeTrailMeshes.Remove(game);
            Destroy(game);
        }


        void Update()
        {
            if (Player.currentCreature == null) return;
            switch (TimeManager.slowMotionState)
            {
                case TimeManager.SlowMotionState.Starting:
                    if (!active)
                    {
                        //Catalog.GetData<EffectData>("Cyberpunk.Sandevistan.Activation").Spawn(Player.currentCreature.transform).Play();
                        Catalog.InstantiateAsync("Cyberpunk.SandevistanVolume", Vector3.zero, Quaternion.identity, null, Volume => volume = Volume, "SandevistanVolume");
                        float speedMult = Cyberpunk2077.SandevistanSpeed;
                        Player.currentCreature.locomotion.SetSpeedModifier(speedMult, speedMult, speedMult, speedMult, speedMult, speedMult * 0.8f);

                    }
                    active = true;
                    break;
                case TimeManager.SlowMotionState.Stopping:
                    if (active)
                    {
                        if (volume != null) Destroy(volume);
                        TimeManager.SetTimeScale(1);
                        Player.currentCreature.locomotion.ClearSpeedModifiers();
                       // Catalog.GetData<EffectData>("Cyberpunk.Sandevistan.Deactivation").Spawn(Player.currentCreature.transform).Play();
                        active = false;
                        foreach (GameObject game in activeTrailMeshes) DestroyImmediate(game);
                        break;
                    }
                    break;
            }
            if (active) TrailUpdate();
        }
    }
}
