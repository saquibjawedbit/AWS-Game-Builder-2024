
using UnityEngine;
using GoSystem;
using GoSystem.Control;
namespace GoSystem
{
    [GBehaviourAttributeAttribute("Rag Doll", true)]
    public class GoRagdollController : GoSystemsBehaviour
    {
        #region Var
        private class BoneTransform
        {
            public Vector3 Position { get; set; }

            public Quaternion Rotation { get; set; }
        }
        public enum PlayerState
        {
            Walking,
            Ragdoll,
            StandingUp,
            ResettingBones
        }
        private float _timeToResetBones = 0.1f;
        private Rigidbody[] _ragdollRigidbodies;
        private PlayerState _currentState = PlayerState.Walking;
        private Animator _animator;
        private CharacterController _characterController;
        private float _timeToWakeUp; public float TimeToWakeUp = 4f;
        private Transform _hipsBone;
        private BoneTransform[] _faceUpStandUpBoneTransforms;
        private BoneTransform[] _faceDownStandUpBoneTransforms;
        private BoneTransform[] _ragdollBoneTransforms;
        private Transform[] _bones;
        private float _elapsedResetBonesTime;
        private bool _isFacingUp, GetTimeFall;

        GoSystem.GoSystems Gs;
        GoSystem.GoCharacterController cc;
        public bool RagDall =true;
      [HideInInspector]  public bool ActivateRagDoll; private float savetime; public float fallTime = 2f;
        public LayerMask layerActive;
        public LayerMask layerActiveWhenHeat;
        [System.Serializable]
        public class Gooptimaistion
        {
            public string _faceUpStandUpStateName = "stand up backword";
            public string _faceDownStandUpStateName = "stand up forword";
            public string _faceUpStandUpClipName = "StandUp Back";
            public string _faceDownStandUpClipName = "Stand Up Forworld";
        }
        public Gooptimaistion AnimationsName;
        private GameObject LinkPosChar;
        private Transform HipsCharBone;
        public UnityEngine.Events.UnityEvent OnStartAction, OnExitAction, OnAction;
        #endregion
        void Awake()
        {
            cc = gameObject.GetComponent<GoCharacterController>();
            Gs = GoSystems.getSystem(gameObject);
            makelink();
            _animator = GetComponent<Animator>();
            _hipsBone = _animator.GetBoneTransform(HumanBodyBones.Hips);
            HipsCharBone = _hipsBone;
            _ragdollRigidbodies = _hipsBone.GetComponentsInChildren<Rigidbody>();
            _timeToWakeUp = TimeToWakeUp;
            _bones = _hipsBone.GetComponentsInChildren<Transform>();
            _faceUpStandUpBoneTransforms = new BoneTransform[_bones.Length];
            _faceDownStandUpBoneTransforms = new BoneTransform[_bones.Length];
            _ragdollBoneTransforms = new BoneTransform[_bones.Length];
            for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
            {
                _faceUpStandUpBoneTransforms[boneIndex] = new BoneTransform();
                _faceDownStandUpBoneTransforms[boneIndex] = new BoneTransform();
                _ragdollBoneTransforms[boneIndex] = new BoneTransform();
            }
            PopulateAnimationStartBoneTransforms(AnimationsName._faceUpStandUpClipName, _faceUpStandUpBoneTransforms);
            PopulateAnimationStartBoneTransforms(AnimationsName._faceDownStandUpClipName, _faceDownStandUpBoneTransforms);
            DisableRagdoll();
        }
        private void Start()
        {
            Gs.RagdallActivate = ActivateRagDoll;
            if (Gs.IsGround == true)
            {
                savetime = fallTime;
                DisableRagdoll();
            }
        }
        private void ActiveEngredinte()
        {
            if (_currentState != PlayerState.Ragdoll && Physics.CheckCapsule(transform.position, GoSystemsController.bones[10].position, 0.7f, layerActiveWhenHeat))
            {
                _currentState = PlayerState.Ragdoll;
                ActivateRagDoll = true;
            }

        }
        private void FallBehaveior()
        {
            if (Gs.IsRagdall)
            {
              
                if (!cc.IsGround )
                {
                    if (!GetTimeFall )
                    {
                        Gs.TimeFall = fallTime;
                        GetTimeFall = true;
                    }
                    if (_currentState == PlayerState.Walking)
                    {
                        Gs.TimeFall -= Time.deltaTime;
                        if (Gs.TimeFall < 0)
                        {
                            Gs.TimeFall = 0;
                        }
                    }
                }
                else
                {

                    if (Gs.TimeFall <= 0)
                    {
                        if (GoTrigger.other != null)
                        {
                            var IsLayer = (layerActive.value & (1 << GoTrigger.other.transform.gameObject.layer)) > 0;
                            if (!IsLayer) return;

                            if ( _currentState == PlayerState.Walking)
                            {
                                _currentState = PlayerState.Ragdoll;
                                ActivateRagDoll = true;

                                Gs.TimeFall = fallTime;
                            }

                        }
                    }
                    else if (Gs.TimeFall > 0 && _currentState == PlayerState.Walking)
                    {
                        Gs.TimeFall = fallTime;
                        GetTimeFall = true;
                    }
                }
            }
        }
        private void Update()
        {
            if (!isLocalPlayer) return;
            switch (_currentState)
            {
                case PlayerState.Walking:
                    FallBehaveior();
                    ActiveEngredinte();
                    break;
                case PlayerState.Ragdoll:
                    RagdollBehaviour();
                    break;
                case PlayerState.StandingUp:
                    StandingUpBehaviour();
                    break;
                case PlayerState.ResettingBones:
                    ResettingBonesBehaviour();
                    break;
            }
            Gs.RagdallActivate = ActivateRagDoll;
            Gs.IsRagdall = RagDall;
        }



        private void RagdollBehaviour()
        {
            var Movemint = _hipsBone.GetComponent<Rigidbody>().velocity.magnitude;
            _timeToWakeUp -= Time.deltaTime;
            EnableRagdoll();
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            if (_timeToWakeUp <= 0)
            {
                _isFacingUp = _hipsBone.forward.y > 0;
                AlignRotationToHips();
                AlignPositionToHips();
                UnLinkItInPlace();
                if (Movemint < 0.1f)
                {
                    _currentState = PlayerState.ResettingBones;
                    _elapsedResetBonesTime = 0;
                }
            }
            else
            {
                LinkItInPlace();
            }
        }
        public void TriggerRagdoll(Vector3 force, Vector3 hitPoint)
        {
            Rigidbody hitRigidbody = FindHitRigidbody(hitPoint);
            hitRigidbody.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
            _currentState = PlayerState.Ragdoll;
        }
        private Rigidbody FindHitRigidbody(Vector3 hitPoint)
        {
            Rigidbody closestRigidbody = null;
            float closestDistance = 0;
            foreach (var rigidbody in _ragdollRigidbodies)
            {
                float distance = Vector3.Distance(rigidbody.position, hitPoint);
                if (closestRigidbody == null || distance < closestDistance)
                {
                    closestDistance = distance;
                    closestRigidbody = rigidbody;
                }
            }
            return closestRigidbody;
        }
        private void DisableRagdoll()
        {
            foreach (var rigidbody in _ragdollRigidbodies)
            {
                rigidbody.isKinematic = true;
                rigidbody.GetComponent<Collider>().enabled = false;
            }
            _animator.enabled = true;
            ExitRagdoll();
        }
        private void EnableRagdoll()
        {
            foreach (var rigidbody in _ragdollRigidbodies)
            {
                rigidbody.isKinematic = false;
                rigidbody.GetComponent<Collider>().enabled = true;
            }
            LockSystems();
        }
        private void StandingUpBehaviour()
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(GetStandUpStateName()) == false)
            {
                _currentState = PlayerState.Walking;
            }
        }
        private void ResettingBonesBehaviour()
        {
            _elapsedResetBonesTime += Time.deltaTime;
            var elapsedPercentage = _elapsedResetBonesTime / _timeToResetBones;
            GoSystemsController.GoisKinematic(true);
            BoneTransform[] standUpBoneTransforms = GetStandUpBoneTransforms();
            for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
            {
                _bones[boneIndex].localPosition = Vector3.Lerp(
                    _ragdollBoneTransforms[boneIndex].Position,
                    standUpBoneTransforms[boneIndex].Position,
                    elapsedPercentage);
                _bones[boneIndex].localRotation = Quaternion.Lerp(
                    _ragdollBoneTransforms[boneIndex].Rotation,
                    standUpBoneTransforms[boneIndex].Rotation,
                    elapsedPercentage);
            }
            if (elapsedPercentage >= 1)
            {
                _currentState = PlayerState.StandingUp;
                DisableRagdoll();
                _animator.Play(GetStandUpStateName(), 0, 0);
            }
        }
        private void AlignRotationToHips()
        {
            var originalHipsPosition = _hipsBone.position;
            var originalHipsRotation = _hipsBone.rotation;
            var desiredDirection = _hipsBone.up;
            if (_isFacingUp)
            {
                desiredDirection *= -1;
            }
            desiredDirection.y = 0;
            desiredDirection.Normalize();
            var fromToRotation = Quaternion.FromToRotation(transform.forward, desiredDirection);
            transform.rotation *= fromToRotation;

            _hipsBone.position = originalHipsPosition;
            _hipsBone.rotation = originalHipsRotation;
        }
        private void AlignPositionToHips()
        {
            var originalHipsPosition = _hipsBone.position;
            transform.position = _hipsBone.position;
            var positionOffset = GetStandUpBoneTransforms()[0].Position;
            positionOffset.y = 0;
            positionOffset = transform.rotation * positionOffset;
            transform.position -= positionOffset;
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo))
            {
                transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
            }
            _hipsBone.position = originalHipsPosition;
        }
        private void PopulateBoneTransforms(BoneTransform[] boneTransforms)
        {
            for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
            {
                boneTransforms[boneIndex].Position = _bones[boneIndex].localPosition;
                boneTransforms[boneIndex].Rotation = _bones[boneIndex].localRotation;
            }
        }
        private void PopulateAnimationStartBoneTransforms(string clipName, BoneTransform[] boneTransforms)
        {
            var positionBeforeSampling = transform.position;
            var rotationBeforeSampling = transform.rotation;
            foreach (AnimationClip clip in _animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == clipName)
                {
                    clip.SampleAnimation(gameObject, 0);
                    PopulateBoneTransforms(boneTransforms);
                    break;
                }
            }
            transform.position = positionBeforeSampling;
            transform.rotation = rotationBeforeSampling;
        }
        private string GetStandUpStateName()
        {
            return _isFacingUp ? AnimationsName._faceUpStandUpStateName : AnimationsName._faceDownStandUpStateName;
        }
        private BoneTransform[] GetStandUpBoneTransforms()
        {
            return _isFacingUp ? _faceUpStandUpBoneTransforms : _faceDownStandUpBoneTransforms;
        }
        private void LockSystems()
        {
            Gs.IsLockMove = true;
            Gs.IsLockJump = true;
            ActivateRagDoll = true;
            GoSystemsController.Charcter.GetComponent<CapsuleCollider>().isTrigger = true;
            _animator.enabled = false;
            OnStartAction.Invoke();
            GoSystems.IsFoods = false;
            GoSystems.IsLags = false;
        }
        public void UnLockSystems()
        {
            _currentState = PlayerState.Walking;
            GetTimeFall = false;
            _timeToWakeUp = TimeToWakeUp;
            ActivateRagDoll = false;
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
            GoSystemsController.Charcter.GetComponent<CapsuleCollider>().isTrigger = false;
            OnExitAction.Invoke();
            Gs.IsLockMove = false;print("dodo dodo");
            Gs.IsLockJump = false;
        }
        void LinkItInPlace()
        {
            if (LinkPosChar.transform.parent != HipsCharBone.parent)
            {
                HipsCharBone.transform.SetParent(LinkPosChar.transform);
            }
            transform.position = HipsCharBone.position;
        }
        void UnLinkItInPlace()
        {
            HipsCharBone.transform.SetParent(transform.transform);
            PopulateBoneTransforms(_ragdollBoneTransforms);
        }
        private void ExitRagdoll()
        {
            GoSystems.OnExit = new UnityEngine.Events.UnityEvent();
            GoSystems.OnExit.AddListener(UnLockSystems);

        }
        void makelink()
        {
            if (LinkPosChar == null)
            {
                LinkPosChar = new GameObject("GoSystemsBehaviour");
            }
        }
    }
}