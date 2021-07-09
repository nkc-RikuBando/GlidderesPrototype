using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    public class RuleSelect : MonoBehaviour
    {
        [SerializeField] GameObject rulePanel;
        [SerializeField] GameObject stagePanel;
        [SerializeField] GameObject charctorPanel;

        // Start is called before the first frame update
        void Start()
        {
            stagePanel.SetActive(false);

            if (PublicStaticBool.isCreate)
            {
                rulePanel.SetActive(true);
                charctorPanel.SetActive(false);
            }
            else
            {
                charctorPanel.SetActive(true);
                rulePanel.SetActive(false);
            }
        }
    }

}
