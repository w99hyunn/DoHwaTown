using UnityEngine;

namespace DoHwa
{
    public enum AvatarSlot { Hair, Top, Bottom, Boots, Ornaments }
    public enum AvatarMotion { Idle, Walk, Run, Dash }
    public enum AvatarFacing { South, SouthEast, East, NorthEast, North, NorthWest, West, SouthWest }

    public sealed class ModularCharacter : MonoBehaviour
    {
        [System.Serializable]
        private struct SlotRenderers
        {
            public AvatarSlot slot;
            public SpriteRenderer[] renderers;
        }

        [System.Serializable]
        private struct DirectionFrame
        {
            public AvatarFacing direction;
            public Sprite[] sprites;
            public int[] sortingOrders;
        }

        [System.Serializable]
        private struct MotionClip
        {
            public AvatarFacing direction;
            public AvatarMotion motion;
            public float framesPerSecond;
            public Sprite[] sprites; // Frame-major, then the same layer order as directions.
        }

        [SerializeField] private MotionClip[] motions;
        private AvatarMotion motion;
        private float motionTime;
        private int renderedFrame = -1;
        public AvatarMotion Motion => motion;
        public int FrameIndex => Mathf.Max(0, renderedFrame);

        [SerializeField] private SlotRenderers[] slots;
        [SerializeField] private SpriteRenderer[] layers;
        [SerializeField] private DirectionFrame[] directions;
        [SerializeField] private AvatarFacing facing;
        public AvatarFacing Facing => facing;

        public void SetFacing(AvatarFacing direction)
        {
            facing = direction;
            RenderFrame();
        }

        // One clock advances every layer, including hidden equipment. Turning keeps the gait phase.
        public void Animate(AvatarFacing direction, AvatarMotion nextMotion, float deltaTime, float playbackRate = 1f)
        {
            if (nextMotion != motion)
            {
                bool gaitChange = (motion == AvatarMotion.Walk || motion == AvatarMotion.Run)
                    && (nextMotion == AvatarMotion.Walk || nextMotion == AvatarMotion.Run);
                float phase = gaitChange ? Phase(FindMotion(facing, motion)) : 0f;
                motion = nextMotion;
                motionTime = gaitChange ? phase * Duration(FindMotion(direction, motion)) : 0f;
                renderedFrame = -1;
            }
            bool turned = facing != direction;
            facing = direction;
            motionTime = motion == AvatarMotion.Idle ? 0f : motionTime + Mathf.Max(0f, deltaTime * playbackRate);
            int frame = motion == AvatarMotion.Idle ? 0 : Frame(FindMotion(facing, motion));
            if (turned || renderedFrame != frame) RenderFrame();
        }

        private float Duration(MotionClip clip) => (clip.sprites.Length / layers.Length) / clip.framesPerSecond;
        private float Phase(MotionClip clip) => Mathf.Repeat(motionTime / Duration(clip), 1f);
        private int Frame(MotionClip clip)
        {
            int count = clip.sprites.Length / layers.Length;
            int frame = Mathf.FloorToInt(motionTime * clip.framesPerSecond);
            return motion == AvatarMotion.Dash ? Mathf.Min(frame, count - 1) : frame % count;
        }

        private void RenderFrame()
        {
            var idle = FindDirection(facing);
            var sprites = idle.sprites;
            int offset = 0;
            renderedFrame = 0;
            if (motion != AvatarMotion.Idle)
            {
                var clip = FindMotion(facing, motion);
                renderedFrame = Frame(clip);
                sprites = clip.sprites;
                offset = renderedFrame * layers.Length;
            }
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].sprite = sprites[offset + i];
                layers[i].sortingOrder = idle.sortingOrders[i];
            }
        }

        private MotionClip FindMotion(AvatarFacing direction, AvatarMotion kind)
        {
            foreach (var clip in motions)
                if (clip.direction == direction && clip.motion == kind) return clip;
            throw new System.ArgumentOutOfRangeException(nameof(kind), "Missing directional motion.");
        }

        // Replacement is frame-major: each frame contains every renderer belonging to this slot.
        public void SetSlotAnimation(AvatarSlot slot, AvatarFacing direction, AvatarMotion kind, Sprite[] sprites)
        {
            if (kind == AvatarMotion.Idle) { SetSlotSprites(slot, direction, sprites); return; }
            var renderers = FindSlot(slot);
            var clip = FindMotion(direction, kind);
            int count = clip.sprites.Length / layers.Length;
            if (sprites.Length != count * renderers.Length)
                throw new System.ArgumentException("Include every frame and layer in the equipment slot.", nameof(sprites));
            for (int f = 0; f < count; f++)
                for (int i = 0; i < renderers.Length; i++)
                {
                    var reference = clip.sprites[f * layers.Length + System.Array.IndexOf(layers, renderers[i])];
                    var sprite = sprites[f * renderers.Length + i];
                    if (sprite.rect.size != reference.rect.size || sprite.pivot != reference.pivot ||
                        !Mathf.Approximately(sprite.pixelsPerUnit, reference.pixelsPerUnit))
                        throw new System.ArgumentException("Equipment frames must match canvas, pivot and PPU.", nameof(sprites));
                }
            for (int f = 0; f < count; f++)
                for (int i = 0; i < renderers.Length; i++)
                    clip.sprites[f * layers.Length + System.Array.IndexOf(layers, renderers[i])] = sprites[f * renderers.Length + i];
            if (facing == direction && motion == kind) RenderFrame();
        }

        public void SetEquipped(AvatarSlot slot, bool equipped)
        {
            foreach (var renderer in FindSlot(slot)) renderer.enabled = equipped;
        }

        public void SetSlotSprites(AvatarSlot slot, Sprite[] sprites) => SetSlotSprites(slot, facing, sprites);

        // Equipment belongs to a direction frame, so turning preserves both replacements and visibility.
        public void SetSlotSprites(AvatarSlot slot, AvatarFacing direction, Sprite[] sprites)
        {
            var renderers = FindSlot(slot);
            var frame = FindDirection(direction);
            if (sprites.Length != renderers.Length)
                throw new System.ArgumentException("The replacement must include every layer in the slot.", nameof(sprites));
            for (int i = 0; i < sprites.Length; i++)
            {
                int layerIndex = System.Array.IndexOf(layers, renderers[i]);
                var reference = frame.sprites[layerIndex];
                if (sprites[i].rect.size != reference.rect.size || sprites[i].pivot != reference.pivot ||
                    !Mathf.Approximately(sprites[i].pixelsPerUnit, reference.pixelsPerUnit))
                    throw new System.ArgumentException("The replacement must match the reference canvas, pivot and PPU.", nameof(sprites));
            }
            for (int i = 0; i < sprites.Length; i++)
                frame.sprites[System.Array.IndexOf(layers, renderers[i])] = sprites[i];
            if (facing == direction) SetFacing(direction);
        }

        private SpriteRenderer[] FindSlot(AvatarSlot slot)
        {
            foreach (var group in slots)
                if (group.slot == slot) return group.renderers;
            throw new System.ArgumentOutOfRangeException(nameof(slot));
        }

        private DirectionFrame FindDirection(AvatarFacing direction)
        {
            foreach (var frame in directions)
                if (frame.direction == direction) return frame;
            throw new System.ArgumentOutOfRangeException(nameof(direction));
        }

        [ContextMenu("미리보기/기본 코디")]
        public void ShowDefaultOutfit()
        {
            foreach (var group in slots)
                foreach (var renderer in group.renderers) renderer.enabled = true;
        }

        [ContextMenu("미리보기/의상 해제")]
        public void ShowWithoutClothes()
        {
            ShowDefaultOutfit();
            SetEquipped(AvatarSlot.Top, false);
            SetEquipped(AvatarSlot.Bottom, false);
            SetEquipped(AvatarSlot.Boots, false);
        }

        [ContextMenu("미리보기/머리 해제")]
        public void ShowWithoutHair()
        {
            ShowDefaultOutfit();
            SetEquipped(AvatarSlot.Hair, false);
            SetEquipped(AvatarSlot.Ornaments, false);
        }

        [ContextMenu("미리보기/기본 몸체")]
        public void ShowBaseBody()
        {
            foreach (var group in slots)
                foreach (var renderer in group.renderers) renderer.enabled = false;
        }

        [ContextMenu("방향/정면")]
        private void FaceSouth() => SetFacing(AvatarFacing.South);
        [ContextMenu("방향/우측 정면")]
        private void FaceSouthEast() => SetFacing(AvatarFacing.SouthEast);
        [ContextMenu("방향/우측")]
        private void FaceEast() => SetFacing(AvatarFacing.East);
        [ContextMenu("방향/우측 후면")]
        private void FaceNorthEast() => SetFacing(AvatarFacing.NorthEast);
        [ContextMenu("방향/후면")]
        private void FaceNorth() => SetFacing(AvatarFacing.North);
        [ContextMenu("방향/좌측 후면")]
        private void FaceNorthWest() => SetFacing(AvatarFacing.NorthWest);
        [ContextMenu("방향/좌측")]
        private void FaceWest() => SetFacing(AvatarFacing.West);
        [ContextMenu("방향/좌측 정면")]
        private void FaceSouthWest() => SetFacing(AvatarFacing.SouthWest);
    }
}
