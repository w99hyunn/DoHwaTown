using System;
using UnityEngine;

namespace DoHwa
{
    [CreateAssetMenu(menuName = "DoHwa/다방향 코디 아이템")]
    public sealed class DohwaEquipmentSet : ScriptableObject
    {
        [Serializable]
        public struct Frame
        {
            public Sprite[] parts;
            public Texture2D bodyMask;
            public Texture2D skinMask;
            public Sprite[] partsWithoutHair;
            public Texture2D bodyMaskWithoutHair;
            public Texture2D skinMaskWithoutHair;
        }

        public AvatarSlot slot;
        // Long hair uses the covered garment artwork; short hair uses complete restored garments.
        public bool coversBackClothing = true;
        // Direction-major: idle then the eight walk frames, same 8-direction order as AvatarFacing.
        public Frame[] frames;
    }
}
