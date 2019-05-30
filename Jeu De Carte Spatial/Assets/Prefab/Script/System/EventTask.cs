using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EventTask : NetworkBehaviour {

	[SyncVar]
	protected NetworkInstanceId originAction;

	[SyncVar]
	protected NetworkInstanceId joueur;

	[SyncVar]
	protected int idSelectionnableTarget;

	[SyncVar]
	protected int typeEvent;

	/**Dans le cas les tasks doivent être executer dans l'odre de creation
	 * on impose que les taches filles soit crée en tant que tache de frère de même hiérachie
	 * */
	[SyncVar]
	protected bool createTaskBrother = false;

	[SyncVar]
	protected bool activate = false;

	[SyncVar]
	protected bool pause = true;

	[SyncVar]
	protected bool finish = false;

	public void initVariable(NetworkInstanceId origin, NetworkInstanceId joueur, int target, int typeEvent, bool createTaskBrother){
		this.originAction = origin;
		this.idSelectionnableTarget = target;
		this.typeEvent = typeEvent;
		this.joueur = joueur;
		this.createTaskBrother = createTaskBrother;
	}

	public void Update(){
		if (isServer) {
			if (finish) {
				endOfTask ();
			} else {
				if (transform.GetSiblingIndex () == 0 && transform.childCount == 0) {
					if (this.activate) {
						this.pause = false;
					} else {
						activateTask ();
					}
				} else if (this.activate) {
					this.pause = true;
				}

				if (this.activate && !this.pause) {
					launchEventAction ();
				}
			}
		}
	}

	protected virtual void activateTask (){
		this.activate = true;
		EventTaskUtils.launchEventCapacity(this.typeEvent, this.originAction, this.joueur, this.idSelectionnableTarget, this.netId);
	}

	protected virtual void launchEventAction (){
		NetworkInstanceId netIdParentFuturTask;

		if (!createTaskBrother) {
			netIdParentFuturTask = this.netId;
		} else if (null != transform.parent && null != transform.parent.gameObject.GetComponent<EventTask> ()) {
			EventTask taskParent = transform.parent.gameObject.GetComponent<EventTask> ();
			netIdParentFuturTask = taskParent.netId;
		} else {
			netIdParentFuturTask = NetworkInstanceId.Invalid;
		}

		EventTaskUtils.launchEventAction (this.typeEvent, this.originAction, this.joueur, this.idSelectionnableTarget, netIdParentFuturTask);
		//TODO annimation 
		endOfTask();
	}

	public void endOfTask(){
		GameObject goParent = transform.parent.gameObject;
		EventTask eventParent = goParent.GetComponent<EventTask> ();
		if (transform.childCount > 0) {
			finish = true;
		}else if (null != eventParent) { //appartient a une tache parent
			eventParent.CmdNextTask ();
		} else {
			CmdDestroyTask ();
		}
	}


	[Command]
	public void CmdNextTask(){
		if (transform.childCount > 0) {
			NetworkManager.Destroy (transform.GetChild (0).gameObject);
		} else {
			//TODO is finish
			//endOfTask ();
		}
	}

	[Command]
	public void CmdDestroyTask(){
		NetworkManager.Destroy (this.gameObject);
	}

	public bool EnCours {
		get {return this.activate && !(this.pause || this.finish);}
	}
}
