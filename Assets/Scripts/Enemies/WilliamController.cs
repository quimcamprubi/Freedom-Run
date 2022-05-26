using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Enemies {
    public class WilliamController : AIPatrol{
        public override void Die() {
            GameObject griselda = GameObject.Find("Griselda");
            griselda.GetComponent<OnTextCalled>().enabled = true;
            StartCoroutine(WaitAndLoadLevel6());
            base.Die();
        }

        private IEnumerator WaitAndLoadLevel6() {
            yield return new WaitForSeconds(3);
            SceneManager.LoadScene("Nivell_6_p2");
        }
    }
}
