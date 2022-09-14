using System;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof(OperaComponent))]
    public static class OperaComponentSystem
    {
        [ObjectSystem]
        public class OperaComponentAwakeSystem : AwakeSystem<OperaComponent>
        {
            protected override void Awake(OperaComponent self)
            {
                self.mapMask = LayerMask.GetMask("Map");
            }
        }

        [ObjectSystem]
        public class OperaComponentUpdateSystem : UpdateSystem<OperaComponent>
        {
            protected override void Update(OperaComponent self)
            {
                self.Update();
            }
        }
        
        public static void Update(this OperaComponent self)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000, self.mapMask))
                {
                    self.ClickPoint = hit.point;
                    C2M_PathfindingResult c2MPathfindingResult = new C2M_PathfindingResult();
                    c2MPathfindingResult.X = self.ClickPoint.x;
                    c2MPathfindingResult.Y = self.ClickPoint.y;
                    c2MPathfindingResult.Z = self.ClickPoint.z;
                    self.ClientScene().GetComponent<SessionComponent>().Session.Send(c2MPathfindingResult);
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                CodeLoader.Instance.LoadHotfix();
                EventSystem.Instance.Load();
                Log.Debug("hot reload success!");
            }
            
            if (Input.GetKeyDown(KeyCode.T))
            {
                C2M_TransferMap c2MTransferMap = new C2M_TransferMap();
                self.ClientScene().GetComponent<SessionComponent>().Session.Call(c2MTransferMap).Coroutine();
            }
        }
    }
}