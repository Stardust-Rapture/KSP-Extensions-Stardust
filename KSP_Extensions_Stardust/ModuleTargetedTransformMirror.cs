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

            if(mirrorScaleAxis != Vector3.zero)
            {
                transformTarget = CheckTransform(mirrorTarget);

                if (transformTarget != null)
                    Debug.LogWarning("TargetedMirroring debug Checkpoint 0 passed. Transform '" + mirrorTarget + "' found.");
                else
                    Debug.LogWarning("TargetedMirroring debug Checkpoint 0 failed. No transform '" + mirrorTarget + "' found.");
            }
            if (isMirrorApplied == true)
            {
                // Debug.LogWarning("Flight State mirror checkpoint");
                PerformMirror();
            }
        }

        public void Start()
        {
            // Debug.Log("TargetedMirroring debug message. Checkpoint 1.");
            // Debug.Log("TargetedMirroring debug. appliedScaling = " + transformTarget.localScale.ToString());

            part.OnEditorAttach += ApplyTargetedMirroring;
            part.OnEditorDetach += ReleaseTargetedMirroring;
        }

        private bool CheckMirroringState()
        {
            if (part.symMethod == SymmetryMethod.Mirror && part.symmetryCounterparts.Count > 0)
            {
                // Debug.Log("TargetedMirroring Checkpoint 2: Mirror state checked TRUE.");
                return true;
            }
            else
            {
                // Debug.Log("TargetedMirroring Checkpoint 2: Mirror state checked FALSE.");
                isMirrorApplied = false;
                return false;
            }
        }

        private void ApplyTargetedMirroring()
        { 
            // Debug.Log("MirrorScaleAxis: " + mirrorScaleAxis.ToString());
            // Debug.Log("Targeted Mirroring Debug. isMirrorApplied: " + isMirrorApplied.ToString());
            // Debug.LogWarning("Calculated Vector3.Dot = " +
            //    Vector3.Dot(EditorLogic.SortedShipList[0].transform.right, part.transform.position - EditorLogic.SortedShipList[0].transform.position).ToString() );

            if (CheckMirroringState() && Vector3.Dot(EditorLogic.SortedShipList[0].transform.TransformDirection(Vector3.right), part.transform.position - EditorLogic.SortedShipList[0].transform.position) > 0 )
            {
                transformTarget = part.FindModelTransform(mirrorTarget);

                if (transformTarget != null)
                    PerformMirror();

                return;
            }

            isMirrorApplied = false;
        }

        private void ReleaseTargetedMirroring()
        {
            isMirrorApplied = false;
            // Debug.Log("Targeted Mirror Released.");
        }
        private void PerformMirror()
        {
            Vector3 targetTransformScale = transformTarget.localScale;
            // Debug.Log("TargetedMirroring debug. Checkpoint 6 transformTarget: " + transformTarget.ToString());
            // Debug.Log("Mirror Target targetTransformScale: " + targetTransformScale.ToString());

            if (mirrorScaleAxis.x != 0f)
                targetTransformScale.x *= -1f;
            if (mirrorScaleAxis.y != 0f)
                targetTransformScale.y *= -1f;
            if (mirrorScaleAxis.z != 0f)
                targetTransformScale.z *= -1f;

            // Debug.Log("APPLIED AXIS MIRRORING. Debug: " + targetTransformScale.ToString());
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