using System;
using UnityEngine;

namespace StardustUtilities
{
    public class SAETargetedTransformMirror : PartModule
    {
        [KSPField(isPersistant = true)]
        public string mirrorTarget = null;

        [KSPField(isPersistant = true)]
        public Vector3 mirrorScaleAxis = Vector3.zero;

        [KSPField(isPersistant = true)]
        public bool isMirrorApplied;
        
        public Transform transformTarget;

        public override void OnStart(PartModule.StartState state)
        {
            mirrorScaleAxis.Normalize();
            isMirrorApplied = false;

            if(mirrorScaleAxis != Vector3.zero)
            {
                transformTarget = CheckTransform(mirrorTarget);

                if (transformTarget != null)
                    Debug.LogWarning("TargetedMirroring debug Checkpoint 0 passed.");
                else
                    Debug.LogWarning("TargetedMirroring debug Checkpoint 0 failed.");
            }
        }

        public void Start()
        {
            mirrorScaleAxis.Normalize();

            Debug.Log("TargetedMirroring debug message. Checkpoint 1.");
            // Debug.Log("TargetedMirroring debug message. mirrorScaleAxis = " + mirrorScaleAxis.ToString());
            if (CheckMirroringState() && isMirrorApplied == false)
            {
                part.OnEditorAttach += ApplyTargetedMirroring;
                part.OnEditorDetach += ReleaseTargetedMirroring;
            }
            else if(CheckMirroringState() && isMirrorApplied == true)
            {
                ApplyTargetedMirroring();
            }
        }

        private bool CheckMirroringState()
        {
            if (part.symMethod == SymmetryMethod.Mirror && part.symmetryCounterparts.Count > 0)
            {
                Debug.Log("TargetedMirroring Checkpoint 2: Mirror state checked.");
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ApplyTargetedMirroring()
        { 
            Debug.Log("TargetedMirroring debug message. Checkpoint 5 partSymCount: " + part.symmetryCounterparts.Count.ToString());
            Debug.Log("MirrorScaleAxis: " + mirrorScaleAxis.ToString());
            Debug.Log("Targeted Mirroring Debug. isMirrorApplied: " + isMirrorApplied.ToString());
            // Debug.Log("Debugging Symmetry Part at attPos0.x = " + part.attPos0.x.ToString());
            Debug.LogWarning("Calculated Vector3.Dot = ");
            Debug.Log(Vector3.Dot(EditorLogic.SortedShipList[0].transform.right, part.transform.position - EditorLogic.SortedShipList[0].transform.position).ToString());

            if (CheckMirroringState()  && Vector3.Dot(EditorLogic.SortedShipList[0].transform.TransformDirection(Vector3.right), part.transform.position - EditorLogic.SortedShipList[0].transform.position) < 0 )
            {
                // Vector3.Dot(EditorLogic.SortedShipList[0].transform.right, part.transform.position - EditorLogic.SortedShipList[0].transform.position) < 0

                Debug.LogWarning("ATTEMPTING TRANSFORM MIRROR on " + part.partInfo.name.ToString());

                Part mirrorPart = part.symmetryCounterparts[0];
                transformTarget = mirrorPart.FindModelTransform(mirrorTarget);

                if (transformTarget != null)
                    PerformMirror();
            }

        }

        private void ReleaseTargetedMirroring()
        {
            isMirrorApplied = true;
            Debug.Log("Targeted Mirror Released.");
        }

        private void PerformMirror()
        {
            Vector3 targetTransformScale = transformTarget.localScale;
            Debug.Log("TargetedMirroring debug. Checkpoint 6 transformTarget: " + transformTarget.ToString());
            Debug.Log("Mirror Target targetTransformScale: " + targetTransformScale.ToString());

            if (mirrorScaleAxis.x != 0f)
                targetTransformScale.x *= -1f;
            if (mirrorScaleAxis.y != 0f)
                targetTransformScale.y *= -1f;
            if (mirrorScaleAxis.z != 0f)
                targetTransformScale.z *= -1f;

            Debug.Log("APPLIED AXIS MIRRORING. Debug: " + targetTransformScale.ToString());
            Debug.Log(part.symmetryCounterparts[0].ToString());

            transformTarget.localScale = targetTransformScale;
            isMirrorApplied = true;
        }

        public virtual Transform CheckTransform(string targetCheck)
        {
            return part.FindModelTransform(targetCheck);
        }
    }
}