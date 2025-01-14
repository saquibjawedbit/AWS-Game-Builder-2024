using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoSystem.Control;
namespace GoSystem
{
    [GBehaviourAttributeAttribute("ZipLine", true)]
    public class GoZipLineSystem : GoSystemsBehaviour
    {
        zipLineApi zipline;
        string Zipline = "zipline";
        Transform A, B, C;
        public float Speed = 6f, OffSet= -2.3f;
        private  bool lockcode = true,lockInput;
        private bool MobileInputZip;
        public GoInput input ;
        GoInputSystem GIS =new GoInputSystem();
        private float TimeToEnd=1;
        GoSystems Gs;
        GoCharacterController GCC;

      public  UnityEngine.Events.UnityEvent OnStartAction , OnExitAction, OnAction;

        private void Awake()
        {

            Gs = GoSystems.getSystem(gameObject);
        }


        // Update is called once per frame
        void Update()
        {
          
            if (GoTrigger.other != null) {

                var getInput = GIS.GetKeyDown(input.Kaybord.ToString(), input.Joystick.ToString(), MobileInputZip);
                if (GoTrigger.other.GetComponent<zipLineApi>() != null)
                {
                    if (lockInput == false)
                        if (getInput)
                        {
         
                            LockCode();
                            zipline = GoTrigger.other.GetComponent<zipLineApi>();
                            A = zipline.PointA;
                            B = zipline.PointB;
                            C = zipline.PointC;
                            Collider point = GoTrigger.other.GetComponent<Collider>();
                            C.position = point.ClosestPoint(transform.position);

                        }
                }
            }
                if (lockcode == false)
            {

                Gs.Dircection(C, B.transform.position, Speed,true,true);
                transform.position = Gs.FixPosition(C.position, transform.position, 0, OffSet);
               
                if (TimeToEnd > 0)
                {
                    TimeToEnd -= Time.deltaTime;
                    Gs.FixRotation(transform, B, 8,Vector3.zero);
                }
                var Dis = Vector3.Distance(C.position, B.position);
                if (Dis<=0+1)
                {
                    UnLockCode();
                }
            }

        }


        private void LockCode()
        {
            OnStartAction.Invoke();
            Gs.AxisCtrl = false;
            Gs.LockAllInputs(true);
            Gs.IsZipLine = true;
            GoSystemsController.GoisKinematic(true);
            GoSystemsController.GoUseGravety(false);
            lockcode = false;
            lockInput = true;
            GoSystems.animatorControler.SetTrigger(Zipline);
          

        }

        private void UnLockCode()
        {
            OnExitAction.Invoke();
            Gs.AxisCtrl = true;
            Gs.LockAllInputs(false);
            Gs.IsZipLine = false;
            GoSystemsController.GoisKinematic(false);
            GoSystemsController.GoUseGravety(true);
            lockcode = true;
            lockInput = false;
            TimeToEnd = 1;
            GoSystems.animatorControler.SetTrigger(Zipline);

        }
        public void OnMobileJumpClick()
        {
            StartCoroutine(input.MobileInput(SetZipBool));
        }
        private void SetZipBool(bool status)
        {
           MobileInputZip = status;
        }
    }
}