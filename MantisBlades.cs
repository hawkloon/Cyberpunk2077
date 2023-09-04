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
    public class MantisBladeMono : MonoBehaviour
    {
        Item item;
        Animation animation;

        
        public void Start()
        {
            item = GetComponent<Item>();
            animation = GetComponent<Animation>();
            item.OnUngrabEvent += Item_OnUngrabEvent;
        }

        private RagdollPart GetArm(Side side)
        {
            foreach(RagdollPart part in Player.currentCreature.ragdoll.parts)
            {
                if(part.name == "LeftForeArm" && side == Side.Left)
                {
                    return part;
                }
                else
                {
                    if(part.name == "RightForeArm" && side == Side.Right)
                    {
                        return part;
                    }
                }
            }
            return null;
        }
        
        private void Item_OnUngrabEvent(Handle handle, RagdollHand ragdollHand, bool throwing)
        {
            item.transform.parent = GetArm(ragdollHand.side).transform;
            item.transform.localPosition = new Vector3(-0.210441127f, 0.0165335834f, -0.00406511873f);
            if (ragdollHand.side == Side.Right) item.transform.localRotation = new Quaternion(-0.706829607f, 0.0872395113f, 0.692297101f, -0.116215765f);
            else item.transform.localRotation = new Quaternion(0.00576885464f, -0.712169468f, 0.0248558037f, 0.701543748f);
            animation.Play("FoldIn");
            var joint = item.gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = GetArm(ragdollHand.side).physicBody.rigidBody;
            item.handles[0].SetTouch(false);
            StartCoroutine(DespawnAfterAnimationEnd(animation.GetClip("FoldIn"), ragdollHand.side));
        }


        private IEnumerator DespawnAfterAnimationEnd(AnimationClip clip, Side side)
        {
            yield return Yielders.ForSeconds(clip.length);
            item.Despawn();
        }
    }
}
