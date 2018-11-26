using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld.Weapon.Shooting
{
    public class SingleShotController : ShotController
    {
        [SerializeField] private string m_buttonFire = "Fire1";

        protected override void Update()
        {
            base.Update();

            if ((!m_isActive) ||
                (m_isDirectCallOnly)) return;

            // check if global ui unactive
            if ((Input.GetButtonDown(m_buttonFire)) &&
                (m_state == Data.ShotState.ReadyToShot) &&
                (SetShotTarget(Input.mousePosition, true)))
            {
                m_state = Data.ShotState.ShotWaiting;
                m_weaponHandler.OnShotProbability();
            }
        }

    }
}
