using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    public interface IPlayerInformation
    {
        UICharacterDataSeter[] characterDataSeter();

        CharacterName CharacterNameReturn(int playerID);
    }
}