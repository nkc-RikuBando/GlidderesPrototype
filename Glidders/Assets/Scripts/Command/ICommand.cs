using UnityEngine;

namespace Glidders
{
    namespace Command
    {
        interface ICommand
        {
            void SetCommandTab();
            void CommandStart();
            void CommandUpdate();
            void SetCharacterObject(GameObject gameObject);
        }
    }
}
