using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Character;

namespace Glidders
{
    namespace Manager
    {
        public class AutoSignalSelecter
        {
            public void SignalSet(CharacterData signalSetCharaData)
            {
                CharacterScriptableObject character = signalSetCharaData.thisObject.GetComponent<CharacterCore>().characterScriptableObject; // ïœçXì_ÅH

                if (signalSetCharaData.moveSignal.moveDataArray[0] == FieldIndexOffset.zero)
                {
                    for (int i = 0; i < character.moveAmount; i++)
                    {
                        switch(Random.Range(0,4))
                        {
                            case 0:
                                signalSetCharaData.moveSignal.moveDataArray[i] = FieldIndexOffset.up;
                                break;
                            case 1:
                                signalSetCharaData.moveSignal.moveDataArray[i] = FieldIndexOffset.down;
                                break;
                            case 2:
                                signalSetCharaData.moveSignal.moveDataArray[i] = FieldIndexOffset.right;
                                break;
                            case 3:
                                signalSetCharaData.moveSignal.moveDataArray[i] = FieldIndexOffset.left;
                                break;
                        }
                    }
                }
                
                if (!signalSetCharaData.attackSignal.skillData)
                {
                    AttackSignal attackSignal;
                    FieldIndexOffset fieldIndexOffset = new FieldIndexOffset(1, 1);

                    int skillRandomNumber = Random.Range(0, 3);

                    attackSignal.skillData = character.skillDataArray[skillRandomNumber]; ;
                    attackSignal.skillNumber = skillRandomNumber;
                    attackSignal.isAttack = true;

                    switch (Random.Range(0, 4))
                    {
                        case 0:
                            fieldIndexOffset = FieldIndexOffset.up;
                            break;
                        case 1:
                            fieldIndexOffset = FieldIndexOffset.down;
                            break;
                        case 2:
                            fieldIndexOffset = FieldIndexOffset.right;
                            break;
                        case 3:
                            fieldIndexOffset = FieldIndexOffset.left;
                            break;
                    }

                    attackSignal.direction = fieldIndexOffset;
                    attackSignal.selectedGrid = signalSetCharaData.index + fieldIndexOffset;

                    signalSetCharaData.attackSignal = attackSignal;
                }

                // signalSetCharaData.attackSignal.skillData = character.skillDataArray[skillRandomNumber];
            }
        }
    }
}