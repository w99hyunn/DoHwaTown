using UnityEngine;
using UnityEngine.InputSystem;

namespace DoHwa
{
    public sealed class DohwaWardrobeControls : MonoBehaviour
    {
        [SerializeField] private DohwaLayeredCharacter character;
        [SerializeField] private DohwaEquipmentSet ivoryJacket;
        [SerializeField] private DohwaEquipmentSet roseJacket;
        private InputActionMap controls;
        private readonly bool[] visible = { true, true, true, true, true };
        private bool rose;

        private void Awake()
        {
            controls = new InputActionMap("코디 미리보기");
            for (int i = 0; i < visible.Length; i++)
                controls.AddAction("Slot" + i, InputActionType.Button, "<Keyboard>/" + (i + 1));
            controls.AddAction("Jacket", InputActionType.Button, "<Keyboard>/6");
            controls.AddAction("Reset", InputActionType.Button, "<Keyboard>/0");
        }

        private void OnEnable() => controls.Enable();
        private void OnDisable() => controls.Disable();
        private void OnDestroy() => controls.Dispose();

        private void Update()
        {
            for (int i = 0; i < visible.Length; i++)
                if (controls["Slot" + i].WasPressedThisFrame())
                {
                    visible[i] = !visible[i];
                    character.SetEquipped((AvatarSlot)i, visible[i]);
                }
            if (controls["Jacket"].WasPressedThisFrame())
            {
                rose = !rose;
                visible[1] = true;
                character.SetEquipment(rose ? roseJacket : ivoryJacket);
            }
            if (controls["Reset"].WasPressedThisFrame())
            {
                rose = false;
                character.SetEquipment(ivoryJacket);
                for (int i = 0; i < visible.Length; i++)
                {
                    visible[i] = true;
                    character.SetEquipped((AvatarSlot)i, true);
                }
            }
        }
    }
}
