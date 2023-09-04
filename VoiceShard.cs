using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using ThunderRoad;
using UnityEngine;

namespace Cyberpunk2077
{
    /*public class VoiceShard : DataShard
    {
        public override string Name => "Voice Shard";

        public override void OnCreatureInsert(Creature creature, Item shard)
        {
            base.OnCreatureInsert(creature, shard);
            if (creature.isPlayer) creature.gameObject.AddComponent<VoiceShardMono>();
            else
            {
                shard.holder.UnSnap(shard, true);
            }
        }
        public override void OnCreatureRemove(Creature creature, Item shard)
        {
            base.OnCreatureRemove(creature, shard);
            if (creature.isPlayer)
            {
                creature.gameObject.TryGetComponent<VoiceShardMono>(out var voiceShard);
                if (voiceShard) GameObject.Destroy(voiceShard);
            }
        }
    }
    public class VoiceShardMono : MonoBehaviour
    {
        Creature creature;

        private AudioClip OnHealthLowAudio;
        private AudioClip OnDamageTakenAudio;

        public AudioSource audioSource;
        bool canSpeak;
        bool canSpeakDamageTaken;

        public void Start()
        {
            creature = GetComponent<Creature>();
            canSpeak = true;
            canSpeakDamageTaken = true;
            audioSource = SetAudioSource();
            StartCoroutine(GetVoiceLines());
            EventManager.onCreatureHit += EventManager_onCreatureHit;
        }

        private void EventManager_onCreatureHit(Creature creature, CollisionInstance collisionInstance, EventTime eventTime)
        {
            if (!creature.isPlayer || audioSource == null || !canSpeak || !canSpeakDamageTaken || !OnDamageTakenAudio) return;
            audioSource.PlayOneShot(OnDamageTakenAudio);
            StartCoroutine(BlockOut(1f));
            StartCoroutine(DelayDamageTaken(20f));

        }

        private AudioSource SetAudioSource()
        {
           var audio = Player.currentCreature.gameObject.AddComponent<AudioSource>();
            audio.loop = false;
            return audio;
        }
        private IEnumerator GetVoiceLines()
        {
            yield return Catalog.LoadAssetCoroutine<AudioClip>("Cyberpunk.HealthLowAudio", result => OnHealthLowAudio = result, "Cyberpunk.HealthLow");
            yield return Catalog.LoadAssetCoroutine<AudioClip>("Cyberpunk.DamageTakenAudio", result => OnDamageTakenAudio = result, "Cyberpunk.DamageTaken");
            yield return Catalog.LoadAssetCoroutine<AudioClip>("Cyberpunk.VoiceShardStartAudio", result =>
            {
                if (audioSource && canSpeak)
                {
                    audioSource.PlayOneShot(result);
                    BlockOut(1f);
                }
            }, "OnShardStart");


        }

        private IEnumerator BlockOut(float delay)
        {
            canSpeak = false;
            yield return Yielders.ForSeconds(delay);
            canSpeak = true;
        }
        private IEnumerator DelayDamageTaken(float delay)
        {
            canSpeakDamageTaken = false;
            yield return Yielders.ForSeconds(delay);
            canSpeakDamageTaken = true;
        }

    }*/
}
