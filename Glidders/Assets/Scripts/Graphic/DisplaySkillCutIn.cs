using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

namespace Glidders
{
    namespace Graphic
    {

        public class DisplaySkillCutIn : MonoBehaviour
        {
            [SerializeField] private Sprite[] characterSprites;
            [SerializeField] private Sprite[] cutInSprites;

            [SerializeField] private Image cutInCharacterImage;
            [SerializeField] private Image cutInImage;
            [SerializeField] private Text skillNameText;

            [SerializeField] private GameObject cutInObject;

            [SerializeField] private Character.UniqueSkillScriptableObject testSkill = default;
            [SerializeField] private int testNumber = 0;

            private Animator cutInAnimation;

            // Start is called before the first frame update
            void Start()
            {
                cutInAnimation = cutInObject.GetComponent<Animator>();
                cutInObject.SetActive(false);
            }

            // Update is called once per frame
            void Update()
            {
                //if (Input.GetKeyDown(KeyCode.Space)) StartSkillCutIn(testNumber, testSkill.skillName);
            }

            public void StartSkillCutIn(int characterId, string skillName)
            {
                cutInObject.SetActive(true);
                skillNameText.text = skillName;
                cutInCharacterImage.sprite = characterSprites[characterId];
                cutInImage.sprite = cutInSprites[characterId];
                cutInAnimation.SetTrigger("CutInStart");
                Time.timeScale = 0.25f;
            }
        }

    }
}
