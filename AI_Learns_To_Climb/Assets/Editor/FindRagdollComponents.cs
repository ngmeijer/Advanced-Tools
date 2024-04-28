using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RagdollController))]
public class FindRagdollComponents : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RagdollController controller = (RagdollController)target;

        GUILayout.Label($"Body part count: {controller.GetBodyPartCount()}");
        if (GUILayout.Button("Assign Components For Limbs"))
        {
            BodyPartController[] bodyparts = FindObjectsOfType<BodyPartController>();

            foreach (BodyPartController bodyPart in bodyparts)
            {
                bodyPart.rb = bodyPart.GetComponent<Rigidbody>();
                bodyPart.joint = bodyPart.GetComponent<ConfigurableJoint>();

                Rigidbody connectedBody = null;
                Transform parentToInvestigate = bodyPart.transform.parent;

                //Keep going until finding a rigidbody
                while (connectedBody == null && parentToInvestigate != null)
                {
                    //Trying to get a rigidbody from the current parent
                    parentToInvestigate.TryGetComponent<Rigidbody>(out connectedBody);

                    //Caching 
                    parentToInvestigate = parentToInvestigate.parent;
                }

                bodyPart.joint.connectedBody = connectedBody;
            }

            EditorUtility.SetDirty(controller);
        }
    }
}
