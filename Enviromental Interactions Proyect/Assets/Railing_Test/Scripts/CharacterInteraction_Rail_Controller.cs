using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    struct RailInteraction
    {
        public string characterInteracting;
        public Coroutine railCoroutine;
    }

public class CharacterInteraction_Rail_Controller : MonoBehaviour
{
    [SerializeField] private InteractableRailV2 railScript;

    List<RailInteraction> currentRailInteractions = new List<RailInteraction>();
    

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<LeftHandIKController>() != null)
            {
                print(1);

                var ikHandScript = other.GetComponent<LeftHandIKController>();

                RailInteraction newRailInteraction;
                newRailInteraction.characterInteracting = other.name;

                Transform characterTransform = other.transform;
                Transform handIkTargetTransform = ikHandScript.leftIkTarget.transform;
                newRailInteraction.railCoroutine = StartCoroutine(railScript.FixCharacterHandIKTargetToRail(characterTransform, handIkTargetTransform));

                ikHandScript.lefthandRig.weight = 1f;

                currentRailInteractions.Add(newRailInteraction);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (RailInteraction railInteraction in currentRailInteractions) 
            {
                if (railInteraction.characterInteracting == other.name) 
                {
                    print(2);

                    var ikHandScript = other.GetComponent<LeftHandIKController>();
                    ikHandScript.lefthandRig.weight = 0f;

                    StopCoroutine(railInteraction.railCoroutine);

                    currentRailInteractions.Remove(railInteraction);

                    break;
                }
            }
        }
    }
}
