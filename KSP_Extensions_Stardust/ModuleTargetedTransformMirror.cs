using System;
using UnityEngine;

namespace StardustUtilities
{
    public class TargetedTransformMirror : PartModule
    {
        [KSPField(isPersistant = true, guiActive = false)]
        public string mirrorTarget = null;

        [KSPField(isPersistant = true, guiActive = false)]
        public Vector3 mirrorScaleAxis = Vector3.zero;

        private Transform transformTarget;
        
        public override void OnStart(PartModule.StartState state)
        {
            mirrorScaleAxis.Normalize();
            if(mirrorScaleAxis != Vector3.zero)
            {
                transformTarget = CheckTransform(mirrorTarget);

                if (transformTarget == null)
                    Debug.LogWarning("No transform of name '" + mirrorTarget + "' found in " + part.partInfo.name);
            }
        }

        public void Start()
        {
            mirrorScaleAxis.Normalize();
            part.OnEditorAttach += TargetedMirroring;
            part.OnEditorDetach += TargetedMirroring;

            Debug.Log("TargetedMirroring debug message. mirrorScaleAxis = " + mirrorScaleAxis.ToString());
        }

        private void TargetedMirroring()
        {
            Debug.Log("TargetedMirroring debug message. transformTarget: " + transformTarget.ToString());
            if (transformTarget != null)
                if (part.symMethod == SymmetryMethod.Mirror && part.symmetryCounterparts.Count > 0)
                {

                    Part mirrorPart = part.symmetryCounterparts[0];
                    Debug.Log("Attempting transform mirror.");

                    Transform mirrorTransform = mirrorPart.FindModelTransform(mirrorTarget);
                    transformTarget = mirrorTransform;
                    Vector3 targetTransformScale = transformTarget.localScale;
                    Debug.Log("targetTransformScale: " + targetTransformScale.ToString());

                    if (mirrorScaleAxis.x != 0f)
                        targetTransformScale.x *= -1f;
                    if (mirrorScaleAxis.y != 0f)
                        targetTransformScale.y *= -1f;
                    if (mirrorScaleAxis.z != 0f)
                    {
                        targetTransformScale.z *= -1f;
                        Debug.Log("Applied Z-axis mirror. Debug: " + targetTransformScale.z.ToString());
                        Debug.Log(part.symmetryCounterparts[0].ToString());
                    }

                    transformTarget.localScale = targetTransformScale;

                }
        }

        public virtual Transform CheckTransform(string mirrorTarget)
        {
            return part.FindModelTransform(mirrorTarget);
        }
    }
}