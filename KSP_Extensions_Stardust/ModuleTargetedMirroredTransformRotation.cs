using System;
using UnityEngine;

namespace StardustUtilities
{
    class SAETargetedMirroredTransformRotation : PartModule
    {
        [KSPField(isPersistant = true)]
        public string rotationOverrideTarget = null;

        [KSPField(isPersistant = true)]
        public Vector3 overrideRotationAxis = Vector3.zero;

        [KSPField(isPersistant = true)]
        public bool isRotationOverriden;

        public Transform transformTarget;

        public override void OnStart(PartModule.StartState state)
        {
            if (overrideRotationAxis != Vector3.zero)
            {
                transformTarget = CheckTransform(rotationOverrideTarget);

                if (transformTarget != null)
                    Debug.LogWarning("TargetedRotation debug Checkpoint 0 passed. Transform '" + rotationOverrideTarget + "' found.");
                else
                    Debug.LogWarning("TargetedRotation debug Checkpoint 0 failed. No transform '" + rotationOverrideTarget + "' found.");
            }
            if (isRotationOverriden == true)
            {
                PerformRotationOverride();
            }
        }

        public void Start()
        {
            part.OnEditorAttach += ApplyTargetedRotationOverride;
            part.OnEditorDetach += ReleaseTargetedRotationOverride;
        }
        private bool CheckMirroringState()
        {
            if (part.symMethod == SymmetryMethod.Mirror && part.symmetryCounterparts.Count > 0)
            {
                return true;
            }
            else
            {
                isRotationOverriden = false;
                return false;
            }
        }

        private void ApplyTargetedRotationOverride()
        {
            if (CheckMirroringState() && Vector3.Dot(EditorLogic.SortedShipList[0].transform.TransformDirection(Vector3.right), part.transform.position - EditorLogic.SortedShipList[0].transform.position) > 0)
            {
                transformTarget = part.FindModelTransform(rotationOverrideTarget);

                if (transformTarget != null)
                    PerformRotationOverride();

                return;
            }

            isRotationOverriden = false;
        }

        private void ReleaseTargetedRotationOverride()
        {
            isRotationOverriden = false;
        }

        private void PerformRotationOverride()
        {
            Vector3 targetTransformRotation = transformTarget.localEulerAngles;

            if (overrideRotationAxis.x != 0f)
                targetTransformRotation.x = overrideRotationAxis.x;
            if (overrideRotationAxis.y != 0f)
                targetTransformRotation.y = overrideRotationAxis.y;
            if (overrideRotationAxis.z != 0f)
                targetTransformRotation.z = overrideRotationAxis.z;

            transformTarget.localEulerAngles = targetTransformRotation;
            isRotationOverriden = true;
        }

        public virtual Transform CheckTransform(string targetCheck)
        {
            return part.FindModelTransform(targetCheck);
        }

    }
}
