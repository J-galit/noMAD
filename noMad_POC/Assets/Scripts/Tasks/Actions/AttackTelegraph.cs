using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions {

	public class AttackTelegraph : ActionTask {

		public BBParameter<GameObject> attackTelegraph;

		protected override void OnExecute() {
			GameObject.Instantiate(attackTelegraph.value, new Vector3(agent.transform.position.x, agent.transform.position.y, agent.transform.position.z), agent.transform.rotation);
			EndAction(true);
		}
	}
}